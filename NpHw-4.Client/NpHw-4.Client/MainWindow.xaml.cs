using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Windows.Threading;

namespace NpHw_4.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int port = 12345;
        
        IPEndPoint remoteEP = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(LoadScreenshotProcess, new object());
        }

        private void LoadScreenshotProcess(object obj)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(IPAddress.Parse("10.3.3.39"), port);

            byte[] data = new byte[1];
            udpClient.Send(data, data.Length);
            data = udpClient.Receive(ref remoteEP);
            int bytes = int.Parse(Encoding.UTF8.GetString(data));

            byte[] bitmapObject = new byte[bytes];


            for (int i = 0; i < bytes; i++)
            {
                data = udpClient.Receive(ref remoteEP);
                bitmapObject[i] = data[0];
            }

            Bitmap screen = (Bitmap)ByteArrayToObject(bitmapObject);

            Dispatcher.Invoke(new ThreadStart(() => image.Source = Convert(screen)));

            udpClient.Close();
        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
