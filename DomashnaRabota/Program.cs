using System;
using Microsoft.Data.SqlClient;
class Program
{
    static void Main()
    {
        string connectionString = "Server=DESKTOP-7GCFJNV\\SQLEXPRESS;Integrated Security=true;TrustServerCertificate=true";
        string databaseName = "Homework";

        using (SqlConnection connection = new(connectionString))
        {
            connection.Open();

            // Check if database exists
            string checkDbQuery = $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}') CREATE DATABASE [{databaseName}]";
            using (SqlCommand command = new SqlCommand(checkDbQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            // Switch to the new database
            connection.ChangeDatabase(databaseName);

            // Create tables
            string createTablesQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
                CREATE TABLE Products (
                    Id INT PRIMARY KEY IDENTITY,
                    Name NVARCHAR(100) NOT NULL
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Buyers' AND xtype='U')
                CREATE TABLE Buyers (
                    Id INT PRIMARY KEY IDENTITY,
                    Name NVARCHAR(100) NOT NULL,
                    ProductId INT,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                );";

            using (SqlCommand command = new SqlCommand(createTablesQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        //Metod koito da insertva edin produkt
        void insertProduct()
        {
            Console.WriteLine("Enter product name: ");
            string productName = Console.ReadLine();
            if (string.IsNullOrEmpty(productName))
            {
                Console.WriteLine("Product name cannot be empty");
                return;
            }
            string insertProductQuery = $"INSERT INTO Products (Name) VALUES ('{productName}')";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using (SqlCommand command = new SqlCommand(insertProductQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Product {productName} inserted into {databaseName}!");
                }
            }
        }
        //Metod koito da insertva edin kupuvach
        void insertBuyer()
        {
            Console.WriteLine("Enter the name of the buyer: ");
            string buyerName = Console.ReadLine();
            if (string.IsNullOrEmpty(buyerName))
            {
                Console.WriteLine("Buyer name cannot be empty");
                return;
            }
            string insertBuyerQuery = $"INSERT INTO Buyer (Name) VALUES ('{buyerName}')";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using (SqlCommand command = new SqlCommand(insertBuyerQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Buyer {buyerName} inserted into {databaseName}!");
                }

                
            }
        }
      


        insertProduct();
        insertBuyer();
    }
}


