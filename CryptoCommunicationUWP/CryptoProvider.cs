using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

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

		public byte[] HashBuffer(byte[] data)
		{
			byte[] res = new byte[data.Length];
			data.CopyTo(res, 0);
			HashAlgorithmProvider alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
			IBuffer buff = CryptographicBuffer.CreateFromByteArray(res);
			buff = alg.HashData(buff);
			CryptographicBuffer.CopyToByteArray(buff, out res);
			return res;
		}

		public byte[] EncryptSymmetrically(byte[] data, byte[] key)
		{
			byte[] res = new byte[data.Length];
			data.CopyTo(res, 0);

            SymmetricKeyAlgorithmProvider AES = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey cryKey = AES.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(key));
            IBuffer buffData = CryptographicBuffer.CreateFromByteArray(res);

            IBuffer crypted = CryptographicEngine.Encrypt(cryKey, buffData, null);

            CryptographicBuffer.CopyToByteArray(crypted, out res);

            return res;
		}

        public byte[] DecryptSymmetrically(byte[] data, byte[] key)
        {
            byte[] res = new byte[data.Length];
            data.CopyTo(res, 0);

            SymmetricKeyAlgorithmProvider AES = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey cryKey = AES.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(key));
            IBuffer buffData = CryptographicBuffer.CreateFromByteArray(res);

            IBuffer encrypted = CryptographicEngine.Decrypt(cryKey, buffData, null);

            CryptographicBuffer.CopyToByteArray(encrypted, out res);

            return res;
        }
    }
}
