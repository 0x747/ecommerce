using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using API.Models;

namespace API.Controllers {
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ClientController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage RegisterClient(Client clientModel) {
            return new ClientOperations().AddClient(clientModel);
        }
    }
}
