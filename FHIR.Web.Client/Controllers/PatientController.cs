using Microsoft.AspNetCore.Mvc;
using FHIR.Web.Client.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System.Linq;

namespace FHIR.Web.Client.Controllers
{
    public class PatientController : Controller
    {
        private readonly FhirClient _fhirClient;

        public PatientController()
        {
            _fhirClient = new FhirClient("https://fhirsandbox.healthit.gov/open/r4/fhir");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<PatientViewModel> patients = new List<PatientViewModel>();

            try
            {
                var searchParams = new SearchParams();
                var response = await _fhirClient.SearchAsync<Patient>(searchParams);

				patients = response.Entry
				    .Where(entry => entry?.Resource is Patient)
					.Select(entry => MapToPatientViewModel(entry.Resource as Patient))
					.ToList();

				return View(patients);
            }
            catch(FhirOperationException ex)
            {
                Console.WriteLine($"FHIR Operation Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return View(patients);
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePatients(string? family, string? given, string? dob )
        {
            List<PatientViewModel> patients = new List<PatientViewModel>();

            try
            {

                var searchParams = new SearchParams();

                if (!string.IsNullOrEmpty(family))
                {
                    searchParams.Add("family", family);
                }

                if (!string.IsNullOrEmpty(given))
                {
                    searchParams.Add("given", given);
                }

                if (!string.IsNullOrEmpty(dob))
                {
                    searchParams.Add("birthdate", dob);
                }

                var result = await _fhirClient.SearchAsync<Patient>(searchParams);

                if (result == null)
                {
					return PartialView("_PatientList", patients);
				}
                else
                {
                    patients = result.Entry
                    .Where(entry => entry.Resource is Patient)
                    .Select(entry => MapToPatientViewModel(entry?.Resource as Patient))
                    .ToList();

					return PartialView("_PatientList", patients);
				}
            }
            catch (FhirOperationException ex)
            {
                Console.WriteLine($"FHIR Operation Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return View(patients);
        }

		[HttpGet]
		public async Task<IActionResult> ViewMedicalHistory(string? id, string ? family, string ? given, string ? dob)
		{
			var medicalHistories = new MedicalInfo { };

			try
			{
				var searchParams = new SearchParams();
				searchParams.Add("patient", id);

				var result = await _fhirClient.SearchAsync<MedicationRequest>(searchParams);

                Console.WriteLine(result);

				var medicalList = result.Entry
					.Where(entry => entry.Resource is MedicationRequest)
					.Select(entry =>entry?.Resource)
					.ToList();

				foreach (var medicalRequest in medicalList)
				{
                    if(medicalRequest != null)
                    {
						var medicalInfo = MapToMedicalInfoViewModel(medicalRequest, family, given, dob);
						return PartialView("_MedicalInfo", medicalInfo);
					}
				}
			}
			catch (FhirOperationException ex)
			{
				Console.WriteLine($"FHIR Operation Exception: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex.Message}");
			}

			return PartialView("_MedicalInfo", medicalHistories);
		}

		private static PatientViewModel MapToPatientViewModel(Patient patient)
        {
            return new PatientViewModel
            {
                Id = patient.Id,
                FamilyName = patient.Name.FirstOrDefault()?.Family,
                GivenName = patient.Name.FirstOrDefault()?.Given.FirstOrDefault(),
                BirthDate = patient.BirthDate,
                Gender = patient.Gender?.ToString(),
            };
        }

        private static MedicalInfo MapToMedicalInfoViewModel(Resource medicationRequest, string family, string given, string dob)
        {
            return new MedicalInfo
            {
                Id = medicationRequest.Id, 
                Name = given + ' ' + family,
                BirthDate = dob,
            };
        }

	}
}