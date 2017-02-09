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
	public class CryptoClient
	{
		private AsynchronousSocketListener _listener;
		private CryptoProvider _provider;

		private Random _a = new Random();

		private byte[] buffer;

		private	byte[] _clientHello;
		private byte[] _serverHello;
		private byte[] _preMasterSecret;

		private byte[] _sessionKey;

		public CryptoClient()
		{
			_provider = new CryptoProvider(CryptoProviderMode.Client);
		}

		public void Connect (IPEndPoint endpoint)
		{
			_listener = new AsynchronousSocketListener(endpoint);
			_listener.Connect();
			CalculateClientHello();
			_listener.Send(_clientHello);
			_listener.RecieveAsync(RecievedPublicKey);
		}

		private void RecievedPublicKey(byte[] data)
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

			_listener.Send(preMasterEncrypted);

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
