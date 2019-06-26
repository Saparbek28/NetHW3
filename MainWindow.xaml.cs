using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace NetHW3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _tcpClient;
        Thread listenThread;
        Thread sendThread;
        public MainWindow()
        {
            InitializeComponent();
            _tcpClient = new TcpClient();


        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _tcpClient.ConnectAsync(IPAddress.Parse(ipTextBox.Text), int.Parse(portTextBox.Text));
                listenThread = new Thread(ListenToServer);
                listenThread.Start(_tcpClient);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private void ListenToServer(object obj)
        {
            TcpClient tcpServer = obj as TcpClient;
            tcpServer.Client.Send(Encoding.ASCII.GetBytes("Saparbek"));
            byte[] buffer = new byte[4 * 1024];
            while (true)
            {
                var receivedSize = tcpServer.Client.Receive(buffer);
                TextToChat(Encoding.UTF8.GetString(buffer, 0, receivedSize));
            }
        }

        private void TextToChat(string text)
        {
            Dispatcher.Invoke(new ThreadStart(() => chatTextBox.AppendText(text + "\n")));
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sendThread = new Thread(SendMessage);
                sendThread.Start(_tcpClient);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void SendMessage(object obj)
        {
            TcpClient tcpServer = obj as TcpClient;
            string buffer = "";
            Dispatcher.Invoke(new ThreadStart(() => buffer = messageTextBox.Text));
            _tcpClient.Client.Send(Encoding.ASCII.GetBytes(buffer), SocketFlags.None);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
