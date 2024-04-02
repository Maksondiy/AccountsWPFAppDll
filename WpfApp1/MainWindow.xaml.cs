using DllAccounts;
using System.IO;
using System.Windows;

namespace WpfAppClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<IAccount> accounts = new List<IAccount>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string row in File.ReadAllLines("Balances.txt"))
            {
                string[] items = row.Split(';');
                int id = int.Parse(items[0]);
                int balance = int.Parse(items[1]);

                // Check if there are additional fields beyond the balance
                if (items.Length > 2)
                {
                    // Try to parse the interest rate
                    if (double.TryParse(items[2], out double interestRate))
                    {
                        // If the interest rate is successfully parsed, check if there's an overdraft limit
                        if (items.Length > 3)
                        {
                            // Try to parse the overdraft limit
                            if (int.TryParse(items[3], out int overdraftLimit) && overdraftLimit > interestRate)
                            {
                                // If the overdraft limit is successfully parsed and it's more than the interest rate,
                                // create a CheckingAccount
                                accounts.Add(new CheckingAccount(id, balance, overdraftLimit));
                                lbAccounts.Items.Add($"Checking Account: Id - {id}, Balance - {balance}, Overdraft Limit - {overdraftLimit}");
                                continue; // Continue to the next row
                            }
                        }
                        // If there's no overdraft limit or it's less than or equal to the interest rate, create a SavingAccount
                        accounts.Add(new SavingAccount(id, balance, interestRate));
                        lbAccounts.Items.Add($"Saving Account: Id - {id}, Balance - {balance}, Interest Rate - {interestRate}");
                    }
                    else
                    {
                        // If interest rate cannot be parsed, create a regular Account
                        accounts.Add(new Account(id, balance));
                        lbAccounts.Items.Add($"Account: Id - {id}, Balance - {balance}");
                    }
                }
                else
                {
                    // If there are no additional fields beyond the balance, create a regular Account
                    accounts.Add(new Account(id, balance));
                    lbAccounts.Items.Add($"Account: Id - {id}, Balance - {balance}");
                }
            }

            DisplayZeroBalanceAccounts();
            CalculateTotalBalance();
        }

        private void DisplayZeroBalanceAccounts()
        {
            var zeroBalanceAccounts = accounts.Where(a => a.Balance == 0);
            File.WriteAllLines("balance0.txt", zeroBalanceAccounts.Select(a => a.ToStringZero()));


            var zeroInterestAccounts = accounts.Where(a => a is SavingAccount account && account.InterestRate == 0);
            File.WriteAllLines("zeroInterestAccounts.txt", zeroInterestAccounts.Select(a => a.ToString()));

            foreach (var account in zeroBalanceAccounts)
            {
                rbAccounts.Items.Add(account.ToStringZero());
            }
        }

        private void CalculateTotalBalance()
        {
            int totalBalance = accounts.Sum(a => a.Balance);
            tbTotalBalance.Text = "Total Balance: " + totalBalance.ToString();
        }

    }
}