using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index(nameof(CreateDateTime))]
public class Event : BaseModel<Guid>
{
    public string Name { get; set; } = "";
    public int Value { get; set; } = 0;
    public DateTime CreateDateTime { get; set; } = DateTime.UtcNow;
}
