using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Auth;
using System.Web.Http.Description;

namespace API.Controllers {

    /// <summary>
    /// API to interact with vendor related data.
    /// </summary>
    [RoutePrefix("api")]
    [AllowAnonymous]
    public class VendorController : ApiController {

        /// <summary>
        /// Sends a vendor model as a x-www-form-urlencoded to register a new vendor.
        /// </summary>
        /// <param name="VendorModel">An object including valid Name, Email, Password, CountryCode, ContactNumber, Address, Country, State, and City.</param>
        /// <returns>An HttpResponseMessage with status code.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost, Route("register")]
        public HttpResponseMessage RegisterVendor(Vendor VendorModel) {
            if (ModelState.IsValid) {
                return new VendorOperations().AddVendor(VendorModel);
            }
            else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

        }

        /// <summary>
        /// Returns a list of all vendors.
        /// </summary>
        /// <returns>A List of vendors.</returns>
        [HttpGet, Route("vendors")]
        [ApiKeyAunthentication]
        public IEnumerable<Vendor> GetAllVendors() {
            return new VendorOperations().GetAllVendors();
        }

        /// <summary>
        /// Returns a list of vendors with the matching name. 
        /// </summary>
        /// <param name="vendorName">Name of the vendor</param>
        /// <param name="IsExactMatch">Used to specify an exact match</param>
        /// <returns>A List of vendors with the matching name.</returns>
        [HttpGet, Route("vendors/{vendorName}/{IsExactMatch:bool?}")]
        public IEnumerable<Vendor> GetVendorByName(string vendorName, bool IsExactMatch = false) {
            return new VendorOperations().GetVendorByName(vendorName, IsExactMatch);
        }
    }
}

