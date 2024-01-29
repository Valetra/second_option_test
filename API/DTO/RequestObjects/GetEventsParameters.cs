using System.ComponentModel.DataAnnotations;

namespace RequestObjects;

public class GetEventsInRangeParameters
{
	public DateTime? From { get; set; }
	public DateTime? To { get; set; }
}