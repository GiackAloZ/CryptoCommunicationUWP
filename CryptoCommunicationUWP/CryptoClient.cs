﻿using System;
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

		public bool IsReady { get; private set; }

		public delegate void ConnectionMadeArgs(object sender);
		public event ConnectionMadeArgs ConnectionMade;

		public CryptoClient() : base(CryptoProviderMode.Client)
		{
			_provider = new CryptoProvider(CryptoProviderMode.Client);
		}

		public async Task ConnectAsync (string ip)
		{
			_serverIp = ip;
			_listener = new AsynchronousSocketListenerSender(SERVER_PORT, CLIENT_PORT);
			_listener.ReceiveDataEvent += ReceivedPublicKey;
			CalculateClientHello();
			_listener.SendToAsync(_serverIp, _clientHello);
			await _listener.StartListeningAsync(512);
		}

		private void ReceivedPublicKey(byte[] data, string fromIp)
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

			PreMasterSecret = new byte[64];
			_preMasterSecret.CopyTo(PreMasterSecret, 0);

			_listener.SendToAsync(_serverIp, preMasterEncrypted);

			byte[] hashingBuffer = new byte[576];
			for (i = 0; i < 64; i++)
				hashingBuffer[i] = PreMasterSecret[i];
			for (int j = 0; i < (256 + 64); i++, j++)
				hashingBuffer[i] = _clientHello[j];
			for (int z = 0; i < (256 + 256 + 64); i++, z++)
				hashingBuffer[i] = _serverHello[z];

			MasterSecret = _provider.HashBuffer(hashingBuffer);

			Ready();
		}

		private void Ready()
		{
			IsReady = true;
			ConnectionMade?.Invoke(this);
		}

		private void CalculateClientHello()
		{
			_a.NextBytes(_clientHello);
		}

		private void CalculatePreMasterSecret()
		{
			_a.NextBytes(_preMasterSecret);
		}

		public void SendByteArray(byte[] data)
		{
            data = _provider.EncryptSymmetrically(data, MasterSecret);
			_listener.SendToAsync(_serverIp, data);
		}
	}
}
