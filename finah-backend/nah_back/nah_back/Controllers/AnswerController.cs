using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace nah_back.Controllers
{
    public class AnswerController : ApiController
    {
        // GET: api/Answer
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Answer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Answer
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Answer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Answer/5
        public void Delete(int id)
        {
        }
    }
}
