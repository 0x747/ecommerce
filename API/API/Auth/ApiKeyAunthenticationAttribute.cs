using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using API.Models;

namespace API.Auth {
    public class ApiKeyAunthenticationAttribute : AuthorizeAttribute {
        
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken) {

            IEnumerable<string> requestHeaders;
            bool DoesKeyExist = actionContext.Request.Headers.TryGetValues("X-API-Key", out requestHeaders);

            if (!DoesKeyExist)
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "API key not provided.");
                return;
            }

            string ApiKey = requestHeaders.FirstOrDefault();

            if (!(new ApiKeyOperations().ValidateKey(ApiKey))) {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, "Invalid API key.");
                return;
            }

            await base.OnAuthorizationAsync(actionContext, cancellationToken);   
        }
    }
}