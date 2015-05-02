using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Ghost : MonoBehaviour {

    private NavMeshAgent mNavAgent;
    private CapsuleCollider mCollider;
    private PlayerDirectory mPlayerDirectory;
	// Use this for initialization
	void Start () {
        mNavAgent = GetComponent<NavMeshAgent>();
        mCollider = this.GetComponent<CapsuleCollider>();
        mPlayerDirectory = GameObject.Find("PlayerDirectory").GetComponent<PlayerDirectory>();
        
    }
	
	// Update is called once per frame
    void Update()
    {
        Player nearestPlayer = GetNearestPlayer();
        if (nearestPlayer != null)
        {

            if (IsPlayerHit(nearestPlayer)) {
                nearestPlayer.KillPlayer();
            }
            else {
                mNavAgent.SetDestination(nearestPlayer.transform.position);
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {

        Vector3 syncPos = Vector3.zero;
        Quaternion syncRot = Quaternion.identity;

        if (stream.isWriting)
        {				//writing to stream
            syncPos = transform.position;
            stream.Serialize(ref syncPos);

            syncRot = transform.rotation;
            stream.Serialize(ref syncRot);
        }

        if (stream.isReading)
        {				//reading from stream
            stream.Serialize(ref syncPos);
            transform.position = syncPos;

            stream.Serialize(ref syncRot);
            transform.rotation = syncRot;
        }
    }


    private Player GetNearestPlayer()
    {
        if (mPlayerDirectory == null)
        {
            mPlayerDirectory = GameObject.Find("PlayerDirectory").GetComponent<PlayerDirectory>();
        }
        ICollection<Player> players = mPlayerDirectory.GetPlayers();

        //impossible distance
        float distance = 10000;
        Player nearestPlayer = null;
        foreach (Player p in players)
        {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < distance)
            {
                distance = d;
                nearestPlayer = p;
            }
        }
        return nearestPlayer;
    }
    private bool IsPlayerHit(Player player)
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        return distance < mCollider.radius;
            
    }

    
}
