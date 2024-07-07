using Microsoft.AspNetCore.Mvc;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FHIR.Web.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PatientController : ControllerBase
	{
		private readonly FhirClient _fhirClient;

		public PatientController()
		{
            var settings = new FhirClientSettings
            {
                PreferredFormat = ResourceFormat.Json,
            };
            //test server
            _fhirClient = new FhirClient("https://fhirsandbox.healthit.gov/open/r4/fhir", settings);
		}

		// name: Sammy Lodewijk 
		[HttpGet]
		public async Task<ActionResult<Bundle>> UpdatePatientList(string? familyName, string? birthDate, string? givenName)
		{
			try
			{
				var searchParams = new SearchParams();

				if (!string.IsNullOrEmpty(familyName))
				{
					searchParams.Add("family", familyName);
				}

				if (!string.IsNullOrEmpty(givenName))
				{
					searchParams.Add("given", givenName);
				}

				if (!string.IsNullOrEmpty(birthDate))
				{
					searchParams.Add("birthdate", birthDate);
				}

				var result = await _fhirClient.SearchAsync<Patient>(searchParams);

				if (result == null)
				{
					return NotFound();
				}
				else
				{
					//var patients = result.Entry.Select(entry => (Patient)entry.Resource).ToList();

					return Ok(result);
				}
			}
			catch (FhirOperationException ex)
			{
				// Handle FHIR operation exceptions (e.g., server errors)
				Console.WriteLine($"FHIR Operation Exception: {ex.Message}");
				return StatusCode((int)ex.Status, ex.Message);
			}
			catch (Exception ex)
			{
				// Handle other exceptions
				Console.WriteLine($"Exception: {ex.Message}");
				return StatusCode(500, ex.Message);
			}
		}
		// test case: id = 12892
		[HttpGet("{id}")]
		public async Task<ActionResult<Patient>> GetPatient(string id)
		{
			try
			{
				var patient = await _fhirClient.ReadAsync<Patient>($"Patient/{id}");

				if (patient == null)
				{
					return NotFound();
				}
				else
				{
					return Ok(patient);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500);
			}
		}

		[HttpGet("medication/{id}")]
		public async Task<ActionResult<IEnumerable<MedicationAdministration>>> GetMedicationRequestsForPatientAsync(string id)
		{
			try
			{
				var searchParams = new SearchParams();
				searchParams.Add("patient", id);

				var result = await _fhirClient.SearchAsync<MedicationAdministration>(searchParams);

				if (result == null)
				{
					return NotFound();
				}
				else
				{
					var medicationRequests = result.Entry.Select(entry => (MedicationAdministration)entry.Resource).ToList();

					return Ok(medicationRequests);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500);
			}
		}
	}
}
