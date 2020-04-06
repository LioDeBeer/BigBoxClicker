using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for a timed powerup
public abstract class CookieProcess
{
    public abstract void Init(ProcessData data, BuildingConfig config);

    public abstract int GetTickTime();

    public abstract void Tick(long tickCount, UserData userData);
}
