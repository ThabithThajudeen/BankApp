namespace Assignment02.Models
{
    public class Bank
    {

        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountHolderName { get; set; }
        public double Balance { get; set; } // Include balance information

        // Additional fields for account details
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }


    }
}
