using Microsoft.EntityFrameworkCore;
using ILG.Domain.Models;

namespace ILG.Worker.Data;

public class AppDbContext : DbContext
{
    public DbSet<CargoOrder> Orders => Set<CargoOrder>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Cria um arquivo chamado logistics.db na raiz do Worker
        optionsBuilder.UseSqlite("Data Source=logistics.db");
    }
}