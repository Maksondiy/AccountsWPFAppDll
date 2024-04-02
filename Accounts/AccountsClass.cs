namespace DllAccounts
{
    public enum AccountType
    {
        Saving,
        Checking
    }

    public interface IAccount
    {
        int Id { get; set; }
        int Balance { get; set; }
        AccountType AccountType { get; set; }

        string ToString();
        string ToStringZero();
    }
    public class Account : IAccount
    {
        public int Id { get; set; }
        public int Balance { get; set; }
        public AccountType AccountType { get; set; }

        public Account(int id, int balance)
        {
            Id = id;
            Balance = balance;
        }

        public Account(string row)//"Alex;4000"
        {
            string[] items = row.Split(';');
            Id = int.Parse(items[0]);
            Balance = int.Parse(items[1]);

            // Parse AccountType from the row if available
            if (items.Length > 2)
            {
                AccountType = (AccountType)Enum.Parse(typeof(AccountType), items[2]);
            }
        }
        public override string ToString()
        {
            return $"Account Id {Id}; balance {Balance}";
        }

        public string ToStringZero()
        {
            return Balance == 0 ? $"Id {Id}; Balance {Balance}" : string.Empty;
        }
    }

    public class SavingAccount : Account
    {
        public double InterestRate { get; set; }

        public SavingAccount(int id, int balance, double interestRate) : base(id, balance)
        {
            InterestRate = interestRate;
        }

        public SavingAccount(string row) : base(row)
        {
            string[] items = row.Split(';');
            InterestRate = double.Parse(items[2]);
            AccountType = AccountType.Saving;
        }

        public override string ToString()
        {
            return $"Account Id: {Id}, Balance: {Balance}, Interest Rate: {InterestRate}";
        }

        public void ApplyInterest()
        {
            Balance += (int)(Balance * InterestRate);
        }
    }

    public class CheckingAccount : Account
    {
        public int OverdraftLimit { get; set; }

        public CheckingAccount(int id, int balance, int overdraftLimit) : base(id, balance)
        {
            OverdraftLimit = overdraftLimit;
            AccountType = AccountType.Checking;
        }

        public override string ToString()
        {
            return $"Account Id: {Id}, Balance: {Balance}, Overdraft Limit : {OverdraftLimit}";
        }

        public bool Withdraw(int amount)
        {
            if (Balance - amount >= -OverdraftLimit)
            {
                Balance -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}