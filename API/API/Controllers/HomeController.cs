using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Drawing.Text;
using Microsoft.SqlServer.Server;
using API.Models;
using System.Web.Security;

namespace API.Controllers {
    public class HomeController : Controller {
        public async Task<string> GetCountries() {
            HttpClient Client = new HttpClient();

            Client.DefaultRequestHeaders.Add("X-CSCAPI-KEY", "");
            string response = await Client.GetStringAsync("https://api.countrystatecity.in/v1/countries");

            return response;
        }

        public async Task<ActionResult> Index() {
            ViewBag.Title = "Home Page";
            ViewBag.TotalClients = new ClientOperations().GetTotalClients();
            ViewBag.TotalCountries = new ClientOperations().GetTotalDistinctCountries();
            ViewBag.TotalRequests = new ApiKeyOperations().GetTotalApiRequests();
            return View();
        }

        public ActionResult States(string countryName) {
            List<State> States;

            using (StreamReader reader = new StreamReader(@"C:\Users\Mihir\Desktop\git\dotnet-mvc\Ecommerce\API\API\states.json")) {
                string places = reader.ReadToEnd();
                States = JsonConvert.DeserializeObject<List<State>>(places);
            }

            List<SelectListItem> states = new List<SelectListItem>();

            foreach (State state in States) {
                if (state.Country_Name == countryName) {
                    System.Diagnostics.Debug.WriteLine(state.Name);
                    states.Add(
                        new SelectListItem { Text = state.Name, Value = state.Name }
                    );
                }
            }

            ViewBag.StateOptions = states;

            return View("StateDropdown");
        }

        public ActionResult Cities(string stateName) {
            List<City> Cities;

            using (StreamReader reader = new StreamReader(@"C:\Users\Mihir\Desktop\git\dotnet-mvc\Ecommerce\API\API\cities.json")) {
                string places = reader.ReadToEnd();
                Cities = JsonConvert.DeserializeObject<List<City>>(places);
            }

            List<SelectListItem> cities = new List<SelectListItem>();

            foreach (City city in Cities) {
                if (city.State_Name == stateName) {
                    System.Diagnostics.Debug.WriteLine(city.Name);
                    cities.Add(
                        new SelectListItem { Text = city.Name, Value = city.Name }
                    );
                }
            }

            ViewBag.CityOptions = cities;

            return View("CityDropdown");
        }

        public ActionResult Register() {
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

            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult RegisterClient(Client clientModel) {

            if (!ModelState.IsValid) {
                // If model state is not valid, return the view with validation errors
                System.Diagnostics.Debug.WriteLine(clientModel.Name);
                System.Diagnostics.Debug.WriteLine(clientModel.Email);
                System.Diagnostics.Debug.WriteLine(clientModel.Password);
                System.Diagnostics.Debug.WriteLine(clientModel.Address);
                System.Diagnostics.Debug.WriteLine(clientModel.Country);
                System.Diagnostics.Debug.WriteLine(clientModel.State);
                System.Diagnostics.Debug.WriteLine(clientModel.City);
                System.Diagnostics.Debug.WriteLine(clientModel.CountryCode);
                System.Diagnostics.Debug.WriteLine(clientModel.ContactNumber);

                ViewBag.ShowErrorMessage = true;
                return RedirectToAction("Register", "Home");
            }

            System.Diagnostics.Debug.WriteLine(clientModel.Name);
            System.Diagnostics.Debug.WriteLine(clientModel.Email);
            System.Diagnostics.Debug.WriteLine(clientModel.Password);
            System.Diagnostics.Debug.WriteLine(clientModel.Address);
            System.Diagnostics.Debug.WriteLine(clientModel.Country);
            System.Diagnostics.Debug.WriteLine(clientModel.State);
            System.Diagnostics.Debug.WriteLine(clientModel.City);
            System.Diagnostics.Debug.WriteLine(clientModel.CountryCode);
            System.Diagnostics.Debug.WriteLine(clientModel.ContactNumber);

            HttpResponseMessage DbResponse = new ClientOperations().AddClient(clientModel);

            if (DbResponse.IsSuccessStatusCode) {
                ViewBag.HasUserBeenRedirectedFromRegistration = true;
                return View("Login");
            } else {
                return RedirectToAction("Register", "Home");
            }
        }

        public ActionResult Login() {
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult AuthenticateCredentials(Client clientModel) {
            if ( new ClientOperations().AuthenticateCredentials(clientModel.Email, clientModel.Password) ) {
                FormsAuthentication.SetAuthCookie(clientModel.Email, true);
                
                HttpCookie cookie = new HttpCookie("EmailCookie");
                cookie["Email"] = clientModel.Email;
                cookie.HttpOnly = true;
                cookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cookie);

                return RedirectToAction("Index", "Dashboard");
            }
            else {
                ViewBag.InvalidCredentials = true;
                return View("Login");
            }
        }
    }
}