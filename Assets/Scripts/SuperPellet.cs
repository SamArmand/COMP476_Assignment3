using UnityEngine;
using System.Collections;

public class SuperPellet : MonoBehaviour {
    private CooldownTimer mTimer = new CooldownTimer();
    public float Cooldown = 0.25f;
    bool isRendererActive = true;
	// Use this for initialization
	void Start () {
        mTimer.StartTimer(Cooldown);
	}   
	
	// Update is called once per frame
	void Update () {
        mTimer.Update();
        if (mTimer.IsTimerFinished())
        {
            Flash();
            mTimer.StartTimer(Cooldown);
        }
	}

    private void Flash()
    {
        this.renderer.enabled = isRendererActive;
        isRendererActive = !isRendererActive;
    }
}
