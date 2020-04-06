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


    Dictionary<string, UserData> userData;
    ConfigData configData;

    public CookieServer()
    {
        userData = new Dictionary<string, UserData>();
        configData = new ConfigData();
    }

    private string GetSavePath(string userId)
    {
        return Application.persistentDataPath + "/" + userId + ".json";
    }

    public async Task<UserData> BeginSession(string userId)
    {
        await Task.Delay(MockLag);
        string path = GetSavePath(userId);
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
        }
        user.InitProcesses(configData.configData);
        userData[userId] = user;
        ProcessUser(user);
        return user;
    }

    public void EndSession(string userId)
    {
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            string path = GetSavePath(userId);
            File.WriteAllText(path, JsonUtility.ToJson(user));
            Debug.Log(path);
            Debug.Log(JsonUtility.ToJson(user));
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

    public async Task<PurchaseResult> MakePurchase(string userId, ProcessType process, int count)
    {
        await Task.Delay(MockLag);
        UserData user;
        PurchaseResult result = new PurchaseResult();
        result.processType = process;
        if ((int)process < (int)ProcessType.COUNT && (int)process >= 0)
        {
            if (userData.TryGetValue(userId, out user))
            {
                long cost = count * configData.configData[(int)process].GetBuildCost(user.processData[(int)process].count);
                if (user.cookieCount >= cost)
                {
                    user.cookieCount -= cost;
                    user.processData[(int)process].count += count;
                }
                result.buildingCount = user.processData[(int)process].count;
                result.cookieCount = user.cookieCount;
            }
        }
        return result;
    }

    public async Task<PurchaseResult> PurchaseUpgrade(string userId, ProcessType process)
    {
        await Task.Delay(MockLag);
        UserData user;
        PurchaseResult result = new PurchaseResult();
        if ((int)process < (int)ProcessType.COUNT && (int)process >= 0)
        {
            result.processType = process;
            if (userData.TryGetValue(userId, out user))
            {
                int level = user.processData[(int)process].level;
                if (level < configData.configData[(int)process].levels.Length - 1)
                {
                    long cost = configData.configData[(int)process].levels[level + 1].cost;
                    if (user.cookieCount >= cost)
                    {
                        user.cookieCount -= cost;
                        ++user.processData[(int)process].level;
                    }
                }
                result.buildingCount = user.processData[(int)process].count;
                result.cookieCount = user.cookieCount;
            }
        }
        return result;
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
