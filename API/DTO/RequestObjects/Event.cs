using System.ComponentModel.DataAnnotations;

namespace RequestObjects;

public class Event
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int Value { get; set; }
}