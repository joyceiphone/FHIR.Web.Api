using System.ComponentModel.DataAnnotations;

namespace FHIR.Web.Client.Models
{
	public class PatientViewModel
	{
		public string Id { get; set; }
		public string ?FamilyName { get; set; }
		public string ?GivenName { get; set; }
		public string ?BirthDate { get; set; }
		public string ?Gender { get; set; }
	}
}
