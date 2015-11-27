using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using ServiceStack.Text;
using WebSocketSharp;

namespace GrapheneRpc
{
	public class GrapheneWebsocket : IDisposable
	{
		readonly string m_url;

		WebSocket m_socket;
		int m_id;
		
		public EventHandler<string> OnMessage;
		public EventHandler<string> OnClose;
		public EventHandler<Exception> OnError;

		public GrapheneWebsocket(string url)
		{
			m_id = 1;
			m_url = url;

			m_socket = new WebSocket(url);

			m_socket.OnMessage += OnMessageInternal;
			m_socket.OnClose += OnCloseInternal;
			m_socket.OnError += OnErrorInternal;
		}

		public void Connect()
		{
			m_socket.Connect();
		}

		public int Send(GrapheneMethods method, int api, params object[] args)
		{
			GrapheneSocketRequest r = new GrapheneSocketRequest(method, m_id++, api, args);
			string request = JsonSerializer.SerializeToString<GrapheneSocketRequest>(r);
			m_socket.Send(request);

			return m_id - 1;
		}

		void OnErrorInternal(object sender, ErrorEventArgs e)
		{
			if (OnError != null)
			{
				OnError(this, e.Exception);
			}
		}

		void OnCloseInternal(object sender, CloseEventArgs e)
		{
			if (OnClose != null)
			{
				OnClose(this, e.Reason);
			}
		}

		void OnMessageInternal(object sender, MessageEventArgs e)
		{
			Debug.Assert(e.Type == Opcode.Text);

			if (OnMessage != null)
			{
				OnMessage(this, UTF8Encoding.UTF8.GetString(e.RawData));
			}
		}
		
		public void Dispose()
		{
			m_socket.Close();
		}
	}
}
