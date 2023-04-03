using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static Dictionary<string, string> accounts = new Dictionary<string, string>(2); 

        public LoginWindow()
        {
            InitializeComponent();
            accounts["admin"] = "admin";
            accounts["user"] = "user";
            tbxUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(tbxUsername.Text) && !string.IsNullOrEmpty(tbxPassword.Password))
            {
                if (accounts.ContainsKey(tbxUsername.Text) && tbxPassword.Password == accounts[tbxUsername.Text])
                {
                    MainWindow mw = new MainWindow(tbxUsername.Text);
                    mw.ShowDialog();
                    tbxUsername.Text = "";
                    tbxPassword.Password = "";
                    tbxUsername.Focus();
                }
                else
                {
                    MessageBox.Show("Username and password does not match any registered account", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if(string.IsNullOrEmpty(tbxPassword.Password))
                tbxPassword.BorderBrush = Brushes.Red;
                if(string.IsNullOrEmpty(tbxUsername.Text))
                tbxUsername.BorderBrush = Brushes.Red;
                MessageBox.Show("Fields must not be empty!", "Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void tbxUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbxUsername.BorderBrush = Brushes.Gray;
        }

        private void tbxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            tbxPassword.BorderBrush = Brushes.Gray;
        }
    }
}
