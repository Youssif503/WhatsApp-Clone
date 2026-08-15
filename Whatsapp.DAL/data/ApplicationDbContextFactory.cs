using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SQLitePCL;

namespace Whatsapp.DAL.data;

public class ApplicationDbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        Batteries.Init();
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite("Data Source=/home/youssifsayed/RiderProjects/Whatsapp.src/Whatsapp.DAL/chat.db");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}