﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace CryptoCommunicationUWP
{
    public class CryptoProvider
	{
		private RSACng _provider;
		private CryptoProviderMode _mode;
		private bool _clientSetted = false;
		private RSAEncryptionPadding _rsaPadding = RSAEncryptionPadding.OaepSHA512;

		private byte[] _publicKey;

		public byte[] PublicKey
		{
			get
			{
				return _publicKey;
			}
			set
			{
				value.CopyTo(_publicKey, 0);
				RSAParameters temp = _provider.ExportParameters(false);
				temp.Modulus = _publicKey;
				_provider.ImportParameters(temp);
				if(_mode == CryptoProviderMode.Client)
					_clientSetted = true;
			}
		}

		public CryptoProvider(CryptoProviderMode mode)
		{
			_mode = mode;
			_provider = new RSACng();
			_publicKey = _provider.ExportParameters(false).Modulus;
		}

		public CryptoProvider(CryptoProviderMode mode, RSAEncryptionPadding padding) : this(mode)
		{
			_rsaPadding = padding;
		}

		public byte[] EncryptPublicKey(byte[] data)
		{
			if (_mode == CryptoProviderMode.Client && !_clientSetted)
				throw new InvalidOperationException("You cannot encrypt if you are client and you didn't set the public key");
			return _provider.Encrypt(data, _rsaPadding);
		}

		public byte[] DecryptPreMasterSecret(byte[] data)
		{
			if (_mode == CryptoProviderMode.Client)
				throw new InvalidOperationException("You cannot decrypt if you are client");
			return _provider.Decrypt(data, _rsaPadding);
		}
	}
}
