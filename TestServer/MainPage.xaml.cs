using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using CryptoCommunicationUWP;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CryptoServer _server;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void grdMain_Loaded(object sender, RoutedEventArgs e)
        {
            _server = new CryptoServer();
            _server.ConnectionMade += _server_ConnectionMade;
            Task t = _server.StartListeningAsync();
            lstLog.Items.Add("Started listening");
			PrintMasterSecret(t);
        }

		private async void PrintMasterSecret(Task t)
		{
			await t;
			lstLog.Items.Add(string.Format("With PreMasterSecret = {0}", _server.PreMasterSecret));
		}

		private void _server_ConnectionMade(object sender, string ipAddress)
        {
            lstLog.Items.Add(string.Format("Connected IP {0}", ipAddress));
        }
    }
}
