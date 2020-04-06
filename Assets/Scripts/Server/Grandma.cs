using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : CookieProcess
{
    private ProcessData processData;

    public override void Init(ProcessData data)
    {
        processData = data;
    }

    public override float GetTickTime()
    {
        return 1.0f;
    }

    public override void Tick(int tickCount, UserData userData)
    {
        userData.cookieCount += tickCount * 5; //TODO - make config class, and put rate there, as well as upgrades
    }
}
