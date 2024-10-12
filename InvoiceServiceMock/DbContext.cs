using Microsoft.EntityFrameworkCore;

public class InvoiceDb : DbContext
{
    public DbSet<Invoice> Invoices { get; set; }

    public string DbPath { get; }

    public InvoiceDb(DbContextOptions<InvoiceDb> options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "invoice.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>().Navigation(e => e.LineItems).AutoInclude();
    }
}
