using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolApi.Models;
using SubstitutionService;
using SubstitutionService.Models;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstitutionsController : ControllerBase
    {
        private readonly SubstitutionManager _manager;

        public SubstitutionsController(SubstitutionManager manager)
        {
            _manager = manager;
        }


        [HttpGet]
        public JsonResult GetSubstitutions()
        {
            return new JsonResult(_manager.GetSubstitutions());
        }

        [HttpGet]
        [Route("classes")]
        public JsonResult GetSubstitutionsSortedByClass()
        {
            var subs = _manager.GetSubstitutions();

            var dict = new SortedDictionary<string, SortedDictionary<string, List<ClassSubstitution>>>();

            foreach (var kvp in subs)
            {
                dict.Add(kvp.Key, new SortedDictionary<string, List<ClassSubstitution>>());

                foreach (var tKvp in kvp.Value)
                {
                    foreach (var lesson in tKvp.Value)
                    {
                        if (!dict[kvp.Key].ContainsKey(lesson.ClassName))
                        {
                            dict[kvp.Key].Add(lesson.ClassName, new List<ClassSubstitution>());
                        }
                        dict[kvp.Key][lesson.ClassName].Add(new ClassSubstitution(lesson, tKvp.Key));
                    }
                }
            }

            foreach (var kvp in dict)
            {
                foreach (var cKvp in kvp.Value)
                {
                    cKvp.Value.OrderBy(x => x.Number);
                }
            }

            return new JsonResult(dict);
        }
    }
}
