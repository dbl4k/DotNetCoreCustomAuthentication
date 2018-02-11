using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreCustomAuthentication.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            // Extract some claims set during the authorization.
            string firstName = User.Claims.FirstOrDefault(n => n.Type.Equals("FirstName")).Value;
            string lastName = User.Claims.FirstOrDefault(n => n.Type.Equals("LastName")).Value;

            return new string[] { firstName, lastName };
        }
        
    }
}
