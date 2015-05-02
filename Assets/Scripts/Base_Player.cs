using UnityEngine;
using System.Collections;

public class Base_Player : MonoBehaviour {

	public string playerName;// { get; set; }
	public int playerNumber { get; set; }

    public int score { get; set; }

    public GameObject player;
	private GameObject newPlayer;

	private Vector3[] spawnLocations = { new Vector3(12.953f,0.53f,-16.786f),
										new Vector3(13.953f,0.53f,-16.786f)
										};

	public GameObject[] bombermanPrefab = new GameObject[4];

	// Use this for initialization
	void Awake() {
		DontDestroyOnLoad (this);

		if(Network.isServer) {
			playerNumber = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ().playerCount;
			networkView.RPC("setPlayerNumber", RPCMode.OthersBuffered, playerNumber);

			this.tag = "Player" + playerNumber;
			networkView.RPC ("setTag", RPCMode.OthersBuffered, playerNumber, this.tag);
		}
	}

	void Start() {
		
		if(networkView.isMine) {
			playerName = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ().playerName;
			networkView.RPC("setName", RPCMode.OthersBuffered, playerName);
		}
	}

	void OnLevelWasLoaded(int level) {

		//on level loaded, spawn player gameobject and make it a child of this
		if (level == 2 && networkView.isMine) {
			newPlayer = Network.Instantiate (player, spawnLocations[playerNumber-1], Quaternion.identity, 0) as GameObject;
			newPlayer.GetComponent<Player>().pacmanPrefab = bombermanPrefab[playerNumber-1];

			newPlayer.transform.parent = this.transform;
			networkView.RPC("setParent", RPCMode.Others, newPlayer.networkView.viewID);
		}
	}

	[RPC] void setParent(NetworkViewID viewID) {
		NetworkView.Find (viewID).transform.parent = this.transform;
	}

	[RPC] void setName(string s) {
		playerName = s;
	}

	[RPC] void setTag(int i, string s) {
		playerNumber = i;
		this.tag = s;
	}

	[RPC] void setPlayerNumber(int i) {
		playerNumber = i;
	}

}
