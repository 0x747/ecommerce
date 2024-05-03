using API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace API.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) {
                // User is not authenticated, redirect to the login page
                return RedirectToAction("Login", "Home");
            }

            HttpCookie cookie = Request.Cookies["EmailCookie"];
            string Email = cookie["Email"];

            IEnumerable<Client> ClientListModel = new ClientOperations().GetClientByEmail(Email);
            int ClientId = ClientListModel.First().Id;
            IEnumerable<ApiKey> ApiKeyModel = new ApiKeyOperations().GetApiKeyByClientId(ClientId);

            ViewBag.Client = ClientListModel;
            ViewBag.ApiKey = ApiKeyModel;
            // User is authenticated, allow access to the dashboard
            return View();
        }

        public ActionResult Settings() {
            if (!User.Identity.IsAuthenticated) {
                // User is not authenticated, redirect to the login page
                return RedirectToAction("Login", "Home");
            }

            List<Country> Countries;

            using (StreamReader reader = new StreamReader(@"C:\Users\Mihir\Desktop\git\dotnet-mvc\Ecommerce\API\API\countries.json")) {
                string places = reader.ReadToEnd();
                Countries = JsonConvert.DeserializeObject<List<Country>>(places);
            }

            List<SelectListItem> countries = new List<SelectListItem>();
            List<SelectListItem> countryCodes = new List<SelectListItem>();

            foreach (Country country in Countries) {
                countries.Add(
                    new SelectListItem { Text = country.Name, Value = country.Name }
                );

                countryCodes.Add(
                    new SelectListItem { Text = $"{country.Name} ({country.Phone_Code})", Value = country.Phone_Code }
                );
            }

            ViewBag.CountryOptions = countries;
            ViewBag.CountryCodeOptions = countryCodes;
            ViewBag.Countries = Countries;

            HttpCookie cookie = Request.Cookies["EmailCookie"];
            string Email = cookie["Email"];

            Client ClientModel = new ClientOperations().GetClientByEmail(Email).First();

            if (!string.IsNullOrEmpty(ClientModel.State)) {
                List<State> States;

                using (StreamReader reader = new StreamReader(@"C:\Users\Mihir\Desktop\git\dotnet-mvc\Ecommerce\API\API\states.json")) {
                    string places = reader.ReadToEnd();
                    States = JsonConvert.DeserializeObject<List<State>>(places);
                }

                List<SelectListItem> states = new List<SelectListItem>();

                foreach (State state in States) {
                    if (state.Country_Name == ClientModel.Country) {
                        System.Diagnostics.Debug.WriteLine(state.Name);
                        states.Add(
                            new SelectListItem { Text = state.Name, Value = state.Name }
                        );
                    }
                }

                ViewBag.StateOptions = states;
            }

            if (!string.IsNullOrEmpty(ClientModel.City)) {
                List<City> Cities;

                using (StreamReader reader = new StreamReader(@"C:\Users\Mihir\Desktop\git\dotnet-mvc\Ecommerce\API\API\cities.json")) {
                    string places = reader.ReadToEnd();
                    Cities = JsonConvert.DeserializeObject<List<City>>(places);
                }

                List<SelectListItem> cities = new List<SelectListItem>();

                foreach (City city in Cities) {
                    if (city.State_Name == ClientModel.State) {
                        System.Diagnostics.Debug.WriteLine(city.Name);
                        cities.Add(
                            new SelectListItem { Text = city.Name, Value = city.Name }
                        );
                    }
                }

                ViewBag.CityOptions = cities;
            }

            int ClientId = ClientModel.Id;
            IEnumerable<ApiKey> ApiKeyModel = new ApiKeyOperations().GetApiKeyByClientId(ClientId);

            ViewBag.ApiKey = ApiKeyModel;
            ClientModel.Password = "";
            return View(ClientModel);
        }

        public ActionResult GenerateApiKey(int ClientId) { 
            if (User.Identity.IsAuthenticated) {
                if (new ApiKeyOperations().CreateApiKey(ClientId).StatusCode == System.Net.HttpStatusCode.Created) {
                    return RedirectToAction("Index", "Dashboard");
                } else {
                    ViewBag.ApiKeyGenerationErrorMessage = "Could not create API Key";
                    return View("Index");
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Logout() {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Home");
        }
    }
}