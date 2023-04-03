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
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography.Xml;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for AddOrEdit.xaml
    /// </summary>
    public partial class AddOrEdit : Window
    {
        public Single single;
        private Single backedSingle;
        public List<string> name = new List<string>();
        public string type = "";
        public bool isValid = false;
        public bool details = false;
        private bool backup = false;
        private Regex regex = new Regex(@"\b((?:https?://|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))");
        private List<double> FontSizes = new double[] { 2, 4, 8, 10, 12, 16, 18, 20, 22, 26, 28, 30, 32, 48, 72 }.ToList();


        public AddOrEdit(Single s)
        {
            InitializeComponent();

            single = s;
            backedSingle = s.Copy();
            this.DataContext = s;
            //  Load fonts
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(x => x.Source);
            //cmbFontFamily.SelectedIndex = 0;

            // Load font sizes
            cmbFontSize.ItemsSource = FontSizes;


            using (Stream stream = File.Open(single.Description, FileMode.OpenOrCreate))
            {
                TextRange tr = new TextRange(rtfDescription.Document.ContentStart, rtfDescription.Document.ContentEnd);
                tr.Load(stream, DataFormats.Rtf);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (type == "user")
            {
                MainToolbar.Visibility = Visibility.Hidden;

                rtfDescription.IsReadOnly = true;
                tbxLink.Visibility = Visibility.Hidden;
                tbxYear.Visibility = Visibility.Hidden;
                tbxName.Visibility = Visibility.Hidden;
                lblCover.Visibility = Visibility.Hidden;
                btnBrowse.Visibility = Visibility.Hidden;
                tbName.Visibility = Visibility.Visible;
                tbYear.Visibility = Visibility.Visible;
                lblLink.Visibility = Visibility.Hidden;
                lblNumOfWords.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
            }

        }

        private bool Validate()
        {
            int year = 0;
            int.TryParse(tbxYear.Text, out year);
            bool passed = true;
            string message = "";
            if (string.IsNullOrEmpty(tbxName.Text.Trim()))
            {
                passed = false;
                tbxName.BorderBrush = Brushes.Red;
            }
            if (year <= 1200 || year > DateTime.Now.Year)
            {
                passed = false;
                tbxYear.BorderBrush = Brushes.Red;
                message += $"\nYear must be between 1200 and {DateTime.Now.Year}";
            }
            if (!regex.IsMatch(tbxLink.Text.Trim()))
            {
                passed = false;
                tbxLink.BorderBrush = Brushes.Red;
            }
            TextRange tr = new TextRange(rtfDescription.Document.ContentStart, rtfDescription.Document.ContentEnd);
            if (string.IsNullOrEmpty(tr.Text.Trim()))
            {
                passed = false;
                rtfDescription.BorderBrush = Brushes.Red;
            }
            if (CoverImg.Source.ToString() == "pack://application:,,,/HCI Projekat;component/resources/noimage.png")
            {
                passed = false;
                btnBrowse.BorderBrush = Brushes.Red;
            }
            if (!passed)
            {
                MessageBox.Show("Fields must not be empty" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            isValid = passed;
            return passed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            if (Validate())
            {
                if (backup)
                {
                    single.Name = backedSingle.Name;
                    single.Year = backedSingle.Year;
                    single.CoverUrl = backedSingle.CoverUrl;
                    single.SpotifyLink = backedSingle.SpotifyLink;

                }
                else
                {
                    if (!File.Exists(single.Description))
                    {
                        single.Description = $"descriptions/{single.Name.Trim()}.rtf";
                    }
                    using (Stream stream = File.Open(single.Description, FileMode.OpenOrCreate))
                    {
                        TextRange tr = new TextRange(rtfDescription.Document.ContentStart, rtfDescription.Document.ContentEnd);
                        tr.Save(stream, DataFormats.Rtf);
                    }

                }

                this.Close();
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.InitialDirectory = picturesFolder;
            dialog.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|All Files (*.*)|*.*";
            dialog.CheckPathExists = true;
            bool? res = dialog.ShowDialog();
            if (res == true)
            {
                single.CoverUrl = dialog.FileName;
                btnBrowse.BorderBrush = Brushes.DarkOliveGreen;
            }
        }

        private void rtfDescription_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtfDescription.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));

            temp = rtfDescription.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;

            temp = rtfDescription.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (temp != DependencyProperty.UnsetValue)
            {
                double value = Math.Floor((double)temp);
                if (cmbFontSize.Items.Contains(value))
                {
                    cmbFontSize.SelectedItem = value;
                }
                else
                {
                    FontSizes.Add(value);
                    FontSizes.OrderBy(x => x);
                    cmbFontSize.ItemsSource = FontSizes;
                }
            }
            else
            {
                cmbFontSize.SelectedItem = 16;
            }

            temp = rtfDescription.Selection.GetPropertyValue((Inline.FontStyleProperty));
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));

            temp = rtfDescription.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtfDescription.Selection.GetPropertyValue(Inline.ForegroundProperty);
            colorPicker.SelectedColor = temp != DependencyProperty.UnsetValue ? ((SolidColorBrush)temp).Color : Colors.Black;
        }
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null && !rtfDescription.Selection.IsEmpty)
            {
                rtfDescription.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            }
        }
        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontSize.SelectedItem != null && !rtfDescription.Selection.IsEmpty)
            {
                rtfDescription.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.SelectedItem);
            }
        }

        private void rtfDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange tr = new TextRange(rtfDescription.Document.ContentStart, rtfDescription.Document.ContentEnd);
            int words = tr.Text.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
            lblNumOfWords.Content = $"Number of words: {words}";
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor.HasValue && !rtfDescription.Selection.IsEmpty)
            {
                rtfDescription.Selection.ApplyPropertyValue(Inline.ForegroundProperty, new SolidColorBrush((Color)colorPicker.SelectedColor));
            }
        }

        private void textBoxValidateColorOnChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).BorderBrush = Brushes.Black;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (details && type == "admin")
            {
                if (!single.Equals(backedSingle))
                {
                    var res = MessageBox.Show("You have unsaved changes!\nDo you wish to save them?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.Yes)
                    {
                        SaveData();
                        this.Close();
                    }
                    else if (res == MessageBoxResult.No)
                    {
                        backup = true;
                        SaveData();
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

    }
}
