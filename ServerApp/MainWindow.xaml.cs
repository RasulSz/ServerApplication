using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.IO;
using System.Net.NetworkInformation;
using System.Drawing;
using Path = System.IO.Path;
using Image = System.Drawing.Image;
using System.Threading;

namespace ServerApp
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

        public string LoadImage(byte[] buffer)
        {

            ImageConverter ic = new ImageConverter();
            var data = ic.ConvertFrom(buffer);
            Image img = data as Image;
            if (img != null)
            {
                Bitmap bitmap1 = new Bitmap(img);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Images2");
                Directory.CreateDirectory(path);
                var strGuid = Guid.NewGuid().ToString();
                bitmap1.Save($@"{path}\image {strGuid}.png");
                var imagepath = $@"{path}\image{strGuid}.png";
                return imagepath;
            }
            else
            {
                return String.Empty;
            }
        }

        private void OpenServerBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Task.Run(() =>
                {
                    var ipAdress = IPAddress.Parse("192.168.1.48");
                    var port = 80;
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        var ep = new IPEndPoint(ipAdress, port);
                        socket.Bind(ep);
                        socket.Listen(10);
                        this.Dispatcher.Invoke(() =>
                        {

                            InfoLbl.Content = $"Listen Over {socket.LocalEndPoint}";
                        });

                        var client = socket.Accept();
                        Task.Run(() =>
                        {
                        this.Dispatcher?.Invoke(() =>
                            {
                                var length = 0;
                                var bytes = new byte[30000];
                                do
                                {
                                    Thread.Sleep(1000);
                                    length = client.Receive(bytes);
                                    AcceptImage.Source = new BitmapImage(new Uri(LoadImage(bytes)));
                                    break;
                                } while (true);
                            });
                        });
                    }
                });
            });
        }
    }
}
