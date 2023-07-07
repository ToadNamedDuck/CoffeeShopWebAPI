using CoffeeShop.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CoffeeShop.Repositories
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly string _connectionString;
        public CoffeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Coffee> GetAll()
        {
            List<Coffee> coffees = new();

            using (var connection = Connection)
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"Select Id, Title, BeanVarietyId from Coffee";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coffees.Add(coffeeBuilder(reader));
                        }
                    }
                }
            }
            return coffees;
        }

        public Coffee Get(int id)
        {
            Coffee coffee = null;
            using (var connection = Connection)
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"Select Id, Title, BeanVarietyId from Coffee
                                            where Id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            coffee = coffeeBuilder(reader);
                        }
                    }
                }
            }
            return coffee;
        }
        public void Add(Coffee coffee)
        {
            if (coffee != null)
            {
                using (var connection = Connection)
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"Insert into Coffee (Title, BeanVarietyId)
                                                OUTPUT INSERTED.ID
                                                Values (@title, @beanId)";
                        command.Parameters.AddWithValue("@title", coffee.Title);
                        command.Parameters.AddWithValue("@beanId", coffee.BeanVarietyId);

                        coffee.Id = (int)command.ExecuteScalar();
                    }
                }
            }
        }

        public void Update(Coffee coffee)
        {
            if (coffee != null)
            {
                using (var connection = Connection)
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"Update Coffee
                                                Set Title = @title,
                                                BeanVarietyId = @beanId
                                                Where Id = @id";

                        command.Parameters.AddWithValue("@title", coffee.Title);
                        command.Parameters.AddWithValue("@beanId", coffee.BeanVarietyId);
                        command.Parameters.AddWithValue("@id", coffee.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"Delete from Coffee where Id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
            }
        }
        private Coffee coffeeBuilder(SqlDataReader reader)
        {
            Coffee coffee = new Coffee();
            coffee.Title = reader.GetString(reader.GetOrdinal("Title"));
            coffee.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            coffee.BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId"));

            return coffee;
        }
    }
}
