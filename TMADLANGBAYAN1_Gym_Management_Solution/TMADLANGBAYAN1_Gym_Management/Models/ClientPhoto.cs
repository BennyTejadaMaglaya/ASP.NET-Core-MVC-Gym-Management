using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
	public class ClientPhoto
	{
		public int ID { get; set; }

		[ScaffoldColumn(false)]
		public byte[]? Content { get; set; }

		[StringLength(255)]
		public string? MimeType { get; set; }

		public int ClientID { get; set; }
		public Client? Client { get; set; }
	}
}
