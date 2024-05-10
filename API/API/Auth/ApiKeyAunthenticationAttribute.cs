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
using System.Diagnostics;

namespace API.Auth {
    public class ApiKeyAunthenticationAttribute : AuthorizeAttribute {
        
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken) {

            IEnumerable<string> requestHeaders;
            bool DoesKeyExist = actionContext.Request.Headers.TryGetValues("X-API-Key", out requestHeaders);

            // Check for API Key in request header.
            if (!DoesKeyExist)
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "API key not provided.");
                return;
            }

            string ApiKey = requestHeaders.FirstOrDefault();

            // Check if key is valid.
            if (!(new ApiKeyOperations().ValidateKey(ApiKey))) {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, "Key is invalid.");
                return;
            }

            // Check if key is active.
            if (!(new ApiKeyOperations().GetApiKeyStatus(ApiKey)))
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Forbidden, "Key is disabled.");
                return;
            }

            // Increment request count.
            if (!(new ApiKeyOperations().IncrementRequestCount(ApiKey))) {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, "Error while incrementing request count.");
                return;
            }

            string Method = actionContext.Request.Method.ToString();
            string Route = actionContext.Request.RequestUri.AbsolutePath;
           
            if (!new ApiLogOperations().AddApiRequest(ApiKey, Method, Route))
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, "Failed to log API request.");
                return;
            }

            await base.OnAuthorizationAsync(actionContext, cancellationToken);   
        }
    }
}