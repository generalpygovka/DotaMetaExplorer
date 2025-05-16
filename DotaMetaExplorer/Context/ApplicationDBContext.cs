namespace DotaMetaExplorer.Context;
using DotaMetaExplorer.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }
    public DbSet<Subscribe> Subscribes { get; set; }
}
