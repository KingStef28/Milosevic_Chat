using Milosevic_AsyncSocketLib;
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

namespace Milosevic_Chat_Client
{
    /// <summary>
    /// Logica di interazione per Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        AsyncSocketClient Client;
        public Chat(AsyncSocketClient client)
        {
            InitializeComponent();
            Client = client;
        }

        private void btn_invia_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_disconnetti_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
