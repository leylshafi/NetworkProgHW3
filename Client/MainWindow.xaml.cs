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

namespace Client;

public partial class MainWindow : Window
{
    UdpClient client;
    IPEndPoint remoteEP;
    public MainWindow()
    {
        InitializeComponent();
        client = new UdpClient();
        remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
    }

    private async void btnRequest_Click(object sender, RoutedEventArgs e)
    {
        var buffer = new byte[ushort.MaxValue - 29];
        await client.SendAsync(buffer, buffer.Length, remoteEP);
        var list = new List<byte>();
        var maxlen = buffer.Length;
        var len = 0;
        while (true)
        {
            do
            {
                var result = await client.ReceiveAsync();
                buffer = result.Buffer;
                len = buffer.Length;
                list.AddRange(buffer);
            } while (len == maxlen);
            var image = LoadImage(list.ToArray());
            if (image != null)
                Image.Source = image;

            list.Clear();
        }
    }

    private static BitmapImage? LoadImage(byte[] imageData)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = new MemoryStream(imageData);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }
}
