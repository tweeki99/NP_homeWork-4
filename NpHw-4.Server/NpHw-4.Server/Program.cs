using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NpHw_4.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 12345;
            UdpClient server = null;

            server = new UdpClient(port);
            IPEndPoint remoteEP = null;
            while (true)
            {
                Bitmap screen = Screenshot();
                byte[] screenData = ObjectToByteArray(screen);

                byte[] bytes = server.Receive(ref remoteEP);
                bytes = Encoding.UTF8.GetBytes(screenData.Length.ToString());
                server.Send(bytes, bytes.Length, remoteEP);

                for (int i = 0; i < screenData.Length; i++)
                {
                    server.Send(new byte[] { screenData[i] }, 1, remoteEP);
                }
            }
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static Bitmap Screenshot()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var g = Graphics.FromImage(bmp))
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            return bmp;
        }
    }
}

