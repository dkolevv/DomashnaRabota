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

        // Method to insert a product
        void InsertProduct()
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

        // Method to insert a buyer
        void InsertBuyer()
        {
            Console.WriteLine("Enter the name of the buyer: ");
            string buyerName = Console.ReadLine();
            if (string.IsNullOrEmpty(buyerName))
            {
                Console.WriteLine("Buyer name cannot be empty");
                return;
            }
            

            string insertBuyerQuery = $"INSERT INTO Buyers (Name) VALUES ('{buyerName}')";
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

        // Method to let a buyer buy a product
        void BuyProduct(string buyerName, string productName)
        {
            string getProductIdQuery = $"SELECT Id FROM Products WHERE Name = '{productName}'";
            int productId = -1;

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using (SqlCommand command = new SqlCommand(getProductIdQuery, connection))
                {
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        productId = (int)result;
                    }
                    else
                    {
                        Console.WriteLine($"Product {productName} does not exist.");
                        return;
                    }
                }
            }

            string insertBuyerQuery = $"INSERT Buyers (Name, ProductId) VALUES ('{buyerName}', {productId})";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using (SqlCommand command = new SqlCommand(insertBuyerQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Buyer {buyerName} bought product {productName}!");
                }
            }
        }

        // Method to query all products and buyer count
        void QueryProductsAndBuyerCount()
        {
            string query = @"
                SELECT p.Id,p.Name, COUNT(b.Id) AS BuyerCount
                FROM Products p
                LEFT JOIN Buyers b ON p.Id = b.ProductId
                GROUP BY p.Name,p.Id";

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int productId = (int)reader["Id"];
                            string productName = reader["Name"].ToString();
                            int buyerCount = (int)reader["BuyerCount"];
                            Console.WriteLine($"Product: {productName} ID: {productId} , Buyer Count: {buyerCount}");
                        }
                    }
                }
            }
        }

        void deleteProductById(int id)
        {
            string deleteProductQuery = $"DELETE FROM Products WHERE Id = {id}";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using (SqlCommand command = new SqlCommand(deleteProductQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Product with id {id} deleted from {databaseName}!");
                }
            }
        }

        void UpdateBuyerName(int id, string newBuyerName)
        {

            Console.WriteLine($"Buyer with id {id} is:");

            string selectBuyerQuery = $"SELECT TOP 1 Name FROM Buyers WHERE Id = {id}";
            string updateBuyerQuery = $"UPDATE Buyers SET Name = '{newBuyerName}' WHERE Id = {id}";

            using SqlConnection connection = new(connectionString);
            {
                connection.Open();
                connection.ChangeDatabase(databaseName);
                using SqlCommand command = new(selectBuyerQuery, connection);
                {
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine($"Buyer with id {id} does not exist.");
                        return;
                    }
                }
            }
        }
            
            

            
            //InsertProduct();
            //InsertBuyer();
        //    Console.WriteLine("Enter a buyer name that would buy a product.");
        //string buyerName = Console.ReadLine();
        //Console.WriteLine("Now enter a product name that would be bought.");
        //string productName = Console.ReadLine();
        //BuyProduct(buyerName, productName);
        //QueryProductsAndBuyerCount();

        //Console.WriteLine("Enter a product ID");
        //int productIdToDelete = int.Parse(Console.ReadLine());
        //deleteProductById(productIdToDelete);
        //QueryProductsAndBuyerCount();
    }
}
