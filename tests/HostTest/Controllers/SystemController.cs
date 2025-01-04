using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nwpie.HostTest.Controllers
{
    [Route("[controller]")]
    public class SystemController : Controller
    {
        public SystemController()
        {

        }

        [Route("Start")]
        public async Task<IActionResult> Start()
        {
            Console.WriteLine("Start Service");
            await Task.CompletedTask;
            return Ok();
        }

        [Route("Stop")]
        public async Task<IActionResult> Stop()
        {
            Console.WriteLine("Stop Service");
            await Task.CompletedTask;
            return Ok();
        }
    }
}
