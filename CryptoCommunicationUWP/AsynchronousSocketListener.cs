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
	public class AsynchronousSocketListener
	{
		private IPEndPoint _endPoint;
		private Socket _listener;

		public delegate void RecieveData(byte[] data);
		public event RecieveData RecieveDataEvent;

		public AsynchronousSocketListener(IPEndPoint endPoint)
		{
			_endPoint = endPoint;
		}

		public void Connect()
		{
			// Create a TCP/IP socket.
			_listener = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			// Bind the socket to the local endpoint and listen for incoming connections.
			try
			{
				_listener.Connect(_endPoint);

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}

		public async void RecieveAsync(RecieveData rec)
		{
			RecieveDataEvent += rec;
			SocketAsyncEventArgs ev = new SocketAsyncEventArgs();
			_listener.AcceptAsync(ev);
			ev.Completed += Recieve;
		}

		private void Recieve(object sender, SocketAsyncEventArgs e)
		{
			SocketAsyncEventArgs eve = new SocketAsyncEventArgs();
			_listener.ReceiveAsync(eve);
			eve.Completed += ReturnBuffer;
		}

		private void ReturnBuffer(object sender, SocketAsyncEventArgs e)
		{
			RecieveDataEvent.Invoke(e.Buffer);
		}

		public int Send(byte[] data)
		{
			return _listener.Send(data);
		}
	}
}
