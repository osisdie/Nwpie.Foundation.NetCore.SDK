using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Nwpie.MiniSite.KVS.Endpoint.Controllers
{
    [Route("[controller]")]
    public class CodeController : ControllerBase
    {
        /// <summary>
        /// GET: Code
        /// </summary>
        [HttpGet]
        public ActionResult<string> Index()
        {
            var dict = new Dictionary<int, string>();
            foreach (StatusCodeEnum item in Enum.GetValues(typeof(StatusCodeEnum)))
            {
                dict.Add((int)item, item.ToString());
            }

            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }
    }
}
