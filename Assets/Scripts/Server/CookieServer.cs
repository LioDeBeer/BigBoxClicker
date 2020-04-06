using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public class UserData
{
    public int cookieCount;
}

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
        }
        userData[userId] = user;
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

    public async Task<int> ClickCookie(string userId, int clickCount)
    {
        await Task.Delay(16);
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            user.cookieCount += clickCount;
            return user.cookieCount;
        }
        return 0;
    }

    public async Task<int> GetCookieCount(string userId)
    {
        await Task.Delay(16);
        UserData user;
        if (userData.TryGetValue(userId, out user))
        {
            return user.cookieCount;
        }
        return 0;
    }


}
