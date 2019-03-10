using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GalaxiBackend;

namespace GalaxiManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void SearchButtonPressed(object sender, RoutedEventArgs e)
        {
            CheckInSearchButton.IsEnabled = false;
            ((Storyboard)InputBorder.Resources["LoadingAnimation"]).Begin();
            if(!int.TryParse(CheckInSearchText.Text, out int n))
            {
                MessageBox.Show("Invalid phone number");
                CheckInSearchButton.IsEnabled = true;
                ((Storyboard)InputBorder.Resources["LoadingAnimation"]).AutoReverse = false;
                return;
            }
            string Phonenumber = CheckInSearchText.Text;
            Client client = null;
            await Task.Run(() =>
            {
                client = Galaxi.GetClient(Phonenumber);
            });
            if (client == null)
            {
                MessageBox.Show("Client does not exist");
            }
            else
            {
                CheckInClientName.Content = client.Name;
                CheckInClienYear.Content = client.Year.ToString();
                CheckInEmail.Content = client.Email;
                CheckInFacultyName.Content = client.Faculty.Name;
                CheckInStatus.Content = (client.CheckedIn) ? "Checked-In" : "Not Checked-In";
            }
            CheckInSearchButton.IsEnabled = true;
            ((Storyboard)InputBorder.Resources["LoadingAnimation"]).AutoReverse = false;
        }
    }
}
