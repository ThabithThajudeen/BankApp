using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Assignment02.Models
{
    public class DBManager
    {
        private static string connectionString = "Data Source=mydatabase.db;Version=3;";

        public static bool CreateTables()
        {
            try
            {
                if (CreateBankTable() && CreateTransactionTable() && CreateUserProfileTable())
                {
                    Console.WriteLine("Tables created successfully.");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }


        private static bool CreateBankTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS BankTable (
                                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                AccountNumber TEXT NOT NULL,
                                AccountHolderName TEXT,
                                Balance REAL,
                                Email TEXT,
                                Address TEXT,
                                Phone TEXT
                            )";

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                Console.WriteLine("Bank table created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        private static bool CreateTransactionTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS TransactionTable (
                                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                AccountNumber TEXT NOT NULL,
                                Amount DECIMAL(18, 2) NOT NULL,
                                -- Add other transaction-related columns here
                                FOREIGN KEY (AccountNumber) REFERENCES BankTable(AccountNumber)
                            )";

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                Console.WriteLine("Transaction table created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool Insert(Bank bank)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Check if account number already exists
                    using (SQLiteCommand checkCommand = connection.CreateCommand())
                    {
                        checkCommand.CommandText = "SELECT COUNT(*) FROM BankTable WHERE AccountNumber = @AccountNumber";
                        checkCommand.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);

                        long count = (long)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            // Account number already exists in the database
                            connection.Close();
                            return false;
                        }
                    }

                    // If account number doesn't exist, proceed to insert the new record
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    INSERT INTO BankTable (AccountNumber, AccountHolderName, Balance, Email, Address, Phone)
                    VALUES (@AccountNumber, @AccountHolderName, @Balance, @Email, @Address, @Phone)";

                        command.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);
                        command.Parameters.AddWithValue("@AccountHolderName", bank.AccountHolderName);
                        command.Parameters.AddWithValue("@Balance", bank.Balance);
                        command.Parameters.AddWithValue("@Email", bank.Email);
                        command.Parameters.AddWithValue("@Address", bank.Address);
                        command.Parameters.AddWithValue("@Phone", bank.Phone);

                        int rowsInserted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return false;
        }

        public static bool CreateUserProfileTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS UserProfileTable (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT,
                        Address TEXT,
                        Phone TEXT
                    )";

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                Console.WriteLine("User Profile table created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool Insert(Transaction transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO TransactionTable (AccountNumber, Amount)
                            VALUES (@AccountNumber, @Amount)";

                        command.Parameters.AddWithValue("@AccountNumber", transaction.AccountNumber);
                        command.Parameters.AddWithValue("@Amount", transaction.Amount);

                        int rowsInserted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return false;
        }

        public static Bank GetById(string accountNumber)
        {
            Bank bank = null;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM BankTable WHERE AccountNumber = @AccountNumber";
                        command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bank = new Bank();
                                bank.Id = Convert.ToInt32(reader["ID"]);
                                bank.AccountNumber = reader["AccountNumber"].ToString();
                                bank.AccountHolderName = reader["AccountHolderName"].ToString();
                                bank.Balance = Convert.ToDouble(reader["Balance"]);
                                bank.Email = reader["Email"].ToString();
                                bank.Address = reader["Address"].ToString();
                                bank.Phone = reader["Phone"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return bank;
        }


        public static bool DeleteBank(string accountNumber)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM BankTable WHERE AccountNumber = @AccountNumber";
                        command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                        int rowsDeleted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsDeleted > 0)
                        {
                            return true; // Deletion was successful
                        }
                    }
                    connection.Close();
                }

                return false; // No rows were deleted
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Deletion failed
            }
        }

        public static bool UpdateBank(Bank bank)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    UPDATE BankTable 
                    SET AccountHolderName = @AccountHolderName, Balance = @Balance 
                    WHERE AccountNumber = @AccountNumber";
                        command.Parameters.AddWithValue("@AccountHolderName", bank.AccountHolderName);
                        command.Parameters.AddWithValue("@Balance", bank.Balance);
                        command.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsUpdated > 0)
                        {
                            return true; // Update was successful
                        }
                    }
                    connection.Close();
                }

                return false; // No rows were updated
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Update failed
            }
        }

        public static List<Bank> GetAllBanks()
        {
            List<Bank> bankList = new List<Bank>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM BankTable";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Bank bank = new Bank();
                                bank.Id = Convert.ToInt32(reader["ID"]);
                                bank.AccountNumber = reader["AccountNumber"].ToString();
                                bank.AccountHolderName = reader["AccountHolderName"].ToString();
                                bank.Balance = Convert.ToDouble(reader["Balance"]);
                                bank.Email = reader["Email"].ToString();
                                bank.Address = reader["Address"].ToString();
                                bank.Phone = reader["Phone"].ToString();

                                bankList.Add(bank);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return bankList;
        }

        public static Bank GetBankByAccountNumber(string accountNumber)
        {
            Bank bank = null;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM BankTable WHERE AccountNumber = @AccountNumber";
                        command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bank = new Bank();
                                bank.Id = Convert.ToInt32(reader["ID"]);
                                bank.AccountNumber = reader["AccountNumber"].ToString();
                                bank.AccountHolderName = reader["AccountHolderName"].ToString();
                                bank.Balance = Convert.ToDouble(reader["Balance"]);
                                bank.Email = reader["Email"].ToString();
                                bank.Address = reader["Address"].ToString();
                                bank.Phone = reader["Phone"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return bank;
        }

        public static bool DeleteTransaction(int transactionId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM TransactionTable WHERE ID = @TransactionId";
                        command.Parameters.AddWithValue("@TransactionId", transactionId);

                        int rowsDeleted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsDeleted > 0)
                        {
                            return true; // Deletion was successful
                        }
                    }
                    connection.Close();
                }

                return false; // No rows were deleted
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Deletion failed
            }
        }

        public static bool UpdateTransaction(Transaction transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    UPDATE TransactionTable 
                    SET AccountNumber = @AccountNumber, Amount = @Amount 
                    WHERE ID = @TransactionId";
                        command.Parameters.AddWithValue("@AccountNumber", transaction.AccountNumber);
                        command.Parameters.AddWithValue("@Amount", transaction.Amount);
                        command.Parameters.AddWithValue("@TransactionId", transaction.Id);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsUpdated > 0)
                        {
                            return true; // Update was successful
                        }
                    }
                    connection.Close();
                }

                return false; // No rows were updated
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Update failed
            }
        }

        public static bool AddTransaction(Transaction transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                INSERT INTO TransactionTable (AccountNumber, Amount) 
                VALUES (@AccountNumber, @Amount)";
                        command.Parameters.AddWithValue("@AccountNumber", transaction.AccountNumber);
                        command.Parameters.AddWithValue("@Amount", transaction.Amount);

                        int rowsInserted = command.ExecuteNonQuery();
                        connection.Close();

                        return rowsInserted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }



        public static List<Transaction> GetAllTransactions()
        {
            List<Transaction> transactionList = new List<Transaction>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM TransactionTable";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transaction transaction = new Transaction();
                                transaction.Id = Convert.ToInt32(reader["ID"]);
                                transaction.AccountNumber = reader["AccountNumber"].ToString();
                                transaction.Amount = Convert.ToDecimal(reader["Amount"]);

                                // Populate other transaction-related properties if needed

                                transactionList.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return transactionList;
        }

        public static List<Transaction> GetTransactionsById(int transactionId)
        {
            List<Transaction> transactionList = new List<Transaction>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM TransactionTable WHERE ID = @TransactionId";
                        command.Parameters.AddWithValue("@TransactionId", transactionId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transaction transaction = new Transaction();
                                transaction.Id = Convert.ToInt32(reader["ID"]);
                                transaction.AccountNumber = reader["AccountNumber"].ToString();
                                transaction.Amount = Convert.ToDecimal(reader["Amount"]);

                                transactionList.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Consider logging the error, not just printing it.
            }

            return transactionList;
        }

        public static List<Transaction> GetTransactionsByAccountNumber(string accountNumber)
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM TransactionTable WHERE AccountNumber = @AccountNumber";
                        command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transaction transaction = new Transaction();
                                transaction.Id = Convert.ToInt32(reader["ID"]);
                                transaction.AccountNumber = reader["AccountNumber"].ToString();
                                transaction.Amount = Convert.ToDecimal(reader["Amount"]);
                                transactions.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Consider logging the error, not just printing it.
            }

            return transactions;
        }

        public static bool Insert(UserProfile userProfile)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    INSERT INTO UserProfileTable (Name, Email, Address, Phone)
                    VALUES (@Name, @Email, @Address, @Phone)";

                        command.Parameters.AddWithValue("@Name", userProfile.Name);
                        command.Parameters.AddWithValue("@Email", userProfile.Email);
                        command.Parameters.AddWithValue("@Address", userProfile.Address);
                        command.Parameters.AddWithValue("@Phone", userProfile.Phone);

                        int rowsInserted = command.ExecuteNonQuery();

                        connection.Close();
                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return false;
        }

        public static UserProfile GetById(int id)
        {
            UserProfile userProfile = null;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM UserProfileTable WHERE ID = @ID";
                        command.Parameters.AddWithValue("@ID", id);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userProfile = new UserProfile();
                                userProfile.Id = Convert.ToInt32(reader["ID"]);
                                userProfile.Name = reader["Name"].ToString();
                                userProfile.Email = reader["Email"].ToString();
                                userProfile.Address = reader["Address"].ToString();
                                userProfile.Phone = reader["Phone"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return userProfile;
        }
        public static bool UpdateUserProfile(UserProfile userProfile)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    UPDATE UserProfileTable
                    SET Name = @Name, Email = @Email, Address = @Address, Phone = @Phone
                    WHERE ID = @ID";
                        command.Parameters.AddWithValue("@Name", userProfile.Name);
                        command.Parameters.AddWithValue("@Email", userProfile.Email);
                        command.Parameters.AddWithValue("@Address", userProfile.Address);
                        command.Parameters.AddWithValue("@Phone", userProfile.Phone);
                        command.Parameters.AddWithValue("@ID", userProfile.Id);

                        int rowsUpdated = command.ExecuteNonQuery();

                        connection.Close();

                        if (rowsUpdated > 0)
                        {
                            return true; // Update was successful
                        }
                    }
                    connection.Close();
                }

                return false; // No rows were updated
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Update failed
            }
        }

        public static bool DeleteUserProfile(int id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM UserProfileTable WHERE ID = @ID";
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsDeleted = command.ExecuteNonQuery();

                        connection.Close();

                        if (rowsDeleted > 0)
                        {
                            return true; // Deletion was successful
                        }
                    }
                    connection.Close();
                }

                return false; // No rows were deleted
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Deletion failed
            }
        }

        public static List<UserProfile> GetAllUserProfiles()
        {
            List<UserProfile> userProfiles = new List<UserProfile>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM UserProfileTable"; // Assuming your table name is UserProfileTable

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserProfile userProfile = new UserProfile();
                                userProfile.Id = Convert.ToInt32(reader["Id"]);
                                userProfile.Name = reader["Name"].ToString();
                                userProfile.Email = reader["Email"].ToString();
                                userProfile.Address = reader["Address"].ToString();
                                userProfile.Phone = reader["Phone"].ToString();

                                userProfiles.Add(userProfile);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return userProfiles;
        }

        // Additional methods for transaction-related operations can be added here

        public static void ResetDatabase()
        {
            // Drop the tables
            DropTable("TransactionTable");
            DropTable("UserProfileTable");
            DropTable("BankTable");

            // Reinitialize the tables and seed data
            DBInitialize();
        }

        private static void DropTable(string tableName)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"DROP TABLE IF EXISTS {tableName};";
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dropping {tableName}: " + ex.Message);
            }
        }


        public static void DBInitialize()
        {
            if (CreateTables())
            {
                Bank bank1 = new Bank
                {
                    AccountNumber = "1234567890",
                    AccountHolderName = "Sajib",
                    Balance = 1000.0,
                    Email = "sajib@example.com",
                    Address = "123 Main St",
                    Phone = "123-456-7890"
                };

                Insert(bank1);

                Bank bank2 = new Bank
                {
                    AccountNumber = "9876543210",
                    AccountHolderName = "Mistry",
                    Balance = 2000.0,
                    Email = "mistry@example.com",
                    Address = "456 Elm St",
                    Phone = "987-654-3210"
                };

                Insert(bank2);

                Bank bank3 = new Bank
                {
                    AccountNumber = "5678901234",
                    AccountHolderName = "Mike",
                    Balance = 1500.0,
                    Email = "mike@example.com",
                    Address = "789 Oak St",
                    Phone = "567-890-1234"
                };

                Insert(bank3);

                Transaction transaction1 = new Transaction
                {
                    AccountNumber = "1234567890",
                    Amount = 500
                };

                Insert(transaction1);

                Transaction transaction2 = new Transaction
                {
                    AccountNumber = "9876543210",
                    Amount = 200
                };

                Insert(transaction2);

                // Similarly, insert other transactions if needed
            }
        }
    }
}
