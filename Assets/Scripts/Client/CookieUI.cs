using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CookieUI : MonoBehaviour
{
    public Text cookieCountText;

    private Task<long> countTask = null;
    //don't want a server message on each click
    private long confirmedCookies = 0;
    private int bufferedClicks = 0;
    private int predictedClicks = 0;
    private float lastSendTime = 0;

    private static float MinSendTime = 0.1f;
    private static string userId = "TestUser";


    // Start is called before the first frame update
    void Start()
    {
        CookieServer.Instance.BeginSession(userId);
        cookieCountText.text = "";
        countTask = CookieServer.Instance.GetCookieCount(userId);
    }

    private void OnDestroy()
    {
        CookieServer.Instance.EndSession(userId);
    }

    // Update is called once per frame
    void Update()
    {
        if (countTask != null)
        {
            if (countTask.IsCompleted)
            {
                confirmedCookies = countTask.Result;
                cookieCountText.text = (confirmedCookies + bufferedClicks).ToString();
                countTask = null;
                predictedClicks = 0;
            }
        }
        else if (Time.time - lastSendTime > 1.0f)
        {
            countTask = CookieServer.Instance.RequestProcessUser(userId);
            lastSendTime = Time.time;
        }
    }

    public void OnCookieClicked()
    {
        ++bufferedClicks;
        ++predictedClicks;
        cookieCountText.text = (predictedClicks + confirmedCookies).ToString();
        float elapsedTime = Time.time - lastSendTime;
        if (elapsedTime >= MinSendTime && countTask == null)
        {
            countTask = CookieServer.Instance.ClickCookie(userId, bufferedClicks);
            bufferedClicks = 0;
        }
    }
}
