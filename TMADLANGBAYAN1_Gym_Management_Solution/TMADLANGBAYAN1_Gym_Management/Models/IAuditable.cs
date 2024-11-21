namespace TMADLANGBAYAN1_Gym_Management.Models
{
	internal interface IAuditable
	{
		string? CreatedBy { get; set; }
		DateTime? CreatedOn { get; set; }
		string? UpdatedBy { get; set; }
		DateTime? UpdatedOn { get; set; }
	}
}
