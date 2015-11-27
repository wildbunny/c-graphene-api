using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapheneRpc
{
    public class GrapheneError
	{
		public string name;
		public string message;
		public string detail;
		public int code;
	}

	/*
	 *		"NULL"                           :  "1.0.%d",
            "BASE"                           :  "1.1.%d",
            "ACCOUNT"                        :  "1.2.%d",
            "FORCE_SETTLEMENT"               :  "1.3.%d",
            "ASSET"                          :  "1.4.%d",
            "DELEGATE"                       :  "1.5.%d",
            "WITNESS"                        :  "1.6.%d",
            "LIMIT_ORDER"                    :  "1.7.%d",
            "CALL_ORDER"                     :  "1.8.%d",
            "CUSTOM"                         :  "1.9.%d",
            "PROPOSAL"                       :  "1.10.%d",
            "OPERATION_HISTORY"              :  "1.11.%d",
            "WITHDRAW_PERMISSION"            :  "1.12.%d",
            "VESTING_BALANCE"                :  "1.13.%d",
            "WORKER"                         :  "1.14.%d",
            "BALANCE"                        :  "1.15.%d",
            "GLOBAL_PROPERTY"                :  "2.0.%d",
            "DYNAMIC_GLOBAL_PROPERTY"        :  "2.1.%d",
            "INDEX_META"                     :  "2.2.%d",
            "ASSET_DYNAMIC_DATA"             :  "2.3.%d",
            "ASSET_BITASSET_DATA"            :  "2.4.%d",
            "DELEGATE_FEEDS"                 :  "2.5.%d",
            "ACCOUNT_BALANCE"                :  "2.6.%d",
            "ACCOUNT_STATISTICS"             :  "2.7.%d",
            "ACCOUNT_DEBT"                   :  "2.8.%d",
            "TRANSACTION"                    :  "2.9.%d",
            "BLOCK_SUMMARY"                  :  "2.10.%d",
            "ACCOUNT_TRANSACTION_HISTORY"    :  "2.11.%d",
            "WITNESS_SCHEDULE"               :  "2.12.%d",
	 */

	public enum GrapheneObjectTypes
	{
		@null=0,
		@base,
		account,
		force_settlement,
		asset,
		@delegate,
		witness,
		limit_order,
		call_order,
		custom,
		proposal,
		operation_history,
		withdraw_permission,
		vesting_balance,
		worker,
		balance,

		MIDDLE,
		
		global_property,
		dynamic_global_property,
		index_meta,
		asset_dynamic_data,
		asset_bitasset_data,
		delegate_feeds,
		account_balance,
		account_statistics,
		account_debt,
		transaction,
		block_summary,
		account_transaction_history,
		witness_schedule,
	}

	public class GraphenePermission
	{
		public int weight_threshold;
		public Dictionary<string, int>[] account_auths;
		public Dictionary<string, int>[] key_auths;
		public Dictionary<string, int>[] address_auths;
	}

	public class GrapheneAccountOptions
	{
		public string memo_key;
		public GrapheneObject voting_account;
		public int num_witness;
		public int num_committee;
	}

	/// <summary>	A graphene account. </summary>
	///		
	///		{id:1.2.29495,
	///		membership_expiration_date:1970-01-01T00:00:00,
	///		registrar:1.2.4,
	///		referrer:1.2.0,
	///		lifetime_referrer:1.2.0,
	///		network_fee_percentage:2000,
	///		lifetime_referrer_fee_percentage:3000,
	///		referrer_rewards_percentage:0,
	///		name:monsterer,
	///		owner:
	///		{
	///			weight_threshold:1,
	///			account_auths:[],
	///			key_auths:[[BTS8RwSWSFcnzWLBfXuvG4XkqSqeuDaqMzoMr6V5LRX2ietxF8APy,1]],
	///			address_auths:[]
	///		},
	///		active:
	///		{
	///			weight_threshold:2,
	///			account_auths:[[1.2.31211,1]],
	///			key_auths:[[BTS8RwSWSFcnzWLBfXuvG4XkqSqeuDaqMzoMr6V5LRX2ietxF8APy,1]],
	///			address_auths:[]
	///		},
	///		options:
	///		{
	///			memo_key:BTS8RwSWSFcnzWLBfXuvG4XkqSqeuDaqMzoMr6V5LRX2ietxF8APy,
	///			voting_account:1.2.5,
	///			num_witness:0,
	///			num_committee:0,
	///			votes:[],
	///			extensions:[]
	///		},
	///		statistics:2.6.29495,
	///		whitelisting_accounts:[],
	///		blacklisting_accounts:[],
	///		blacklisted_accounts:[]
	///		}
	///
	/// 
	/// <remarks>	Paul, 19/10/2015. </remarks>
	public class GrapheneAccount
	{
		public GrapheneObject id;
		public DateTime membership_expiration_date;
		public GrapheneObject registrar;
		public GrapheneObject referrer;
		public GrapheneObject lifetime_referrer;
		public int network_fee_percentage;
		public int lifetime_referrer_fee_percentage;
		public int referrer_rewards_percentage;
		public string name;
		public GraphenePermission owner;
		public GraphenePermission active;
		public GrapheneAccountOptions options;
		public GrapheneObject statistics;
		public GrapheneObject[] whitelisting_accounts;
		public GrapheneObject[] blacklisting_accounts;
		public GrapheneObject[] blacklisted_accounts;
	}

	/// <summary>	A graphene object. </summary>
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneObject
	{
		GrapheneObjectTypes m_type;
		uint m_index;
		string m_raw;

		public GrapheneObject(string descriptor)
		{
			m_raw = descriptor;
			string[] parts = descriptor.Split('.');

			if (parts.Length == 3)
			{
				m_type = (GrapheneObjectTypes)Enum.Parse(typeof(GrapheneObjectTypes), parts[1]);

				if (parts[0] == "2")
				{
					m_type = (GrapheneObjectTypes)((uint)m_type + (int)GrapheneObjectTypes.MIDDLE + 1);
				}

				m_index = uint.Parse(parts[2]);
			}
		}

		public GrapheneObject(uint a, uint b, uint c) : this(a + "." + b + "." + c) { }

		public override string ToString()
		{
			return m_raw;
		}

		public uint GetIndex() { return m_index; }

		public override bool Equals(object obj)
		{
			return this.ToString() == obj.ToString();
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public static bool operator == (GrapheneObject a, GrapheneObject b)
		{
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator != (GrapheneObject a, GrapheneObject b)
		{
			return !(a==b);
		}
	}

	/// <summary>	A graphene asset quantity. </summary>
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneAmount
	{
		public ulong amount;
		public GrapheneObject asset_id;
	}

	/// <summary>	A grpahene exchange rate. </summary>
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneExchangeRate
	{
		public GrapheneAmount @base;
		public GrapheneAmount quote;
	}

	/// <summary>	A graphene asset. </summary>
	///
	/// {
	///		id:1.3.0,
	///		symbol:BTS,
	///		precision:5,
	///		issuer:1.2.3,
	///		options:
	///		{
	///			max_supply:360057050210207,
	///			market_fee_percent:0,
	///			max_market_fee:1000000000000000,
	///			issuer_permissions:0,
	///			flags:0,
	///			core_exchange_rate:
	///			{
	///				base:{amount:1,asset_id:1.3.0},
	///				quote:{amount:1,asset_id:1.3.0}
	///			},
	///			whitelist_authorities:[],
	///			blacklist_authorities:[],
	///			whitelist_markets:[],
	///			blacklist_markets:[],
	///			description:,
	///			extensions:[]
	///		},
	///		dynamic_asset_data_id:2.3.0
	///	}
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneAsset
	{
		public GrapheneObject id;
		public string symbol;
		public ulong precision;
		public GrapheneObject issuer;
		public GrapheneAssetOptions options;
		public GrapheneObject dynamic_asset_data_id;

		public decimal GetPrecisionDividor()
		{
			return (decimal)Math.Pow(10, precision);
		}

		public decimal GetAmountFromLarimers(ulong larmiers)
		{
			return (decimal)larmiers / GetPrecisionDividor();
		}

		public ulong GetLarimersFromAmount(decimal amount)
		{
			return (ulong)(GetPrecisionDividor() * amount);
		}

		public decimal Truncate(decimal amount)
		{
			decimal div = GetPrecisionDividor();
			return (ulong)(Math.Max(amount, 0) * div) / div;
		}

		public bool IsUia()
		{
			return this.issuer.GetIndex() > 3;
		}
	}

	/// <summary>	A graphene asset options. </summary>
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneAssetOptions
	{
		public ulong max_supply;
		public decimal market_fee_percent;
		public ulong max_market_fee;
		public uint issuer_permissions;
		public uint flags;
		public GrapheneExchangeRate core_exchange_rate;
		public GrapheneObject[] whitelist_authorities;
		public GrapheneObject[] blacklist_authorities;
		public GrapheneObject[] whitelist_markets;
		public GrapheneObject[] blacklist_markets;
		public string description;
	}

	/// <summary>	A graphene memo. </summary>
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneMemo
	{
		public string from;
		public string to;
		public ulong nonce;
		public string message;
	}

	/// <summary>	A graphene operation. </summary>
	///
	///		{
	///        "fee": 
	///        {
	///          "amount": 2089843,
	///          "asset_id": "1.3.0"
	///        },
	///        "from": "1.2.17",
	///        "to": "1.2.7",
	///        "amount": 
	///        {
	///          "amount": 10000000,
	///          "asset_id": "1.3.0"
	///        },
	///        "memo": 
	///        {
	///          "from": "GPH6MRyAjQq8ud7hVNYcfnVPJqcVpscN5So8BhtHuGYqET5GDW5CV",
	///          "to": "GPH6MRyAjQq8ud7hVNYcfnVPJqcVpscN5So8BhtHuGYqET5GDW5CV",
	///          "nonce": "16430576185191232340",
	///          "message": "74d0e455e2e5587b7dc85380102c3291"
	///        },
	///        "extensions": []
	///      }
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneOperation
	{
		public GrapheneAmount fee;
		public GrapheneAmount amount;
		public GrapheneObject from;
		public GrapheneObject to;
		public GrapheneObject publisher;
		//public GrapheneObject asset_id;
		public GrapheneMemo memo;
	}

	/// <summary>	Information about the graphene transaction. </summary>
	///
	/// {
	///  "ref_block_num": 18,
	///  "ref_block_prefix": 2320098938,
	///  "expiration": "2015-10-13T13:56:15",
	///  "operations": [[
	///      0,{
	///        "fee": {
	///          "amount": 2089843,
	///          "asset_id": "1.3.0"
	///        },
	///        "from": "1.2.17",
	///        "to": "1.2.7",
	///        "amount": {
	///          "amount": 10000000,
	///          "asset_id": "1.3.0"
	///        },
	///        "memo": {
	///          "from": "GPH6MRyAjQq8ud7hVNYcfnVPJqcVpscN5So8BhtHuGYqET5GDW5CV",
	///          "to": "GPH6MRyAjQq8ud7hVNYcfnVPJqcVpscN5So8BhtHuGYqET5GDW5CV",
	///          "nonce": "16430576185191232340",
	///          "message": "74d0e455e2e5587b7dc85380102c3291"
	///        },
	///        "extensions": []
	///      }
	///    ]
	///  ],
	///  "extensions": [],
	///  "signatures": [
	///    "1f147aed197a2925038e4821da54bd7818472ebe25257ac9a7ea66429494e7242d0dc13c55c6840614e6da6a5bf65ae609a436d13a3174fd12f073550f51c8e565"
	///  ]
	/// }
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneTransactionRecord
	{
		public ulong ref_block_num;
		public ulong ref_block_prefix;
		public DateTime expiration;
		public Dictionary<ulong, GrapheneOperation>[] operations;
		public string[] signatures;
		public int trx_in_block;
		public ulong block_num;
	}

	/// <summary>	Information about the graphene. </summary>
	///
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneInfo
	{
		public ulong head_block_num;
		public string head_block_id;
		public string head_block_age;
		public string next_maintenance_time;
		public string chain_id;
		public decimal participation;
		public string[] active_witnesses;
		public string[] active_committee_members;
	}

	/// <summary>	A graphene block. </summary>
	///
	/// {
	///		previous:000309e96012e54e43b6d9e3aa8ada24137b6f48,
	///		timestamp:2015-10-20T15:49:06,
	///		witness:1.6.21,
	///		transaction_merkle_root:0000000000000000000000000000000000000000,
	///		extensions:[],
	///		witness_signature:20383eea8fb3e0ede4fa99c95d436253aaa24ac35030292ce10d92978f29498bd6653a81e2f3c97ed8d6ee21307e36fe25fb08a0003a1dd1af0d3a27cded45635f,
	///		transactions:[],
	///		block_id:000309ea2fc1134ea5145b1208f1a6870f9fc9a0,
	///		signing_key:BTS5ARpXg6yWZptHgik721dyXek57v53op6XT7rwTj7Kmb8CGwWEM
	///	}
	/// <remarks>	Paul, 20/10/2015. </remarks>
	public class GrapheneBlock
	{
		public string previous;
		public DateTime timestamp;
		public GrapheneObject witness;
		public string transaction_merkle_root;
		public string witness_signature;
		public GrapheneTransactionRecord[] transactions;
		public string block_id;
		public string signing_key;
		public string[] transaction_ids;
	}

	/// <summary>	A graphene order. </summary>
	///
	///	{
	///   	"id": "1.7.1148",
	///    "expiration": "2020-10-20T17:33:00",
	///    "seller": "1.2.18208",
	///    "for_sale": 8033,
	///    "sell_price": {
	///      "base": {
	///        "amount": 18533,
	///        "asset_id": "1.3.121"
	///      },
	///      "quote": {
	///        "amount": 49112450,
	///        "asset_id": "1.3.0"
	///      }
	///    }
	/// <remarks>	Paul, 21/10/2015. </remarks>
	public class GrapheneOrder
	{
		public GrapheneObject id;
		public DateTime expiration;
		public GrapheneObject seller;
		public ulong for_sale;
		public GrapheneExchangeRate sell_price;
		public decimal GetPrice(bool bid, decimal ratio)
		{
			if (bid)
			{ 
				return (decimal)(sell_price.@base.amount/ratio) / (decimal)sell_price.quote.amount; 
			}
			else
			{ 
				return (decimal)(sell_price.quote.amount/ratio) / (decimal)sell_price.@base.amount; 
			}
		}
		public decimal price;
	}

	public class GrapheneOpContainer
	{
		public GrapheneObject id;
		public Dictionary<int, GrapheneOperation> op;
		public Dictionary<int, GrapheneObject> result;
		public ulong block_num;
		public ulong trx_in_block;
		public ulong op_in_trx;
		public ulong virtual_op;
	}

	public class GrapheneOperationHistoryItem
	{
		public string memo;
		public string description;
		public GrapheneOpContainer op;
	}

	public class GrapheneApiIndex
	{
		public string name;
		public int data;
	}

	/// <summary>	A graphene dynamic asset data. </summary>
	/// {
	///		"id": "2.3.416",
	///    "current_supply": 2158569,
	///    "confidential_supply": 0,
	///    "accumulated_fees": 0,
	///    "fee_pool": 0
	///   }
	///
	/// <remarks>	Paul, 24/10/2015. </remarks>
	public class GrapheneDynamicAssetData
	{
		public GrapheneObject id;
		public ulong current_supply;
		public ulong confidential_supply;
		public ulong accumulated_fees;
		public ulong fee_pool;
	}
}
