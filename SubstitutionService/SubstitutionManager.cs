using Newtonsoft.Json;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using SubstitutionService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubstitutionService
{
    public class SubstitutionManager
    {
        public SubstitutionManager()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // Set the website URI
        private readonly Uri _website = new Uri("http://zastepstwa.staff.edu.pl/");
        //x private readonly Uri _website = new Uri("http://localhost:8000/zast.html");
        //x private readonly Uri _website = new Uri("http://localhost:8000/zast2.html");

        // Set the regexes
        private readonly Regex _multidayRegex = new Regex(@"[0-9]{2}\.[0-9]{2}\.[0-9]{4} - [0-9]{2}\.[0-9]{2}\.[0-9]{4}");
        private readonly Regex _dayRegex = new Regex(@"([0-9]{2}\.[0-9]{2}\.[0-9]{4})");
        private readonly Regex _emptyRegex = new Regex(@"W dniu [0-9\-]{9,10} nie zaplanowano zastępstw.");
        private readonly Regex _teacherRegex = new Regex(@"(.*) \/ ([0-9\.]*)");

        public Dictionary<string, Dictionary<string, List<Lesson>>> GetSubstitutions()
        {
            // Create browser instance
            var browser = new ScrapingBrowser
            {
                // Tell browser to use ISO-8859-2 (Central European (ISO)) encoding
                Encoding = Encoding.GetEncoding("ISO-8859-2")
            };

            // Go to page
            var page = browser.NavigateToPage(_website);

            // Select all rows
            var rows = page.Html.CssSelect("tr").ToList();

            // Create a data dictionary
            var subs = new Dictionary<string, Dictionary<string, List<Lesson>>>();

            // Set flags and temporary variables
            bool isMultiday = false;
            string lastDay = "";
            string lastTeacher = "";

            if (_emptyRegex.IsMatch(rows[1].InnerText.Trim()))
            {
                return subs;
            }

            // Iterate through rows
            for (int i = 0; i < rows.Count; i++)
            {
                // Set row from list
                var row = rows[i];

                // Check if row is first
                if (i == 0)
                {
                    // Check if substitutions are multi-day
                    isMultiday = _multidayRegex.IsMatch(row.InnerText.Trim());

                    // Set first key
                    lastDay = _dayRegex.Matches(row.InnerText.Trim())[0].Value;
                    subs.Add(lastDay, new Dictionary<string, List<Lesson>>());
                    continue;
                }

                if (isMultiday)
                {
                    // Spans multiple days
                    if (row.ChildNodes.Count == 3)
                    {
                        // Is a teacher row
                        var matches = _teacherRegex.Matches(row.ChildNodes[1].InnerText.Trim());

                        lastTeacher = matches[0].Groups[1].Value;
                        lastDay = matches[0].Groups[2].Value;

                        //x Console.WriteLine(lastTeacher + " " + lastDay);

                        if (subs.ContainsKey(lastDay))
                        {
                            subs[lastDay][lastTeacher] = new List<Lesson>();
                        }
                        else
                        {
                            subs.Add(lastDay, new Dictionary<string, List<Lesson>>());
                            subs[lastDay][lastTeacher] = new List<Lesson>();
                        }
                    }
                    else if (row.ChildNodes.Count == 9)
                    {
                        // Is a lesson row
                        if (row.ChildNodes[1].InnerText.Trim() == "&nbsp;")
                        {
                            // Empty row (separator)
                            //x Console.WriteLine("Empty row");
                            continue;
                        }
                        else if (row.ChildNodes[1].InnerText.Trim() == "lekcja")
                        {
                            // Header row
                            //x Console.WriteLine("Header row");
                            continue;
                        }

                        // Set lesson parameters
                        int num = int.Parse(row.ChildNodes[1].InnerText.Trim());
                        string className = row.ChildNodes[3].InnerText.Trim().Split('-')[0].Trim();
                        string description = row.ChildNodes[3].InnerText.Trim().Split('-')[1].Trim();
                        string subtitute = row.ChildNodes[5].InnerText.Trim() == "&nbsp;" ? null : row.ChildNodes[5].InnerText.Trim();
                        string comment = row.ChildNodes[7].InnerText.Trim() == "&nbsp;" ? null : row.ChildNodes[7].InnerText.Trim();

                        // Add lesson to list
                        subs[lastDay][lastTeacher].Add(new Lesson
                        {
                            Number = num,
                            ClassName = className,
                            Description = description,
                            Substitute = subtitute,
                            Comment = comment
                        });
                    }
                }
                else
                {
                    // Spans one day
                    if (row.ChildNodes.Count == 3)
                    {
                        // Is a teacher row
                        lastTeacher = row.ChildNodes[1].InnerText.Trim();
                        //x Console.WriteLine(lastTeacherName);
                        subs[lastDay][lastTeacher] = new List<Lesson>();
                    }
                    else if (row.ChildNodes.Count == 9)
                    {
                        // Is a lesson row
                        if (row.ChildNodes[1].InnerText.Trim() == "&nbsp;")
                        {
                            // Empty row (separator)
                            //x Console.WriteLine("Empty row");
                            continue;
                        }
                        else if (row.ChildNodes[1].InnerText.Trim() == "lekcja")
                        {
                            // Header row
                            //x Console.WriteLine("Header row");
                            continue;
                        }

                        // Set lesson parameters
                        int num = int.Parse(row.ChildNodes[1].InnerText.Trim());
                        string className = row.ChildNodes[3].InnerText.Trim().Split('-')[0].Trim();
                        string description = row.ChildNodes[3].InnerText.Trim().Split('-')[1].Trim();
                        string subtitute = row.ChildNodes[5].InnerText.Trim() == "&nbsp;" ? null : row.ChildNodes[5].InnerText.Trim();
                        string comment = row.ChildNodes[7].InnerText.Trim() == "&nbsp;" ? null : row.ChildNodes[5].InnerText.Trim();

                        // Add lesson to list
                        subs[lastDay][lastTeacher].Add(new Lesson
                        {
                            Number = num,
                            ClassName = className,
                            Description = description,
                            Substitute = subtitute,
                            Comment = comment
                        });
                    }
                }
            }

            // Convert dictionary into string
            var json = JsonConvert.SerializeObject(subs);

            // Change encoding from ISO-8859-2 to UTF-8 (like normal people do, duh)
            var bytesJson = Encoding.GetEncoding("ISO-8859-2").GetBytes(json);
            var utfJsonBytes = Encoding.Convert(Encoding.GetEncoding("ISO-8859-2"), Encoding.UTF8, bytesJson);
            var utfJson = Encoding.UTF8.GetString(utfJsonBytes);

            // Convert string back into dictionary
            var utfSubs = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Lesson>>>>(utfJson);

            // Return substitutions
            return utfSubs;
        }
    }
}
