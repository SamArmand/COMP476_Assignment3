using UnityEngine;
using System.Collections;
using System.Timers;

public class Player : MonoBehaviour {

	private float speed = 5f;

	public GameObject pacmanPrefab { get; set; }

	private GameObject pacman;
	private GameObject playerCamera;
    private Timer timer;
    public AudioClip PelletEatingSoundEffect;
    public AudioClip SuperPelletEatingSoundEfect;
    private Vector3 mStartingPosition;

    private PlayerDirectory mPlayerDirectory;
    public bool IsDead {get;set;}
    
	//Synchronizes position and rotation of player
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {

		Vector3 syncPos = Vector3.zero;
		Quaternion syncRot = Quaternion.identity;

		if(stream.isWriting) {				//writing to stream
			syncPos = rigidbody.position;
			stream.Serialize(ref syncPos);

			syncRot = rigidbody.rotation;
			stream.Serialize(ref syncRot);
		}

		if(stream.isReading) {				//reading from stream
			stream.Serialize(ref syncPos);
			rigidbody.position = syncPos;

			stream.Serialize(ref syncRot);
			rigidbody.rotation = syncRot;
		}
	}
	
	// Use this for initialization
	void Start () {

        timer = new Timer();
        
        timer.AutoReset = true;
        timer.Stop();
        timer.Interval = 10000;
        timer.Elapsed += ResetSpeed;
        mPlayerDirectory = GameObject.Find("PlayerDirectory").GetComponent<PlayerDirectory>();
        IsDead = false;
         
        //Register this player to the directory so it can be accessed by the ghosts
        mPlayerDirectory.RegisterPlayer(this);


		if (networkView.isMine) {

			pacman = Network.Instantiate(pacmanPrefab, this.transform.position, Quaternion.identity, 0) as GameObject;

			pacman.transform.parent = this.transform;
			networkView.RPC("setParent", RPCMode.OthersBuffered, pacman.networkView.viewID);

            GameObject[] pacmen = GameObject.FindGameObjectsWithTag("pacman");

            foreach(GameObject p in pacmen)
            {
                if (p != pacman)
                {
                    Physics.IgnoreCollision(p.GetComponent<CapsuleCollider>(), pacman.GetComponent<CapsuleCollider>());
                }
            }

		}

        mStartingPosition = transform.position;

	}

	// Update is called once per frame
	void Update () {
        if (IsDead)
        {
            IsDead = false;
        }
		if (networkView.isMine) {

            inputMovement();

            if (transform.position.x < -0.5f)
            {
                transform.position = new Vector3(27.5f, transform.position.y, transform.position.z);
            }

            else if (transform.position.x > 28)
            {
                transform.position = new Vector3(-0.5f, transform.position.y, transform.position.z);
            }
        }
	}

    private void ResetSpeed(object sender, ElapsedEventArgs e)
    {
        speed = 5f;
        timer.Stop();
    }

    public void KillPlayer()
    {
        IsDead = true;
            transform.position = mStartingPosition;
    }

    private void inputMovement() {

		//Move up
		if (Input.GetKey(KeyCode.W)) {
			this.rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
			this.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		}

		//Move Down
		else if (Input.GetKey(KeyCode.S)) {
			this.rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
			this.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
		}

		//Move Right
		else if (Input.GetKey (KeyCode.D)) {
			this.rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
			this.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
		}

		//Move Left
		else if (Input.GetKey(KeyCode.A)) {
			this.rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
			this.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
		}

	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "pellet")
        {
            GameObject.Destroy(collision.gameObject);
            gameObject.GetComponentInParent<Base_Player>().score++;

            GameObject.FindObjectOfType<Stats>().pelletCount--;
            AudioSource.PlayClipAtPoint(PelletEatingSoundEffect, transform.position);
        }

        else if (collision.gameObject.tag == "super pellet")
        {
            GameObject.Destroy(collision.gameObject);
            gameObject.GetComponentInParent<Base_Player>().score++;
            speed = 10f;
            timer.Stop();
            timer.Start();

            GameObject.FindObjectOfType<Stats>().pelletCount--;
            AudioSource.PlayClipAtPoint(SuperPelletEatingSoundEfect, transform.position);

        }
        
    }

    [RPC] void setParent(NetworkViewID viewID) {
		NetworkView.Find (viewID).transform.parent = this.transform;	
	}

}
