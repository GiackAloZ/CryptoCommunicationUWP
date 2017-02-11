using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using Windows.Networking.Sockets;
using System.IO;
using Windows.Networking;

namespace CryptoCommunicationUWP
{
	public class AsynchronousSocketListenerSender
	{
		private int _sendPort;
		private int _receivePort;
		private StreamSocketListener _listener;
		private StreamSocket _sender;

		private byte[] _buffer;
		private int _bufferLength;

		public delegate void RecieveData(byte[] data, string fromIp);
		public event RecieveData RecieveDataEvent;

		public AsynchronousSocketListenerSender(int sendport, int receiveport)
		{
			_sendPort = sendport;
			_receivePort = receiveport;
		}

		public async Task StartListeningAsync(int bufferLength)
		{
			// Create a TCP/IP socket.
			_listener = new StreamSocketListener();
			_bufferLength = bufferLength;

			// Bind the socket to the local endpoint and listen for incoming connections.
			await _listener.BindServiceNameAsync(_receivePort.ToString());
			_listener.ConnectionReceived += ConnectionReceived;

		}

		private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			//Read line from the remote client.
			Stream inStream = args.Socket.InputStream.AsStreamForRead();
			StreamReader reader = new StreamReader(inStream);
			char[] buff = new char[_bufferLength];
			await reader.ReadAsync(buff, 0, _bufferLength);
			_buffer = new byte[_bufferLength];
			for(int i = 0; i < _bufferLength; i++)
			{
				_buffer[i] = (byte)buff[i];
			}
			RecieveDataEvent.Invoke(_buffer, args.Socket.Information.RemoteAddress.CanonicalName);
		}

		public async void SendToAsync(string ip, byte[] data)
		{
			_sender = new StreamSocket();
			await _sender.ConnectAsync(new HostName(ip), _sendPort.ToString());

			Stream streamOut = _sender.OutputStream.AsStreamForWrite();
			StreamWriter writer = new StreamWriter(streamOut);
			char[] buff = new char[data.Length];
			for (int i = 0; i < data.Length; i++)
				buff[i] = (char)data[i];
			await writer.WriteLineAsync(buff);
			await writer.FlushAsync();

			_sender.Dispose();
		}
	}
}
