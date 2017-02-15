using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace TestServerWinIoT
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
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
            _server.ReceivedMessage += _server_ReceivedMessage;
			Task t = _server.StartListeningAsync();
			lstLog.Items.Add("Started listening");
		}

        private void _server_ReceivedMessage(object sender, string message, string ipAddress)
        {
            IAsyncAction t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateLogReceived(message, ipAddress);
            });
        }

        private void UpdateLogReceived(string message, string ipAddress)
        {
            lstLog.Items.Add(string.Format("Received : {0} from {1}", message, ipAddress));
        }

        private void _server_ConnectionMade(object sender, string ipAddress)
		{
			IAsyncAction t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				UpdateLogConnected(ipAddress);
			});
		}

		private void UpdateLogConnected(string ipAddress)
		{
			lstLog.Items.Add(string.Format("Connected IP {0}", ipAddress));
			lstLog.Items.Add(string.Format("WithPreMasterSecret {0}", string.Join(" , ", _server.MasterSecret)));
		}
	}
}
