using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;


public class CookieServer
{
    static CookieServer smInst = new CookieServer();
    public static CookieServer Instance
    {
        get
        {
            return smInst;
        }
    }
    static int MockLag = 16;


    Dictionary<string, UserData> userData = new Dictionary<string, UserData>();
    BuildingConfig[] configData = new BuildingConfig[(int)ProcessType.COUNT];

    public CookieServer()
    {
        userData = new Dictionary<string, UserData>();
        configData = new BuildingConfig[(int)ProcessType.COUNT];
        configData[(int)ProcessType.Grandma] = Resources.Load<BuildingConfig>("config/GrandmaConfig");
    }

    public void BeginSession(string userId)
    {
        string path = Application.persistentDataPath + userId + ".json";
        UserData user = new UserData();
        long now = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            user = JsonUtility.FromJson<UserData>(jsonData);
            for (int i = 0; i < (int)ProcessType.COUNT; ++i)
            {
                if (user.processData[i] == null)
                {
                    user.processData[i] = new ProcessData();
                    user.processData[i].lastProcessedTime = now;
                }
            }
        }
        else
        {
            for (int i = 0; i < (int)ProcessType.COUNT; ++i)
            {
                user.processData[i] = new ProcessData();
                user.processData[i].lastProcessedTime = now;
            }
            //testing
            user.processData[(int)ProcessType.Grandma].count = 1;
        }
        user.InitProcesses(configData);
        userData[userId] = user;
        ProcessUser(user);
    }

    public void EndSession(string userId)
    {
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            string path = Application.persistentDataPath + userId + ".json";
            File.WriteAllText(path, JsonUtility.ToJson(user));
        }
    }

    public async Task<long> ClickCookie(string userId, int clickCount)
    {
        await Task.Delay(MockLag);
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            user.cookieCount += clickCount;
            ProcessUser(user);
            return user.cookieCount;
        }
        return 0;
    }

    public async Task<long> GetCookieCount(string userId)
    {
        await Task.Delay(MockLag);
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            return user.cookieCount;
        }
        return 0;
    }

    public async Task<long> RequestProcessUser(string userId)
    {
        await Task.Delay(MockLag);
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            ProcessUser(user);
            return user.cookieCount;
        }
        return 0;
    }

    private void ProcessUser(UserData user)
    {
        long now = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        for (int i = 0; i < (int)ProcessType.COUNT; ++i)
        {
            if (user.processes[i] != null)
            {
                long elapsedTime = now - user.processData[i].lastProcessedTime;
                long tickCount = elapsedTime / user.processes[i].GetTickTime();
                user.processes[i].Tick(tickCount, user);
                user.processData[i].lastProcessedTime += tickCount * user.processes[i].GetTickTime();
            }
        }
    }


}
