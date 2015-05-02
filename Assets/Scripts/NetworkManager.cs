using UnityEngine;
using System.Collections;

//172.31.36.176 is the concordia ip address for local connection

public class NetworkManager : MonoBehaviour
{
	public GameObject lobbyPlayerPrefab;
	public string playerName { get; set; }
	
	public int playerCount { get; set; }

	private bool isChoice;
	private bool isOnline;
	private bool first;

	//Online Multiplayer Variables
	private const string typeName = "COMP476Network";
	private string gameName;
	private HostData[] hostList;						//list of servers

	//Local Multiplayer Variables
	private int portNumber;
	private string ipAddress;
	private string portNumber_string;

	void Awake() {
		DontDestroyOnLoad (this);
	}

	void Start() {
		ipAddress = "Enter IP Address";			//Put IPv4 address here
		portNumber_string = "35000";

		playerCount = 1;
		playerName = "Enter Player Name Here";
		gameName = "Enter Server Name Here";

		first = true;
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if(isChoice == false) {

				//playerName = GUI.TextField(new Rect(100,50,250,30),playerName);

				if(GUI.Button(new Rect(100, 100, 250, 30), "Online Multiplayer")) {
					isChoice = true;
					isOnline = true;
				}
				
				if(GUI.Button(new Rect(100,150,250,30),"Local Multiplayer")) {
					isChoice = true;
					isOnline = false;
				}
			}
			
			//Online Multiplayer Menu
			if(isChoice && isOnline) {

				playerName = GUI.TextField(new Rect(100,50,250,30),playerName);

				gameName = GUI.TextField(new Rect(100,100, 250, 30), gameName);

				if (GUI.Button(new Rect(100, 150, 250, 30), "Start Server"))
					StartOnlineServer();
				
				if (GUI.Button(new Rect(100, 200, 250, 50), "Refresh Hosts"))
					RefreshHostList();
				
				if (hostList != null)
				{
					for (int i = 0; i < hostList.Length; i++)
					{
						if (GUI.Button(new Rect(400, 100 + (80 * i), 250, 50), hostList[i].gameName))
							JoinServer(hostList[i]);
					}
				}
			}

			//Local Multiplayer Menu
			else if(isChoice && isOnline == false) {

				playerName = GUI.TextField(new Rect(100,50,250,30),playerName);

				if (GUI.Button(new Rect(100, 100, 250, 30), "Start Server"))
					StartLocalServer();
				
				ipAddress = GUI.TextField(new Rect(100,150,250,30),ipAddress);
				
				portNumber_string = GUI.TextField(new Rect(100,200,250,30),portNumber_string);
				
				if(GUI.Button(new Rect(100,250,250,30), "Join"))
					JoinIP(ipAddress,portNumber_string);
			}
			
		}
	}

	//Server Functions
	public void StartOnlineServer()
	{
		Debug.Log ("Online Server created.");
		
		Network.InitializeServer(4, 35000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	
	public void StartLocalServer()
	{
		Debug.Log ("Local Server created.");
		
		Network.InitializeServer(4, 35000, !Network.HavePublicAddress());
	}
	
	void OnServerInitialized()
	{
		Application.LoadLevel(1);
	}


	//Client Functions
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	public void JoinServer(HostData hostData)
	{
		Debug.Log ("Joining Server.");
		Network.Connect(hostData);
	}

	public void JoinIP(string ip, string port) {
		portNumber = int.Parse(portNumber_string);
		Network.Connect (ip, portNumber);
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Connected to server.");

		Network.SetSendingEnabled(0, false);	
		Network.isMessageQueueRunning = false; //disabled because it's not in correct scene yet.

		Application.LoadLevel (1);
	}
	
	//spawns player after level is loaded
	void OnLevelWasLoaded(int level) {
		Debug.Log ("Level done loading. Spawning player");
		if(level == 1 && first) {
			SpawnLobbyPlayer ();
			first = false;
		}

		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);	
	}

	private void SpawnLobbyPlayer()
	{
		Network.Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity, 0);
	}

	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log ("Player " + (++playerCount) + " connected from " +
		           player.ipAddress + ":" + player.port);

		networkView.RPC ("setPlayerCount", RPCMode.OthersBuffered, playerCount);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			Debug.Log ("Local server connection lost");
		} 
		else {
			if(info == NetworkDisconnection.LostConnection) {
				Debug.Log ("Lost connection to the server");
			}
			else {
				Debug.Log ("Successfully disconnected from the server");
			}
		}
	}

	[RPC] void setPlayerCount(int i) {
		playerCount = i;
	}
}
