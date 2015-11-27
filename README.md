This is a c# websocket API for the graphene blockchains


Requirements:

* cli_wallet
* witness_node

* cli_wallet must be unlocked

Useage:

var cliWallet = "ws://127.0.0.1:8091";
var witnessNode = "ws://127.0.0.1:8090";
string[] both = {cliWallet, witnessNode};

var wallet = new GrapheneWallet(string.Join(",", both), "*", "*");

(A)Synchronus generic api call:

public async Task&lt;T&gt; ApiCallAsync&lt;T&gt;(GrapheneMethods method, GrapheneApi api, params object[] args)

public T ApiCall&lt;T&gt;(GrapheneMethods method, GrapheneApi api, params object[] args)

GrapheneWallet provides many specialised API calls:

public GrapheneAccount GetAccount(string account)
public GrapheneAsset GetAsset(string symbol)
public Dictionary<string, GrapheneTransactionRecord> Transfer(string from, string to, decimal amount, string symbol, string memo = "")
....

Decrypt memo:

static public string Decrypt(GrapheneMemo memo, KeyPair receiverKeyPair)


Enjoy!
