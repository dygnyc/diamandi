using Godot;
using System;

public class MobTimer : Timer
{

    float decrementAmount = .001f;
    float waitTime;
    float waitTimeFloor = .15f; //lowest wait time
    float startWaitTime;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        startWaitTime = this.GetWaitTime();
        waitTime = startWaitTime;

    }
    public void _on_Player_Hit()
    {
        ResetWaitTime();
    }

    public void ResetWaitTime()
    {
        waitTime = startWaitTime;
    }


    public void _on_MobTimer_timeout()
    {
        //GD.Print(waitTime);

        if (waitTime > waitTimeFloor)
        {
            waitTime -= decrementAmount;
            //decrease waitTime
            //set new waitTime
            this.SetWaitTime(waitTime);
        }

    }


}
