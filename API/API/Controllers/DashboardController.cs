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
            IEnumerable<ApiLog> ApiLogs = new ApiLogOperations().GetApiLogsByClientEmail(Email);
           
            ViewBag.Client = ClientListModel;
            ViewBag.ApiKey = ApiKeyModel;
            ViewBag.ApiLogs = ApiLogs;
            // User is authenticated, allow access to the dashboard
            return View();
        }

        public ActionResult Settings() {
            if (!User.Identity.IsAuthenticated) {
                // User is not authenticated, redirect to the login page
                return RedirectToAction("Login", "Home");
            }

            List<Country> Countries;

            using (StreamReader reader = new StreamReader(@"")) {
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

                using (StreamReader reader = new StreamReader(@"")) {
                    string places = reader.ReadToEnd();
                    States = JsonConvert.DeserializeObject<List<State>>(places);
                }

                List<SelectListItem> states = new List<SelectListItem>();

                foreach (State state in States) {
                    if (state.Country_Name == ClientModel.Country) {
                        states.Add(
                            new SelectListItem { Text = state.Name, Value = state.Name }
                        );
                    }
                }

                ViewBag.StateOptions = states;
            }

            if (!string.IsNullOrEmpty(ClientModel.City)) {
                List<City> Cities;

                using (StreamReader reader = new StreamReader(@"")) {
                    string places = reader.ReadToEnd();
                    Cities = JsonConvert.DeserializeObject<List<City>>(places);
                }

                List<SelectListItem> cities = new List<SelectListItem>();

                foreach (City city in Cities) {
                    if (city.State_Name == ClientModel.State) {
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
            ViewBag.ShowSuccess = TempData["ShowSuccess"];
            ViewBag.ShowEmailSuccess = TempData["ShowEmailSuccess"];
            ViewBag.ShowPasswordSuccess = TempData["ShowPasswordSuccess"];
            ViewBag.ShowPasswordSame = TempData["ShowPasswordSame"];
            ViewBag.ShowKeyDisabled = TempData["ShowKeyDisabled"];
            ViewBag.ShowKeyEnabled = TempData["ShowKeyEnabled"];

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

        public ActionResult UpdatePersonalDetails(Client ClientModel)
        {
            if (new ClientOperations().UpdateClientPersonalDetails(ClientModel))
            {
                TempData["ShowSuccess"] = true;
            } else
            {
                TempData["ShowSuccess"] = false;
            }

            return RedirectToAction("Settings", "Dashboard");
        }

        public ActionResult UpdateClientEmail(Client ClientModel)
        {
            if (new ClientOperations().UpdateClientEmail(ClientModel))
            {
                TempData["ShowEmailSuccess"] = true;

                FormsAuthentication.SetAuthCookie(ClientModel.Email, true);

                HttpCookie cookie = new HttpCookie("EmailCookie"); // Create session cookie for new email.
                cookie["Email"] = ClientModel.Email;
                cookie.HttpOnly = true;
                cookie.Expires = DateTime.Now.AddDays(7);  // Cookie expires after 7 days.
                Response.Cookies.Add(cookie);

            } else
            {
                TempData["ShowEmailSuccess"] = false;
            }

            return RedirectToAction("Settings", "Dashboard");
        }

        public ActionResult UpdateClientPassword(Client ClientModel)
        {   
            if (ClientModel.Password == ClientModel.NewPassword)
            {
                TempData["ShowPasswordSame"] = true;
            }
            else if (new ClientOperations().UpdateClientPassword(ClientModel))
            {
                TempData["ShowPasswordSuccess"] = true;
            } 
            else
            {
                TempData["ShowPasswordSuccess"] = false;
            }

            return RedirectToAction("Settings", "Dashboard");
        }

        public ActionResult DisableApiKey(Client ClientModel)
        {
            if (new ApiKeyOperations().DisableApiKey(ClientModel))
            {
                TempData["ShowKeyDisabled"] = true;
            }
            else
            {
                TempData["ShowKeyDisabled"] = false;
            }

            return RedirectToAction("Settings", "Dashboard");
        }

        public ActionResult EnableApiKey(Client ClientModel)
        {
            if (new ApiKeyOperations().EnableApiKey(ClientModel))
            {
                TempData["ShowKeyEnabled"] = true;
            }
            else
            {
                TempData["ShowKeyEnabled"] = false;
            }

            return RedirectToAction("Settings", "Dashboard");
        }
    }
}