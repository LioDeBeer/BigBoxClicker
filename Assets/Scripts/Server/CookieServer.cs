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


    Dictionary<string, UserData> userData = new Dictionary<string, UserData>();

    public void BeginSession(string userId)
    {
        string path = Application.persistentDataPath + userId + ".json";
        UserData user = new UserData();
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            user = JsonUtility.FromJson<UserData>(jsonData);
            for (int i = 0; i < (int)ProcessType.COUNT; ++i)
            {
                if (user.processData[i] == null)
                {
                    user.processData[i] = new ProcessData();
                    user.processData[i].lastProcessedTime = Time.time;
                }
            }
        }
        else
        {
            for (int i = 0; i < (int)ProcessType.COUNT; ++i)
            {
                user.processData[i] = new ProcessData();
                user.processData[i].lastProcessedTime = Time.time;
            }
            //testing
            user.processData[(int)ProcessType.Grandma].count = 1;
        }
        user.InitProcesses();
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
        await Task.Delay(16);
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
        await Task.Delay(16);
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            return user.cookieCount;
        }
        return 0;
    }

    private void ProcessUser(UserData user)
    {
        for (int i = 0; i < (int)ProcessType.COUNT; ++i)
        {
            if (user.processes[i] != null)
            {
                float elapsedTime = Time.time - user.processData[i].lastProcessedTime;
                int tickCount = (int)(elapsedTime / user.processes[i].GetTickTime());
                user.processes[i].Tick(tickCount, user);
                user.processData[i].lastProcessedTime += tickCount * user.processes[i].GetTickTime();
            }
        }
    }


}
