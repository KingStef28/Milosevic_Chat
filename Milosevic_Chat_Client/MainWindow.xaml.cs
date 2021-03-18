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

namespace Milosevic_Chat_Client
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            txt_ipaddr.Text = "127.0.0.1";
            txt_port.Text = "23000";
        }

        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            Chat win = new Chat();
            win.Show();
            this.Hide();
        }
    }
}
