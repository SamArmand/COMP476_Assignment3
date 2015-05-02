using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerDirectory : MonoBehaviour {

    private List<Player> mPlayerList = new List<Player>();

    public void Start()
    {

    }
    public void Update()
    {
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (mPlayerList.Count == 0)
            return;
        foreach (Player p in mPlayerList)
        {
            if (!p.IsDead)
            {
                return;
            }

        }
        GameObject.FindObjectOfType<Stats>().pelletCount = 0;
    }



    public void RegisterPlayer(Player p)
    {
        if(!mPlayerList.Contains(p))
        {
            mPlayerList.Add(p);
        }
    }

    public ICollection<Player> GetPlayers()
    {
        return mPlayerList;
    }
}
