using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using API.Models;

namespace API.Controllers
{
    [RoutePrefix("api")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ApiKeyController : ApiController
    {
        [HttpPost, Route("get-key/{clientId:int}")]
        public HttpResponseMessage GetApiKey(int ClientId) {

            if (User.Identity.IsAuthenticated) {
                return new ApiKeyOperations().CreateApiKey(ClientId);
            } else {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);    
            }
        }
    }
}
