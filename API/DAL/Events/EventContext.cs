using Microsoft.EntityFrameworkCore;

using DAL.Models;

namespace DAL.Contexts;

public class EventContext(DbContextOptions<EventContext> options) : DbContext(options)
{
    public DbSet<Event> Events { get; set; }
}