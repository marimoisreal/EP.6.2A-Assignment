using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EP._6._2A_Assignment.Data
{
    public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=SWD62A_2025ASSIGNMENT;Trusted_Connection=True;MultipleActiveResultSets=true"
            );
            return new IdentityContext(optionsBuilder.Options);
        }
    }
}
