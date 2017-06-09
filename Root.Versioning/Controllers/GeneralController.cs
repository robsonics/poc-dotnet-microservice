using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Root.Versioning.Controllers
{
    [Route("api/[controller]")]
    public class GeneralController : Controller
    {
        [HttpGet]
        public string Version() {
            return "1.0.0";
        }

    }
}
