using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FHIR.Web.Client.Models;
using Hl7.Fhir.Model;
using Newtonsoft.Json;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json.Linq;
using FHIR.Web.Client.Helpers;

namespace FHIR.Web.Client.Controllers
{
    public class PatientController : Controller
    {
        private readonly HttpClient _client;

        public PatientController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7148/api/Patient/12896"); // Adjust base address as per your API endpoint
        }

        public async Task<IActionResult> Index()
        {
            List<PatientViewModel> patients = new List<PatientViewModel>();

            try
            {
                var response = await _client.GetAsync(_client.BaseAddress);
                var jsonContent = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(jsonContent);

                //jsonObject.Remove("active");

                JsonHelper.ModifyProperties(jsonObject);

                string modifiedJson = jsonObject.ToString();


                var fhirParser = new FhirJsonParser();
                var patient = fhirParser.Parse<Patient>(modifiedJson);

                patients.Add(new PatientViewModel
                {
                    Id = patient.Id,
                    BirthDate = patient.BirthDate,
                    FamilyName = patient.Name[0].Family,
                    GivenName = patient.Name.FirstOrDefault()?.Given.FirstOrDefault(),
                    Gender = patient.Gender?.ToString()
                });

                return View(patients);
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Exception: {ex.Message}");
                patients = new List<PatientViewModel>(); // or handle error case
            }

            return View(patients);
        }

        // Additional client-side controller actions as needed
    }
}