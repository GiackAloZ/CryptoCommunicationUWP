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
		public bool IsReady { get; private set; }

		public delegate void ConnectionMadeArgs(object sender, string ipAddress);
		public event ConnectionMadeArgs ConnectionMade;

		public delegate void ReceivedMessageArgs(object sender, string message, string ipAddress);
		public event ReceivedMessageArgs ReceivedMessage;

		public CryptoServer() : base(CryptoProviderMode.Server)
		{
			_provider = new CryptoProvider(CryptoProviderMode.Server);
		}

		public async Task StartListeningAsync()
		{
			_listener = new AsynchronousSocketListenerSender(CLIENT_PORT, SERVER_PORT);
			_listener.ReceiveDataEvent += ReceivedRequestAsync;
			await _listener.StartListeningAsync(512);
		}

		private async void ReceivedRequestAsync(byte[] data, string fromIp)
		{
			_clientHello = new byte[256];
			int i;
			for(i = 0; i < 256; i++)
			{
				_clientHello[i] = data[i];
			}

			CalculateServerHello();

			byte[] helloprivate = new byte[512];
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
			_listener.ReceiveDataEvent -= ReceivedRequestAsync;
			_listener.ReceiveDataEvent += ReceivedPreMasterSecret;
			await _listener.StartListeningAsync(256);
		}

		private void ReceivedPreMasterSecret(byte[] data, string fromIp)
		{
			_preMasterSecret = _provider.DecryptPreMasterSecret(data);
			PreMasterSecret = new byte[64];
			_preMasterSecret.CopyTo(PreMasterSecret, 0);

			byte[] hashingBuffer = new byte[576];
			int i;
			for (i = 0; i < 64; i++)
				hashingBuffer[i] = PreMasterSecret[i];
			for (int j = 0; i < (256 + 64); i++, j++)
				hashingBuffer[i] = _clientHello[j];
			for (int z = 0; i < (256 + 256 + 64); i++, z++)
				hashingBuffer[i] = _serverHello[z];

			MasterSecret = _provider.HashBuffer(hashingBuffer);

			_listener.ReceiveDataEvent -= ReceivedPreMasterSecret;
			_listener.ReceiveDataEvent += ReceivedData;
			Task t = _listener.StartListeningAsync(1024);

			Ready(fromIp);
		}

		private void ReceivedData(byte[] data, string fromIp)
		{
            data = _provider.DecryptSymmetrically(data, MasterSecret);
			ReceivedMessage?.Invoke(this, Encoding.UTF8.GetString(data), fromIp);
		}

		private void Ready(string fromIp)
		{
			IsReady = true;
			ConnectionMade?.Invoke(this, fromIp);
		}

		private void CalculateServerHello()
		{
			_a.NextBytes(_serverHello);
		}
	}
}
