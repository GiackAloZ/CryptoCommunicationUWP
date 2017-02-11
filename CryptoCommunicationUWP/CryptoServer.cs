using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;


namespace CryptoCommunicationUWP
{
	public class CryptoServer : CryptoHost
	{
		public delegate void ConnectionMadeArgs(object sender, string ipAddress);
		public event ConnectionMadeArgs ConnectionMade;

		public CryptoServer() : base(CryptoProviderMode.Server)
		{
			_provider = new CryptoProvider(CryptoProviderMode.Server);
		}

		public async Task StartListeningAsync()
		{
			_listener = new AsynchronousSocketListenerSender(CLIENT_PORT, SERVER_PORT);
			_listener.RecieveDataEvent += RecievedRequestAsync;
			await _listener.StartListeningAsync(512);
		}

		private async void RecievedRequestAsync(byte[] data, string fromIp)
		{
			byte[] helloprivate = new byte[512];
			int i;
			for(i = 0; i < 256; i++)
			{
				helloprivate[i] = _serverHello[i];
			}
			byte[] publicKey = _provider.PublicKey;
			for(int j = 0; i < 512; i++, j++)
			{
				helloprivate[i] = publicKey[j];
			}
			_listener.SendToAsync(fromIp, helloprivate);
			_listener.RecieveDataEvent += RecievedPreMasterSecret;
			await _listener.StartListeningAsync(1024);
		}

		private void RecievedPreMasterSecret(byte[] data, string fromIp)
		{
			_preMasterSecret = _provider.DecryptPreMasterSecret(data);
			_preMasterSecret.CopyTo(PreMasterSecret, 0);

			ConnectionMade.Invoke(this, fromIp);
			//TODO
			//Exchange session keys
		}

		private void CalculateServerHello()
		{
			_a.NextBytes(_serverHello);
		}
	}
}
