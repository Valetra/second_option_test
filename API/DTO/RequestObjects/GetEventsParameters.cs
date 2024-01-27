using System.ComponentModel.DataAnnotations;

namespace RequestObjects;

public class GetEventsInRangeParameters
{
	[Required]
	public DateTime From { get; set; }
	public DateTime? To { get; set; }
}