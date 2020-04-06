using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : CookieProcess
{
    private ProcessData processData;
    private BuildingConfig configData;

    public override void Init(ProcessData data, BuildingConfig config)
    {
        processData = data;
        configData = config;
    }

    public override int GetTickTime()
    {
        if (configData)
        {
            return configData.tickRate;
        }
        return 1000;
    }

    public override void Tick(long tickCount, UserData userData)
    {
        if (configData != null)
        {
            userData.cookieCount += tickCount * configData.levels[processData.level].rate;
        }
    }
}
