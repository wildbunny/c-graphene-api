using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceStack.Text;

namespace GrapheneRpc
{
	public class GrapheneHttpSocketWrapper
	{
		const int kTimeoutSeconds = 10;

		GrapheneWebsocket m_socket;

		Dictionary<GrapheneApi, int> m_apiMap;

		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Paul, 19/10/2015. </remarks>
		///
		/// <param name="url">	   	URL of the document. </param>
		/// <param name="username">	(Optional) the username. </param>
		/// <param name="password">	(Optional) the password. </param>
		public GrapheneHttpSocketWrapper(string url, bool login, string username="*", string password="*")
		{
			m_socket = new GrapheneWebsocket(url);
			m_socket.Connect();

			m_socket.OnClose += (o,s) => Reconnect();
			m_socket.OnError += (o,e) => Reconnect();

			m_apiMap = new Dictionary<GrapheneApi, int>
			{
				{GrapheneApi.@public, 0},
				{GrapheneApi.login, 1}
			};

			if (login)
			{
				bool done = false;

				while (!done)
				{
					try
					{
						bool success = ApiCallSync<bool>(GrapheneMethods.login, GrapheneApi.login, username, password);

						int history = ApiCallSync<int>(GrapheneMethods.history, GrapheneApi.login);
						int broadcast = ApiCallSync<int>(GrapheneMethods.network_broadcast, GrapheneApi.login);

						m_apiMap[GrapheneApi.history] = history;
						m_apiMap[GrapheneApi.network_broadcast] = broadcast;

						done = true;
					}
					catch { }
				}
			}
		}

		/// <summary>	Reconnects this object. </summary>
		///
		/// <remarks>	Paul, 10/11/2015. </remarks>
		void Reconnect()
		{
			m_socket.Connect();
		}

		/// <summary>	API call synchronise. </summary>
		///
		/// <remarks>	Paul, 23/10/2015. </remarks>
		///
		/// <typeparam name="T">	Generic type parameter. </typeparam>
		/// <param name="method">	The method. </param>
		/// <param name="args">  	A variable-length parameters list containing arguments. </param>
		///
		/// <returns>	A T. </returns>
		public T ApiCallSync<T>(GrapheneMethods method, GrapheneApi api, params object[] args)
		{
			int outer = System.Threading.Thread.CurrentThread.ManagedThreadId;

			Task<T> t = Task.Run<T>(() =>
			{
				if (System.Threading.Thread.CurrentThread.ManagedThreadId == outer)
				{
					throw new NotImplementedException();
				}
				return ApiCall<T>(method, api, args);
			});

			bool sucesss = t.Wait(kTimeoutSeconds * 1000);

			if (!sucesss)
			{
				throw new TimeoutException();
			}

			return t.Result;
		}

		/// <summary>	API call. </summary>
		///
		/// <remarks>	Paul, 19/10/2015. </remarks>
		///
		/// <typeparam name="T">	Generic type parameter. </typeparam>
		/// <param name="method">	The method. </param>
		/// <param name="args">  	A variable-length parameters list containing arguments. </param>
		///
		/// <returns>	A Task&lt;T&gt; </returns>
		async public Task<T> ApiCall<T>(GrapheneMethods method, GrapheneApi api, params object[] args)
		{
			TaskCompletionSource<T> task = new TaskCompletionSource<T>();
			EventHandler<string> onMessage = null;
			int id = -1;

			onMessage = (object sender, string message) =>
			{
				GrapheneSocketResponse<T> decoded = JsonSerializer.DeserializeFromString<GrapheneSocketResponse<T>>(message);

				if (decoded.id == id)
				{
					m_socket.OnMessage -= onMessage;

					if (decoded.error != null)
					{
						task.SetException(new GrapheneRpcException(new GrapheneError { message = decoded.error }));
					}
					else
					{
						task.SetResult(decoded.result);
					}
				}
			};

			m_socket.OnMessage += onMessage;
			id = m_socket.Send(method, m_apiMap[api], args);

			return await task.Task;
		}
	}
}
