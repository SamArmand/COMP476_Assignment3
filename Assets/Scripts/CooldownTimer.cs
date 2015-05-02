using UnityEngine;
using System.Collections;

public class CooldownTimer
{

    private float mTimeLeft = -1;
    private bool mIsPaused = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

        if (!IsTimerFinished() && !mIsPaused)
            UpdateTimer();
    }

    public void StartTimer(float time)
    {
        mTimeLeft = time;
        mIsPaused = false;
    }

    public float TimeLeft()
    {
        return mTimeLeft;
    }

    public bool IsTimerFinished()
    {
        return mTimeLeft < 0;
    }

    public void PauseTimer()
    {
        mIsPaused = true;
    }

    public void ResumeTimer()
    {
        mIsPaused = false;
    }

    private void UpdateTimer()
    {
        mTimeLeft -= Time.deltaTime;
    }


}
