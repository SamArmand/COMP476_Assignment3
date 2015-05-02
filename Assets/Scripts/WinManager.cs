using UnityEngine;
using System.Collections;

public class WinManager : MonoBehaviour {

	private string playerName;
	private bool restart;
	private GameObject[] players;

	void OnGUI() {

		GUI.Label (new Rect (Screen.width / 3, Screen.height / 4, 300f, 150f), "" + playerName); 

		if(Network.isServer) {
			if(GUI.Button(new Rect(Screen.width/3, Screen.height/2, 200f, 50f), "Return to Lobby"))
				restart = true;
		}

		else if(Network.isClient) {
			GUI.Box(new Rect(Screen.width/2, Screen.height/2, 200f, 50f), "Waiting for Server");
		}
	}

	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag("Player");

        if (GameObject.FindGameObjectWithTag("Player1").GetComponent<Base_Player>().score > GameObject.FindGameObjectWithTag("Player2").GetComponent<Base_Player>().score)
            playerName = "Player 1 WINS!!";

        else if (GameObject.FindGameObjectWithTag("Player2").GetComponent<Base_Player>().score > GameObject.FindGameObjectWithTag("Player1").GetComponent<Base_Player>().score)
            playerName = "Player 2 WINS!!";

        //If none are found, then it's a tie game
        if (playerName == "")
			playerName = "TIE!!";

        GameObject.FindGameObjectWithTag("Player1").GetComponent<Base_Player>().score = 0;
        GameObject.FindGameObjectWithTag("Player2").GetComponent<Base_Player>().score = 0;
    }
	
	// Update is called once per frame
	void Update () {

		if(Network.isServer) {
			if(restart) {

				//Destroys all remaining players so duplicates are not present in next scene
				foreach(GameObject player in players) {
					Network.Destroy(player);
				}

				Application.LoadLevel (1);
				networkView.RPC ("loadLevel", RPCMode.Others, 1);
			}
		}
	}

	[RPC] void loadLevel(int i) {
		
		Application.LoadLevel (i);
	}
}
