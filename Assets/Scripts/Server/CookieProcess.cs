using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for a timed powerup
public abstract class CookieProcess
{
    public abstract void Init(ProcessData data);

    public abstract float GetTickTime();

    public abstract void Tick(int tickCount, UserData userData);
}
