using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stats : MonoBehaviour {

	public Texture2D[] texture = new Texture2D[4];

	private float timer;

	private List<GameObject> players = new List<GameObject> ();

	private Color[] color = { Color.white, Color.blue, Color.red, Color.black };

	private Rect[] nameRect = { new Rect(35f, 80f, 200f, 30f),
								new Rect(825f, 80f, 200f, 30f)
							};

    public int pelletCount;

	void OnGUI() {

		for(int i=0; i<players.Count; i++) {
			GUI.color = color[i];
			GUI.Label(nameRect[i], players[i].GetComponent<Base_Player>().playerName + ": " + players[i].GetComponent<Base_Player>().score);
			GUI.color = Color.white;
		}
	}

	// Use this for initialization
	void Start () {

        pelletCount = 312;

		timer = 2f;

		for(int i=1; i<3; i++) {
			if(GameObject.FindGameObjectWithTag ("Player"+i) != null) {
				players.Add(GameObject.FindGameObjectWithTag ("Player"+i));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(Network.isServer) {
            if (pelletCount == 0)
            {
                gameOver();
            }

		}
	}

	private void gameOver() {
		timer -= Time.deltaTime;

		if (timer <= 0) {
			Application.LoadLevel (3);
			networkView.RPC ("loadLevel", RPCMode.Others, 3);
		}
	}

	[RPC] void loadLevel(int i) {
		
		Application.LoadLevel (i);
	}
}
