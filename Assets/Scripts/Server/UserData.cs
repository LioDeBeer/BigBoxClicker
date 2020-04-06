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

public class ProcessData
{
    public int count;
    public int level;
    public float lastProcessedTime;

}

public class UserData
{
    public long cookieCount;
    public ProcessData[] processData = new ProcessData[(int)ProcessType.COUNT];
    public CookieProcess[] processes = new CookieProcess[(int)ProcessType.COUNT];

    public void InitProcesses()
    {
        processes[(int)ProcessType.Grandma] = new Grandma();
        processes[(int)ProcessType.Grandma].Init(processData[(int)ProcessType.Grandma]);
    }

}
