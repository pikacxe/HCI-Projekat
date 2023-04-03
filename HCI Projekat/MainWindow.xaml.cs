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
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Single> singles = new ObservableCollection<Single>();
        public IList<Single> ToBeDeleted = new List<Single>();
        private string type = string.Empty;
        public MainWindow(string type)
        {
            InitializeComponent();

            //  Read data from file
            // ReadSingles();

            // Load data
            DataIO dataIO = new DataIO();
            singles = dataIO.DeSerializeObject<ObservableCollection<Single>>("resources/data.xml");

            //  Set windows context
            this.DataContext = this;

            //  Set ListView source
            dataTable.ItemsSource = singles;


            //  Determine type of user
            if (type == "user")
            {
                Title.Content = "Welcome, User";
                btnAdd.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
            }
            else
            {
                Title.Content = "Welcome, Admin";
            }
            this.type = type;
        }


        //private void ReadSingles()
        //{
        //    string[] lines = File.ReadAllLines("resources/wiz.txt");
        //    foreach (var line in lines.OrderBy(x => x.First()))
        //    {
        //        if (string.IsNullOrEmpty(line.Trim())) continue;
        //        string[] data = line.Split('|');
        //        Single single = new Single(data[0], int.Parse(data[1]), data[2], data[3], data[4], DateTime.Parse(data[5]));
        //        singles.Add(single);
        //    }
        //}

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddOrEdit aore = new AddOrEdit(new Single());
            aore.type = type;
            aore.ShowDialog();
            if (aore.isValid)
            {
                singles.Add(aore.single);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            DataIO dataIO = new DataIO();
            dataIO.SerializeObject(singles, "resources/data.xml");
            this.Close();
        }
        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            AddOrEdit aore = new AddOrEdit((Single)dataTable.SelectedItem);
            aore.lblTitle.Content = type == "admin" ? "Edit record" : "About";
            aore.type = type;
            aore.details = true;
            aore.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(ToBeDeleted.Count == 0)
            {
                MessageBox.Show("Please select one or more checkboxes inside entries you want to delete!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                foreach (var x in ToBeDeleted)
                {
                    singles.Remove(x);
                    if (File.Exists(x.Description))
                    {
                        File.Delete(x.Description);
                    }
                }
                ToBeDeleted.Clear();
            }
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                item.IsSelected = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("This will open selected link in default web browser.\n Do you wish to procced?\n\n*We are not responsible for integrity of the provided link*", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    // Specify the path to the Google Chrome executable file.
                    string browserPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

                    // Use the specified browser to navigate to the URL.
                    Process.Start(new ProcessStartInfo(browserPath, e.Uri.ToString()));

                    // Use the default browser to navigate to the URL
                    //System.Diagnostics.Process.Start(e.Uri.ToString());
                }
                catch (Exception ex)
                {
                    // Display an error message if the default browser is not set or there is an issue with the browser configuration
                    MessageBox.Show("You realy do not have chrome? -.-'\n\n" + ex.Message, "Web error", MessageBoxButton.OKCancel,MessageBoxImage.Error);
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cbhCard_Checked(object sender, RoutedEventArgs e)
        {
            ToBeDeleted.Add((Single)dataTable.SelectedItem);
        }

        private void cbhCard_Unchecked(object sender, RoutedEventArgs e)
        {
            ToBeDeleted.Remove((Single)dataTable.SelectedItem);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
