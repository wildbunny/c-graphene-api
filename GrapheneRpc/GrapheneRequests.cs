using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapheneRpc
{
	public enum GrapheneMethods
	{
		//
		// web wallet commands
		//
		 
		cancel_all_subscriptions,
		get_account_balances,
		get_account_by_name,
		get_account_count,
		get_account_references,
		get_accounts,
		get_assets,
		get_balance_objects,
		get_blinded_balances,
		get_block,
		get_block_header,
		get_call_orders,
		get_chain_id,
		get_chain_properties,
		get_committee_member_by_account,
		get_committee_members,
		get_config,
		get_dynamic_global_properties,
		get_full_accounts,
		get_global_properties,
		get_key_references,
		get_limit_orders,
		get_margin_positions,
		get_named_account_balances,
		get_objects,
		get_potential_address_signatures,
		get_potential_signatures,
		get_proposed_transactions,
		get_required_fees,
		get_required_signatures,
		get_settle_orders,
		get_transaction,
		get_transaction_hex,
		get_vested_balances,
		get_vesting_balances,
		get_witness_by_account,
		get_witness_count,
		get_witnesses,
		get_workers_by_account,
		list_assets,
		lookup_account_names,
		lookup_accounts,
		lookup_asset_symbols,
		lookup_committee_member_accounts,
		lookup_vote_ids,
		lookup_witness_accounts,
		set_block_applied_callback,
		set_pending_transaction_callback,
		set_subscribe_callback,
		subscribe_to_market,
		unsubscribe_from_market,
		validate_transaction,
		verify_account_authority,
		verify_authority,

		//
		// full node commands
		//

		help,
		gethelp,
		info,
		login,
		begin_builder_transaction,
		add_operation_to_builder_transaction,
		replace_operation_in_builder_transaction,
		set_fees_on_builder_transaction,
		preview_builder_transaction,
		sign_builder_transaction,
		propose_builder_transaction,
		remove_builder_transaction,
		is_new,
		is_locked,
		@lock,
		unlock,
		set_password,
		dump_private_keys,
		list_my_accounts,
		list_accounts,
		list_account_balances,
		import_key,
		import_accounts,
		import_account_keys,
		import_balance,
		suggest_brain_key,
		register_account,
		upgrade_account,
		create_account_with_brain_key,
		sell_asset,
		borrow_asset,
		transfer,
		create_asset,
		update_asset,
		update_bitasset,
		update_asset_feed_producers,
		publish_asset_feed,
		issue_asset,
		issue_asset2,
		get_asset,
		get_bitasset_data,
		fund_asset_fee_pool,
		reserve_asset,
		reserve_asset2,
		global_settle_asset,
		settle_asset,
		whitelist_account,
		create_committee_member,
		get_witness,
		get_committee_member,
		list_witnesses,
		list_committee_members,
		create_witness,
		update_witness,
		withdraw_vesting,
		vote_for_committee_member,
		vote_for_witness,
		set_voting_proxy,
		set_desired_witness_and_committee_member_count,
		get_account,
		get_account_id,
		get_account_history,
		get_market_history,
		get_object,
		get_private_key,
		load_wallet_file,
		normalize_brain_key,
		save_wallet_file,
		serialize_transaction,
		sign_transaction,
		get_prototype_operation,
		propose_parameter_change,
		propose_fee_change,
		approve_proposal,
		dbg_make_uia,
		dbg_make_mia,
		flood_network,
		network_add_nodes,
		network_get_connected_peers,
		set_key_label,
		get_key_label,
		get_public_key,
		get_blind_accounts,
		get_my_blind_accounts,
		get_blind_balances,
		create_blind_account,
		transfer_to_blind,
		transfer_from_blind,
		blind_transfer,
		blind_history,
		receive_blind_transfer,
		get_recent_transaction_by_id,
		transfer2,
		
		history,
		database,
		network_broadcast,
		network_node
	}

	public enum GrapheneApi
	{
		@public,
		login,
		history,
		database,
		network_broadcast,
		network_node
	}

	/// <summary>
	/// {"jsonrpc": "2.0", "method": "get_accounts", "params": [["1.2.0"]], "id": 1}
	/// </summary>
	public class GrapheneRequest
	{
		public decimal jsonrpc;
		public int id;
		public GrapheneMethods method;
		public object[] @params;

		public GrapheneRequest(GrapheneMethods method, params object[] @params)
		{
			this.jsonrpc = 2;
			this.id = 1;
			this.method = method;
			this.@params = @params;
		}
	}

	public class GrapheneSocketRequest
	{
		readonly public int id;
		readonly public string method;
		public object[] @params;

		public GrapheneSocketRequest(GrapheneMethods method, int id, int api, params object[] @params)
		{
			this.id = id;
			this.method = "call";

			object[] fudgeParams = { api, method, @params };

			this.@params = fudgeParams;
		}
	}
}
