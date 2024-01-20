using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private const string BASE_URL = "http://localhost:18063/api";

        public MainWindow()
        {
            InitializeComponent();
           // SetEventHandlers();
        }

       
        //Account Methods!
        private void CreateAccount()
        {
            ManageAccount(Method.Post);
        }

        private void UpdateAccount()
        {
            ManageAccount(Method.Put);
        }

        private void DeleteAccount()
        {
            var client = new RestClient($"{BASE_URL}/BankAccount");
            var request = new RestRequest($"/{AccountNumberTextBox.Text}", Method.Delete);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                MessageBox.Show("Account deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Console.WriteLine(response.Content);
                MessageBox.Show($"Failed to delete account: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ManageAccount(Method method)
        {
            var client = new RestClient($"{BASE_URL}/BankAccount");
            var request = new RestRequest("", method);
            var account = new
            {
                AccountNumber = AccountNumberTextBox.Text,
                AccountHolderName = AccountHolderNameTextBox.Text,
                Balance = decimal.Parse(BalanceTextBox.Text),
                Email = AccountEmailTextBox.Text,
                Address = AccountAddressTextBox.Text,
                Phone = AccountPhoneTextBox.Text
            };
            request.AddJsonBody(account);
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.IsSuccessful)
            {
                MessageBox.Show("Action successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Console.WriteLine(response.Content);
                MessageBox.Show($"Action failed with error: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void GetAccountDetails()
        {
            Console.WriteLine("I am in GetAccount!!");
            var client = new RestClient($"{BASE_URL}/BankAccount/{AccountNumberTextBox.Text}");
            var request = new RestRequest("", Method.Get);
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var account = JsonConvert.DeserializeObject<dynamic>(response.Content);
                Console.WriteLine(account);
                AccountNumberTextBox.Text = account.accountNumber;
                AccountHolderNameTextBox.Text = account.accountHolderName;
                BalanceTextBox.Text = account.balance.ToString();
                AccountEmailTextBox.Text = account.email;
                AccountAddressTextBox.Text = account.address;
                AccountPhoneTextBox.Text = account.phone;
                AccountIdTextBox.Text = account.id;
            }
            else
            {
                MessageBox.Show($"Failed to fetch account details: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount();
        }

        private void UpdateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccount();
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteAccount();
        }

        private void SearchAccountButton_Click(object sender, RoutedEventArgs e)
        {
            GetAccountDetails();
        }







        //Transaction Methods
        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTransaction("deposit");
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTransaction("withdraw");
        }

        private void GetTransactionHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            GetTransactionHistory();
        }

        private void GetTransactionHistory()
        {
       

            // Assuming you have a TextBox named TransactionAccountNumberTextBox to input the account number for which you want the transaction history
            string accountNumber = TransactionAccountNumberTextBox.Text;

            var client = new RestClient($"{BASE_URL}/BankAccount/transactions/account/{accountNumber}");
            var request = new RestRequest("", Method.Get);
            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                TransactionHistoryList.ItemsSource = transactions;
            }
            else
            {
                MessageBox.Show($"Failed to fetch transaction history: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Example of a Transaction class, adjust the properties as per actual API response
        public class Transaction
        {
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public decimal Amount { get; set; }
        }





        private void ExecuteTransaction(string action)
        {
            try
            {
                // Ensure valid inputs
                if (string.IsNullOrWhiteSpace(TransactionAccountNumberTextBox.Text) || string.IsNullOrWhiteSpace(TransactionAmountTextBox.Text))
                {
                    MessageBox.Show("Please provide both Account Number and Amount.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Parse amount
                if (!decimal.TryParse(TransactionAmountTextBox.Text, out decimal amount))
                {
                    MessageBox.Show("Please enter a valid amount.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var client = new RestClient($"{BASE_URL}/BankAccount/{action}");
                var request = new RestRequest("",Method.Post);
                var transaction = new
                {
                    AccountNumber = TransactionAccountNumberTextBox.Text,
                    Amount = amount
                };

                request.AddJsonBody(transaction);
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    MessageBox.Show($"{action.ToUpper()} successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Action failed with error: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //User Profile Methods

        private void CreateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileOperation("create");
        }

        private void UpdateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileOperation("update");
        }

        private void SearchProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileOperation("search");
        }

        private void DeleteProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileOperation("delete");
        }

        private void UserProfileOperation(string action)
        {
            try
            {
                var client = new RestClient($"{BASE_URL}/UserProfile");
                RestResponse response;

                RestRequest request;
                switch (action)
                {
                    case "create":
                        request = new RestRequest("", Method.Post);
                        request.AddJsonBody(new
                        {
                            Name = UserNameTextBox.Text,
                            Email = UserEmailTextBox.Text,
                            Address = UserAddressTextBox.Text,
                            Phone = UserPhoneTextBox.Text
                        });
                        break;
                    case "update":
                        // TODO: You need an ID for updating, the code assumes you have a TextBox named `UserIdTextBox` for this purpose
                        request = new RestRequest($"/{UserIdTextBox.Text}", Method.Put);
                        request.AddJsonBody(new
                        {
                            Name = UserNameTextBox.Text,
                            Email = UserEmailTextBox.Text,
                            Address = UserAddressTextBox.Text,
                            Phone = UserPhoneTextBox.Text
                        });
                        break;
                    case "search":
                        request = new RestRequest($"/{UserIdTextBox.Text}", Method.Get);
                        response = client.Execute(request); // Declare and initialize the 'response' variable here
                        if (response.IsSuccessful)
                        {
                            var userProfile = JsonConvert.DeserializeObject<dynamic>(response.Content);
                            Console.WriteLine(userProfile);

                            // Populate the fields with the search result
                            UserNameTextBox.Text = userProfile.name;
                            UserEmailTextBox.Text = userProfile.email;
                            UserAddressTextBox.Text = userProfile.address;
                            UserPhoneTextBox.Text = userProfile.phone;
                        }
                        else
                        {
                            MessageBox.Show($"Search failed with error: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;

                    case "delete":
                        // TODO: For deleting, you need the ID, using `UserIdTextBox` for this
                        request = new RestRequest($"/{UserIdTextBox.Text}", Method.Delete);
                        break;
                    default:
                        throw new Exception("Invalid action specified");
                }

                response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    
                        MessageBox.Show($"{action.ToUpper()} operation successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
                else
                {
                    MessageBox.Show($"Operation failed with error: {response.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





    }
}
