using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCommunicationUWP
{
	public abstract class CryptoHost
	{
		internal const int SERVER_PORT = 7777;
		internal const int CLIENT_PORT = 7778;

		internal AsynchronousSocketListenerSender _listener;
		internal CryptoProvider _provider;

		internal Random _a = new Random();

		internal byte[] buffer;

		internal byte[] _clientHello = new byte[256];
		internal byte[] _serverHello = new byte[256];
		internal byte[] _preMasterSecret = new byte[64];

		internal byte[] _sessionKey;

		public byte[] PreMasterSecret { get; set; }

		public byte[] MasterSecret { get; set; }

		public CryptoProviderMode Mode { get; }

		public CryptoHost(CryptoProviderMode cpm)
		{
			Mode = cpm;
		}
	}
}
