using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SubstitutionService;
using SubstitutionService.Models;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstitutionController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetSubstitutions()
        {
            var manager = new SubstitutionManager();
            var subs = manager.GetSubstitutions();
            return new JsonResult(subs);
        }
    }
}
