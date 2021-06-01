using Dapper;
using Microsoft.Extensions.Configuration;
using AppMQTT.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AppMQTT.Repository
{

    public class SignalsRepository : IRepository<Signals>
    {
        private string connectionString;
        public SignalsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }
        public IEnumerable<Signals> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Signals>("select * from signals");
            }
        }
        public IEnumerable<Signals> FindByData(DateTime date1, DateTime date2)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Signals>("SELECT * FROM signals WHERE time between @Date1 and @Date2", new { Date1 = date1, Date2 = date2 });
            }
        }
        public IEnumerable<Signals> FindByData(DateTime date1, DateTime date2, string name)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Signals>("SELECT * FROM signals WHERE time between @Date1 and @Date2 and name = @Name", new { Date1 = date1, Date2 = date2, Name = name });
            }
        }
        public void Add(Signals item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO signals (name, data, time, quality, edizm, type) VALUES(@Name, @Data, @Time, @Quality, @Edizm, @Type)", item);
            }
        }
        public void Remove(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM signals WHERE Id=@Id", new { Id = id });
            }
        }
    }
}