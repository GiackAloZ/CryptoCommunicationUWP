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
	public class CryptoClient : CryptoHost
	{
		private string _serverIp;

		public CryptoClient() : base(CryptoProviderMode.Client)
		{
			_provider = new CryptoProvider(CryptoProviderMode.Client);
		}

		public void Connect (string ip, int port)
		{
			_serverIp = ip;
			_listener = new AsynchronousSocketListenerSender(port);
			CalculateClientHello();
			_listener.SendToAsync(_serverIp, _clientHello);
			_listener.RecieveDataEvent += RecievedPublicKey;
			_listener.StartListeningAsync(1024);
		}

		private void RecievedPublicKey(byte[] data, string fromIp)
		{
			CalculatePreMasterSecret();

			_serverHello = new byte[256];
			int i;
			for(i = 0; i < 256; i++)
			{
				_serverHello[i] = data[i];
			}
			byte[] publicKey = new byte[256];
			for(int j = 0; i < 512; i++, j++)
			{
				publicKey[j] = data[i];
			}

			_provider.PublicKey = publicKey;
			byte[] preMasterEncrypted = _provider.EncryptPublicKey(_preMasterSecret);

			_preMasterSecret.CopyTo(PreMasterSecret, 0);

			_listener.SendToAsync(_serverIp, preMasterEncrypted);

			//TODO
			//calculate master secret and session keys
		}

		private void CalculateClientHello()
		{
			_a.NextBytes(_clientHello);
		}

		private void CalculatePreMasterSecret()
		{
			_a.NextBytes(_preMasterSecret);
		}
	}
}
