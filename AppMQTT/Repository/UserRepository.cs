using AppMQTT.Models;
using AppMQTT.Repository;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AppMQTT.Repository
{
    public class UserRepository : IRepository<User>
    {
        private string connectionString;
        public UserRepository(IConfiguration configuration)
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
        public User FindLogin(string login, string password)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("select * from users where login=@Login and password = @Password", new { Login = login, Password = password }).FirstOrDefault();
            }
        }
        public User FindRegister(string login)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("select * from users where login=@Login", new { Login = login }).FirstOrDefault();
            }
        }
        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO users (login, password) VALUES(@Login, @Password)", item);
            }
        }

        public IEnumerable<User> FindAll()
        {
            throw new NotImplementedException();
        }


        public void Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
