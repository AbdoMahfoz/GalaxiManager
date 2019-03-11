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
using System.Threading;

namespace GalaxiManagerWPF
{
    public partial class MainWindow : Window
    {
        Client CurrentActiveClient;
        public MainWindow()
        {
            InitializeComponent();
            ((Storyboard)CheckInOutPanel.Resources["ButtonReset"]).Completed += (object sender, EventArgs e) =>
            {
                CheckInButton.IsEnabled = false;
            };
        }
        void ResetContent()
        {
            CheckInButton.Content = "Check in/out";
            ((Storyboard)CheckInOutPanel.Resources["ButtonReset"]).Begin();
            CheckInClientName.Content = "---";
            CheckInClienYear.Content = "---";
            CheckInEmail.Content = "---";
            CheckInFacultyName.Content = "---";
            CheckInStatus.Content = "---";
        }
        private async void SearchButtonPressed(object sender, RoutedEventArgs e)
        {
            CheckInSearchButton.IsEnabled = false;
            if(CheckInButton.IsEnabled)
            {
                ResetContent();
                await Task.Run(() => { Thread.Sleep(500); });
            }
            ((Storyboard)InputBorder.Resources["LoadingAnimation"]).Begin();
            if(!int.TryParse(CheckInSearchText.Text, out int n))
            {
                MessageBox.Show("Invalid phone number");
                ((Storyboard)InputBorder.Resources["LoadingAnimation"]).Stop();
                ((Storyboard)InputBorder.Resources["EndingAnimation"]).Begin();
                CheckInSearchButton.IsEnabled = true;
                return;
            }
            string Phonenumber = CheckInSearchText.Text;
            Client client = null;
            bool HasCheckedIn = false;
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
                CheckInHistory lastCheckIn = null;
                await Task.Run(() =>
                {
                    lastCheckIn = Galaxi.GetLastCheckin(client);
                });
                HasCheckedIn = !lastCheckIn.IsCheckedOut;
                CheckInClientName.Content = client.Name;
                CheckInClienYear.Content = client.Year.ToString();
                CheckInEmail.Content = client.Email;
                CheckInFacultyName.Content = client.Faculty.Name;
                CheckInStatus.Content = HasCheckedIn ? $"Checked-In at {lastCheckIn.CheckIn.ToShortTimeString()}" : "Not Checked-In";
            }
            CheckInSearchButton.IsEnabled = true;
            CheckInButton.IsEnabled = true;
            ((Storyboard)InputBorder.Resources["LoadingAnimation"]).Stop();
            ((Storyboard)InputBorder.Resources["EndingAnimation"]).Begin();
            if(!HasCheckedIn)
            {
                CheckInButton.Content = "Check in!";
                ((Storyboard)CheckInOutPanel.Resources["CheckInEnabled"]).Begin();
            }
            else
            {
                CheckInButton.Content = "Check out!";
                ((Storyboard)CheckInOutPanel.Resources["CheckOutEnabled"]).Begin();
            }
            CurrentActiveClient = client;
        }
        private async void CheckInButtonPressed(object sender, RoutedEventArgs e)
        {
            CheckInButton.IsEnabled = false;
            CheckInSearchButton.IsEnabled = false;
            if ((string)CheckInButton.Content == "Check in!")
            {
                await Task.Run(() =>
                { 
                    Galaxi.CheckInClient(CurrentActiveClient);
                    MessageBox.Show("Checked-In Successfully!");
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    Galaxi.CheckOutClient(CurrentActiveClient);
                    MessageBox.Show("Checked-Out Successfully!");
                });
            }
            ResetContent();
            CheckInSearchButton.IsEnabled = true;
        }
    }
}
