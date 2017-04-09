using System.Data.Entity;
using Learning.Calculator.Models;
using Learning.Calculator.Models.Token;

namespace Learning.Calculator.EntityWrapper
{
    public class CalculatorDbContext : DbContext
    {
        public CalculatorDbContext() : 
            base(@"server=DESKTOP-4496VDM\ARTSYLPRODUCTS10;uid=sa;pwd=Artsyl0154dA;database=CalculatorDb2")
        {
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new CreateDatabaseIfNotExists<CalculatorDbContext>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<CalculatorDbContext, Configuration>());
        }

        public DbSet<CalculatorModel> CalculatorModels { get; set; }
        public DbSet<Token> Tokens { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CalculatorModel.CalculatorModelConfiguration());
            modelBuilder.Configurations.Add(new NumberToken.NumberTokenConfiguration());
            modelBuilder.Configurations.Add(new OperationToken.OperationTokenConfiguration());
            modelBuilder.Configurations.Add(new Token.TokenConfiguration());
        }
    }
}
