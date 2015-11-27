using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;

using RestLib;
using ServiceStack.Text;

namespace GrapheneRpc
{
	public class GrapheneRpcException : Exception
	{
		public GrapheneError m_error;

		public GrapheneRpcException(GrapheneError error)
		{
			m_error = error;
		}

		public override string Message { get { return m_error.message; } }
	}

	public class GrapheneWallet
	{
		public const int kBitsharesMaxAccountNameLength = 63;

		string m_rpcUrlCli;
		string m_rpcUrlBlockchain;
		string m_rpcUsername;
		string m_rpcPassword;

		GrapheneHttpSocketWrapper m_cliWallet;
		GrapheneHttpSocketWrapper m_blockchain;

		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Paul, 26/11/2014. </remarks>
		///
		/// <param name="rpcUrl">	  	Bitshares RPC root url. </param>
		/// <param name="rpcUsername">	The RPC username. </param>
		/// <param name="rpcPassword">	The RPC password. </param>
		public GrapheneWallet(string rpcUrls, string rpcUsername, string rpcPassword)
		{
			string[] urls = rpcUrls.Split(',');
			m_rpcUrlCli = urls[0];
			m_rpcUrlBlockchain = urls[1];
			m_rpcUsername = rpcUsername;
			m_rpcPassword = rpcPassword;

			ConfigureSerialisation();

			m_cliWallet = new GrapheneHttpSocketWrapper(m_rpcUrlCli, false, rpcUsername, rpcPassword);
			m_blockchain = new GrapheneHttpSocketWrapper(m_rpcUrlBlockchain, true, rpcUsername, rpcPassword);
		}

		/// <summary>	Configure serialisation. </summary>
		///
		/// <remarks>	Paul, 19/10/2015. </remarks>
		static public void ConfigureSerialisation()
		{
			// configure servicestack.text to be able to parse the bitshares rpc responses
			JsConfig<decimal>.DeSerializeFn = s => decimal.Parse(s, NumberStyles.Float);
			JsConfig<GrapheneObject>.DeSerializeFn = s => new GrapheneObject(s);
			JsConfig<GrapheneObject>.SerializeFn = o => o.ToString();
			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			JsConfig.IncludeTypeInfo = false;
			JsConfig.IncludePublicFields = true;
			JsConfig.IncludeNullValues = true;
		}

		/// <summary>	API call. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="method">	The method. </param>
		/// <param name="args">  	A variable-length parameters list containing arguments. </param>
		///
		/// <returns>	A T. </returns>
		public async Task<T> ApiCallAsync<T>(GrapheneMethods method, GrapheneApi api, params object[] args)
		{
			return await m_cliWallet.ApiCall<T>(method, api, args);
		}

		/// <summary>	API call. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="method">	The method. </param>
		/// <param name="args">  	A variable-length parameters list containing arguments. </param>
		///
		/// <returns>	A T. </returns>
		public T ApiCall<T>(GrapheneMethods method, GrapheneApi api, params object[] args)
		{
			return m_cliWallet.ApiCallSync<T>(method, api, args);
		}

		/// <summary>	API call blockchain. </summary>
		///
		/// <remarks>	Paul, 23/10/2015. </remarks>
		///
		/// <typeparam name="T">	Generic type parameter. </typeparam>
		/// <param name="method">	The method. </param>
		/// <param name="args">  	A variable-length parameters list containing arguments. </param>
		///
		/// <returns>	A T. </returns>
		public T ApiCallBlockchain<T>(GrapheneMethods method, GrapheneApi api, params object[] args)
		{
			return m_blockchain.ApiCallSync<T>(method, api, args);
		}

		/// <summary>	Gets an account. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="account">	The account. </param>
		///
		/// <returns>	The account. </returns>
		public GrapheneAccount GetAccount(string account)
		{
			return ApiCall<GrapheneAccount>(GrapheneMethods.get_account, GrapheneApi.@public, account);
		}

		/// <summary>	Gets an asset. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="symbol">	The symbol. </param>
		///
		/// <returns>	The asset. </returns>
		public GrapheneAsset GetAsset(string symbol)
		{
			return ApiCall<GrapheneAsset>(GrapheneMethods.get_asset, GrapheneApi.@public, symbol);
		}

		/// <summary>	Gets an object. </summary>
		///
		/// <remarks>	Paul, 24/10/2015. </remarks>
		///
		/// <typeparam name="T">	Generic type parameter. </typeparam>
		/// <param name="id">	The identifier. </param>
		///
		/// <returns>	The object. </returns>
		public T GetObject<T>(GrapheneObject id)
		{
			return ApiCall<T[]>(GrapheneMethods.get_object, GrapheneApi.@public, id)[0];
		}

		/// <summary>	List account balances. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="account">	The account. </param>
		///
		/// <returns>	A Task&lt;GrapheneAmount[]&gt; </returns>
		public GrapheneAmount[] ListAccountBalances(string account)
		{
			return ApiCall<GrapheneAmount[]>(GrapheneMethods.list_account_balances, GrapheneApi.@public, account);
		}

		/// <summary>	List assets. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <returns>	A Task&lt;GrapheneAsset[]&gt; </returns>
		public GrapheneAsset[] ListAssets(string startSymbol="", int limit=100)
		{
			return ApiCall<GrapheneAsset[]>(GrapheneMethods.list_assets, GrapheneApi.@public, startSymbol, limit);
		}

		/// <summary>	Transfers. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="from">			Source for the. </param>
		/// <param name="to">			to. </param>
		/// <param name="amount">   	The amount. </param>
		/// <param name="symbol">   	The symbol. </param>
		/// <param name="memo">			(Optional) the memo. </param>
		/// <param name="broadcast">	(Optional) true to broadcast. </param>
		///
		/// <returns>	A Task&lt;GrapheneTransactionRecord&gt; </returns>
		public Dictionary<string, GrapheneTransactionRecord> Transfer(string from, string to, decimal amount, string symbol, string memo = "")
		{
			return ApiCall<Dictionary<string, GrapheneTransactionRecord>>(GrapheneMethods.transfer2, GrapheneApi.@public, from, to, Numeric.SerialisedDecimal(amount), symbol, memo);
		}

		/// <summary>	Issue asset. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="to">			to. </param>
		/// <param name="amount">   	The amount. </param>
		/// <param name="symbol">   	The symbol. </param>
		/// <param name="memo">			(Optional) the memo. </param>
		/// <param name="broadcast">	(Optional) true to broadcast. </param>
		///
		/// <returns>	A Task&lt;GrapheneTransactionRecord&gt; </returns>
		public Dictionary<string, GrapheneTransactionRecord> IssueAsset(string to, decimal amount, string symbol, string memo = "")
		{
			return ApiCall<Dictionary<string, GrapheneTransactionRecord>>(GrapheneMethods.issue_asset2, GrapheneApi.@public, to, Numeric.SerialisedDecimal(amount), symbol, memo);
		}

		/// <summary>	Burn uia. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="account">  	The account. </param>
		/// <param name="amount">   	The amount. </param>
		/// <param name="symbol">   	The symbol. </param>
		/// <param name="broadcast">	(Optional) true to broadcast. </param>
		///
		/// <returns>	A Task&lt;GrapheneTransactionRecord&gt; </returns>
		public Dictionary<string,GrapheneTransactionRecord> BurnUia(string account, decimal amount, string symbol)
		{
			return ApiCall<Dictionary<string, GrapheneTransactionRecord>>(GrapheneMethods.reserve_asset2, GrapheneApi.@public, account, Numeric.SerialisedDecimal(amount), symbol);
		}

		/// <summary>	Gets the information. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <returns>	A Task&lt;GrapheneInfo&gt; </returns>
		public GrapheneInfo Info()
		{
			return ApiCall<GrapheneInfo>(GrapheneMethods.info, GrapheneApi.@public);
		}

		/// <summary>	Gets a transaction. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="txid">	The txid. </param>
		///
		/// <returns>	The transaction. </returns>
		public GrapheneTransactionRecord GetTransaction(uint blockNum, uint transactionInBlock)
		{
			return ApiCallBlockchain<GrapheneTransactionRecord>(GrapheneMethods.get_transaction, GrapheneApi.@public, blockNum, transactionInBlock);
		}

		/// <summary>	Gets recent transaction. </summary>
		///
		/// <remarks>	Paul, 23/10/2015. </remarks>
		///
		/// <param name="txid">	The txid. </param>
		///
		/// <returns>	The recent transaction. </returns>
		public GrapheneTransactionRecord GetRecentTransaction(string txid)
		{
			return ApiCallBlockchain<GrapheneTransactionRecord>(GrapheneMethods.get_recent_transaction_by_id, GrapheneApi.@public, txid);
		}

		/// <summary>	Gets a block. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="height">	The height. </param>
		///
		/// <returns>	The block. </returns>
		public GrapheneBlock GetBlock(ulong height)
		{
			return ApiCall<GrapheneBlock>(GrapheneMethods.get_block, GrapheneApi.@public, height);
		}

		/// <summary>	Limit orders. </summary>
		///
		/// <remarks>	Paul, 21/10/2015. </remarks>
		///
		/// <param name="quoteSymbol">	The quote symbol. </param>
		/// <param name="baseSymbol"> 	The base symbol. </param>
		/// <param name="limit">	  	The limit. </param>
		///
		/// <returns>	A GrapheneOrder[]. </returns>
		public GrapheneOrder[] LimitOrders(string baseSymbol, string quoteSymbol, int limit)
		{
			GrapheneAsset quote = GetAsset(quoteSymbol);
			GrapheneAsset @base = GetAsset(baseSymbol);

			GrapheneOrder[] orderbook = ApiCall<GrapheneOrder[]>(GrapheneMethods.get_limit_orders, GrapheneApi.@public, baseSymbol, quoteSymbol, limit);

			for (int i=0; i<limit; i++)
			{
				decimal bp0 = @base.GetAmountFromLarimers(orderbook[i].sell_price.@base.amount);
				decimal qp0 = quote.GetAmountFromLarimers(orderbook[i].sell_price.quote.amount);
				decimal bp1 = quote.GetAmountFromLarimers(orderbook[i+limit].sell_price.@base.amount);
				decimal qp1 = @base.GetAmountFromLarimers(orderbook[i+limit].sell_price.quote.amount);

				orderbook[i].price = qp0 / bp0;
				orderbook[i + limit].price = bp1 / qp1;
			}

			return orderbook;
		}

		/// <summary>	Gets account history. </summary>
		///
		/// <remarks>	Paul, 21/10/2015. </remarks>
		///
		/// <param name="account">	The account. </param>
		/// <param name="stop">   	The stop. </param>
		/// <param name="limit">  	The limit. </param>
		/// <param name="start">  	The start. </param>
		///
		/// <returns>	An array of graphene operation history item. </returns>
		public GrapheneOpContainer[] GetAccountHistory(GrapheneObject account, uint stop, uint start, uint limit)
		{
			GrapheneObject s = new GrapheneObject(1, 11, start);
			GrapheneObject e = new GrapheneObject(1, 11, stop);

			// fuck know why the parameter order is so messed up here
			return ApiCallBlockchain<GrapheneOpContainer[]>(GrapheneMethods.get_account_history, GrapheneApi.history, account, e, limit, s);
		}

		/// <summary>	Gets private key. </summary>
		///
		/// <remarks>	Paul, 26/10/2015. </remarks>
		///
		/// <param name="publicKey">	The public key. </param>
		///
		/// <returns>	The private key. </returns>
		public string GetPrivateKey(string publicKey)
		{
			return ApiCall<string>(GrapheneMethods.get_private_key, GrapheneApi.@public, publicKey);
		}

		/// <summary>	Query if 'name' is valid account name. </summary>
		///
		/// <remarks>	Paul, 20/10/2015. </remarks>
		///
		/// <param name="name">	The name. </param>
		///
		/// <returns>	true if valid account name, false if not. </returns>
		public static bool IsValidAccountName(string name)
		{
			string regExPattern = @"^([\-\.a-z0-9]+)$";
			return Regex.IsMatch(name, regExPattern) && name.Length <= kBitsharesMaxAccountNameLength;
		}

		/// <summary>	Gets all transfers. </summary>
		///
		/// <remarks>	Paul, 21/10/2015. </remarks>
		///
		/// <param name="t">	The GrapheneTransactionRecord to process. </param>
		///
		/// <returns>	all transfers. </returns>
		static public List<GrapheneOperation> GetAllTransfers(GrapheneTransactionRecord t)
		{
			List<GrapheneOperation> allTransfers = new List<GrapheneOperation>();

			foreach (var outer in t.operations)
			{
				IEnumerable<GrapheneOperation> ops = outer.Select<KeyValuePair<ulong, GrapheneOperation>, GrapheneOperation>(kvp => kvp.Value);

				IEnumerable<GrapheneOperation> transfers = ops.Where(o => o.from != null && o.to != null);

				allTransfers.AddRange(transfers);
			}

			return allTransfers;
		}
	}
}
