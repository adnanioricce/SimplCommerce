using System;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.WebHost;
using Microsoft.EntityFrameworkCore;

namespace SimplCommerce.IntegrationTests 
{
    public class TestBase : IDisposable
    {
        protected readonly string _dbName = "";
        protected readonly SimplDbContext _dbContext;
        public TestBase(CustomWebApplicationFactory<Startup> factory)
        {
            var scope = factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SimplDbContext>();            
            var connection = dbContext.Database.GetDbConnection();
            _dbName = connection.Database;
            string backupScript = $@"BACKUP DATABASE {_dbName} to DISK=N'{_dbName}.bak' WITH FORMAT, INIT, STATS=10;";
            dbContext.Database.ExecuteSqlRaw(backupScript);
            _dbContext = dbContext;
        }
        public virtual void Dispose()
        {
            _dbContext.Database.ExecuteSqlRaw($@"USE master;
                                                ALTER DATABASE {_dbName}
                                                SET SINGLE_USER
                                                --This rolls back all uncommitted transactions in the db.
                                                WITH ROLLBACK IMMEDIATE
                                                RESTORE DATABASE {_dbName} FROM DISK = '{_dbName}.bak' WITH REPLACE");
        }
    }
}
