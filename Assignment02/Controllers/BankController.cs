using Assignment02.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Assignment02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        // GET api/BankAccount
        [HttpGet]
        public IActionResult GetAllBankAccounts()
        {
            List<Bank> bankAccounts = DBManager.GetAllBanks();
            return Ok(bankAccounts);
        }

        // GET api/BankAccount/1234567890
        [HttpGet("{accountNumber}")]
        public IActionResult GetBankAccountByAccountNumber(string accountNumber)
        {
            Bank bankAccount = DBManager.GetBankByAccountNumber(accountNumber);
            if (bankAccount == null)
            {
                return NotFound("Bank account not found");
            }
            return Ok(bankAccount);
        }

        // POST api/BankAccount
        [HttpPost]
        public IActionResult CreateBankAccount([FromBody] Bank bankAccount)
        {
            if (DBManager.Insert(bankAccount))
            {
                return Ok("Bank account created successfully");
            }
            return BadRequest("Error in creating bank account");
        }

        // PUT api/BankAccount
        [HttpPut]
        public IActionResult UpdateBankAccount([FromBody] Bank bankAccount)
        {
            if (DBManager.UpdateBank(bankAccount))
            {
                return Ok("Bank account updated successfully");
            }
            return BadRequest("Error in updating bank account");
        }

        // DELETE api/BankAccount/1234567890
        [HttpDelete("{accountNumber}")]
        public IActionResult DeleteBankAccount(string accountNumber)
        {
            if (DBManager.DeleteBank(accountNumber))
            {
                return Ok("Bank account deleted successfully");
            }
            return BadRequest("Error in deleting bank account");
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] Transaction transaction)
        {
            if (transaction == null || transaction.Amount <= 0)
            {
                return BadRequest("Invalid deposit transaction data.");
            }

            var bankAccount = DBManager.GetBankByAccountNumber(transaction.AccountNumber);
            if (bankAccount == null)
            {
                return NotFound("Bank account not found.");
            }

            // Implement deposit logic here
            if (transaction.Amount > 0)
            {
                bankAccount.Balance += (double) transaction.Amount;

                // Update the bank account in the database
                if (DBManager.UpdateBank(bankAccount))
                {
                    DBManager.AddTransaction(transaction);
                    // You can also add the transaction to a transaction history.
                    // For example, create a TransactionHistory class and add the transaction there.
                    // TransactionHistory.AddTransaction(transaction);

                    return Ok("Deposit successful.");
                }
            }

            return BadRequest("Error in processing deposit.");
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] Transaction transaction)
        {
            if (transaction == null || transaction.Amount <= 0)
            {
                return BadRequest("Invalid withdrawal transaction data.");
            }

            var bankAccount = DBManager.GetBankByAccountNumber(transaction.AccountNumber);
            if (bankAccount == null)
            {
                return NotFound("Bank account not found.");
            }

            if (bankAccount.Balance < (double)transaction.Amount)
            {
                return BadRequest("Insufficient balance for withdrawal.");
            }

            // Implement withdrawal logic here
            if (transaction.Amount > 0)
            {
                bankAccount.Balance -= (double)transaction.Amount;

                // Update the bank account in the database
                if (DBManager.UpdateBank(bankAccount))
                {
                    DBManager.AddTransaction(transaction);
                    // You can also add the transaction to a transaction history.
                    // For example, create a TransactionHistory class and add the transaction there.
                    // TransactionHistory.AddTransaction(transaction);

                    return Ok("Withdrawal successful.");
                }
            }

            return BadRequest("Error in processing withdrawal.");
        }

        // GET api/BankAccount/transactions
        [HttpGet("transactions")]
        public IActionResult AllTransaction()
        {
            List<Transaction> transactions= DBManager.GetAllTransactions();
            return Ok(transactions);
        }

        // New method for transactions by specific ID
        [HttpGet("transactions/{id}")]
        public IActionResult TransactionsById(int id)
        {
            List<Transaction> transactions = DBManager.GetTransactionsById(id);
            if (transactions.Count == 0)
            {
                return NotFound($"No transactions found with ID: {id}");
            }
            return Ok(transactions);
        }

        // New method for transactions by account number
        [HttpGet("transactions/account/{accountNumber}")]
        public IActionResult TransactionsByAccountNumber(string accountNumber)
        {
          

            List<Transaction> transactions = DBManager.GetTransactionsByAccountNumber(accountNumber);
            if (transactions.Count == 0)
            {
                return NotFound($"No transactions found for account number: {accountNumber}");
            }
            return Ok(transactions);
        }

        [HttpPost("CreateUserProfile")]
        public IActionResult CreateUserProfile([FromBody] UserProfile userProfile)
        {
            if (DBManager.Insert(userProfile))
            {
                return Ok("User profile created successfully");
            }
            return BadRequest("Error in creating user profile");
        }

        // GET api/BankAccount/GetAllUserProfiles
        [HttpGet("GetAllUserProfiles")]
        public IActionResult GetAllUserProfiles()
        {
            List<UserProfile> userProfiles = DBManager.GetAllUserProfiles();
            return Ok(userProfiles);
        }
    }


}

