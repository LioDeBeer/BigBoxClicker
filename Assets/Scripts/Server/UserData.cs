using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcessType
{
    Cursor,
    Grandma,
    Farm,
    Factory,
    Mine,
    COUNT
}

[System.Serializable]
public class ProcessData
{
    public long count;
    public int level;
    public long lastProcessedTime;

}

[System.Serializable]
public class UserData
{
    public long cookieCount;
    public ProcessData[] processData = new ProcessData[(int)ProcessType.COUNT];
    public CookieProcess[] processes = new CookieProcess[(int)ProcessType.COUNT];

    public void InitProcesses(BuildingConfig[] config)
    {
        if (config.Length == (int)ProcessType.COUNT)
        {
            processes[(int)ProcessType.Grandma] = new Grandma();
            processes[(int)ProcessType.Grandma].Init(processData[(int)ProcessType.Grandma], config[(int)ProcessType.Grandma]);
        }
        else
        {
            Debug.LogError("Invalid config passed in to InitProcesses");
        }
    }

}

[System.Serializable]
public class PurchaseResult
{
    public ProcessType processType;
    public long buildingCount;
    public long cookieCount;
}
