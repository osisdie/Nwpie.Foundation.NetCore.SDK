using System;
using Microsoft.AspNetCore.Mvc;

namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Controllers
{
    public class HomeController : ControllerBase
    {
        // GET /
        [HttpGet]
        public string Index()
        {
            return DateTime.UtcNow.ToString("s");
        }
    }
}
