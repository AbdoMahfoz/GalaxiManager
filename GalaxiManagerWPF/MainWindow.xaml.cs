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
        Panel CurrentActivePanel = null;
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
            bool clientFound = false;
            CheckInSearchButton.IsEnabled = false;
            if(CheckInButton.IsEnabled)
            {
                ResetContent();
                await Task.Delay(500);
            }
            ((Storyboard)InputBorder.Resources["LoadingAnimation"]).Begin();
            if(!int.TryParse(CheckInSearchText.Text, out int n) || n<0)
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
                clientFound = true;
                CheckInHistory lastCheckIn = null;
                await Task.Run(() =>
                {
                    lastCheckIn = Galaxi.GetLastCheckin(client);
                });
                if (lastCheckIn == null)
                {
                    HasCheckedIn = false;
                }
                else
                {
                    HasCheckedIn = !lastCheckIn.IsCheckedOut;
                }
                CheckInClientName.Content = client.Name;
                CheckInClienYear.Content = client.Year.ToString();
                CheckInEmail.Content = client.Email;
                CheckInFacultyName.Content = client.Faculty.Name;
                CheckInStatus.Content = HasCheckedIn ? $"Checked-In at {lastCheckIn.CheckIn.ToShortTimeString()}" : "Not Checked-In";
            }
            CheckInSearchButton.IsEnabled = true;
            if(clientFound)
            CheckInButton.IsEnabled = true;
            else
            CheckInButton.IsEnabled = false;
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
        private void ApplyNavigation(Panel NewPanel)
        {
            if (CurrentActivePanel == null)
            {
                NewPanel.Opacity = 0.0;
                NewPanel.Visibility = Visibility.Visible;
                CurrentActivePanel = NewPanel;
                ((Storyboard)CurrentActivePanel.Resources["FadeIn"]).Begin();
            }
            else if(CurrentActivePanel != NewPanel)
            {
                Storyboard storyboard = (Storyboard)CurrentActivePanel.Resources["FadeOut"];
                storyboard.Completed += (object sender, EventArgs e) =>
                {
                    CurrentActivePanel.Visibility = Visibility.Hidden;
                    NewPanel.Opacity = 0.0;
                    NewPanel.Visibility = Visibility.Visible;
                    CurrentActivePanel = NewPanel;
                    ((Storyboard)CurrentActivePanel.Resources["FadeIn"]).Begin();
                };
                storyboard.Begin();
            }
        }
        private void NavigationClick(object sender, RoutedEventArgs e)
        {
            Button NavButton = (Button)sender;
            switch(NavButton.Name)
            {
                case "NavigationCheckInOutButton":
                    ApplyNavigation(CheckInOutPanel);
                    break;
                case "NavigationStockReportButton":
                    ApplyNavigation(StockReportPanel);
                    break;
            }
        }
        private async void StockReportPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(StockReportPanel.Visibility == Visibility.Visible)
            {
                Payment[] payments = null;
                await Task.Run(() =>
                {
                    payments = Galaxi.GetAllPayments();
                });
                ObservableCollection<Payment> paymentsList = new ObservableCollection<Payment>();
                StockReportItems.ItemsSource = paymentsList;
                if (payments == null)
                {
                    MessageBox.Show("No stock Available !");
                    return;
                }
                foreach(Payment payment in payments)
                {
                    paymentsList.Add(payment);
                    await Task.Delay(100);
                }
            }
        }
    }
}
