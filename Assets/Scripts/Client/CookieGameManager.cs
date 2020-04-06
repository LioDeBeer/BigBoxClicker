using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CookieGameManager : MonoBehaviour
{
    private static CookieGameManager smInst;
    public static CookieGameManager Instance { get { return smInst; } }


    private ConfigData configData;
    public ConfigData ConfigData{ get{ return configData;}}
    private UserData clientUserData;
    public UserData UserData { get { return clientUserData; } }

    private Task<UserData> loginTask = null;
    string userId;
    public string UserId { get { return userId; } }
    public CookieUI cookieUI;

    // Start is called before the first frame update
    void Start()
    {
        smInst = this;
        userId = "TestUser";

        configData = new ConfigData();

        loginTask = CookieServer.Instance.BeginSession(userId);
    }

    private void OnDestroy()
    {
        CookieServer.Instance.EndSession(userId);
    }

    // Update is called once per frame
    void Update()
    {
        if (loginTask != null && loginTask.IsCompleted)
        {
            clientUserData = loginTask.Result;
            cookieUI.InitCount(clientUserData.cookieCount);
            cookieUI.InitShop();
            loginTask = null;
        }
    }

}
