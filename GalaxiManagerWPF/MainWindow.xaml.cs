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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
            ObservableCollection<BasicItem> items = new ObservableCollection<BasicItem>()
            {
                new BasicItem() { Name = "Yooo" }
            };
            listBx.ItemsSource = items;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += (object o, EventArgs e) =>
            {
                items.Add(new BasicItem() { Name = DateTime.Now.ToShortTimeString() });
            };
            timer.Start();
        }
    }
    public class BasicItem
    {
        public string Name { get; set; }
    }
}
