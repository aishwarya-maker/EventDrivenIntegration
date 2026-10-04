using Consumer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<EventResult> EventResults => Set<EventResult>();
}