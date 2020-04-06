using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CookieUI : MonoBehaviour
{
    public Text cookieCountText;
    public Transform ShopRoot;
    public GameObject ShopPrefab;

    public Transform UpgradeRoot;
    public GameObject UpgradePrefab;

    private Task<long> countTask = null;
    private Task<PurchaseResult> purchaseTask = null;
    //don't want a server message on each click
    private long confirmedCookies = 0;
    private int bufferedClicks = 0;
    private int predictedClicks = 0;
    private float lastSendTime = 0;

    private static float MinSendTime = 0.1f;
    private ShopButton[] shopButtons = new ShopButton[(int)ProcessType.COUNT];


    // Start is called before the first frame update
    void Start()
    {
        cookieCountText.text = "";
        countTask = CookieServer.Instance.GetCookieCount(CookieGameManager.Instance.UserId);
        ShopPrefab.SetActive(false);
        UpgradePrefab.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        if (countTask != null)
        {
            if (countTask.IsCompleted)
            {
                confirmedCookies = countTask.Result;
                CookieGameManager.Instance.UserData.cookieCount = confirmedCookies;
                cookieCountText.text = (confirmedCookies + bufferedClicks).ToString();
                countTask = null;
                UpdateShop();
                predictedClicks = 0;
            }
        }
        else if (purchaseTask != null)
        {
            if (purchaseTask.IsCompleted)
            {
                PurchaseResult result = purchaseTask.Result;
                CookieGameManager.Instance.UserData.cookieCount = result.cookieCount;
                CookieGameManager.Instance.UserData.processData[(int)result.processType].count = result.buildingCount;
                UpdateShop();
                UpdateUpgrades();
                purchaseTask = null;
            }
        }
        else if (Time.time - lastSendTime > 1.0f)
        {
            countTask = CookieServer.Instance.RequestProcessUser(CookieGameManager.Instance.UserId);
            lastSendTime = Time.time;
        }
    }

    public void InitCount(long count)
    {
        cookieCountText.text = (count).ToString();
        bufferedClicks = 0;
        predictedClicks = 0;
    }

    public void InitShop()
    {
        UserData userData = CookieGameManager.Instance.UserData;
        while (ShopRoot.childCount > 0)
        {
            GameObject child = ShopRoot.GetChild(0).gameObject;
            child.transform.SetParent(null);
            GameObject.Destroy(child);
        }
        for (int i = 0; i < (int)ProcessType.COUNT; ++i)
        {
            BuildingConfig config = CookieGameManager.Instance.ConfigData.configData[i];
            ProcessType processType = (ProcessType)i;
            if (config)
            {
                GameObject buttonObj = GameObject.Instantiate(ShopPrefab);
                buttonObj.transform.SetParent(ShopRoot);
                buttonObj.SetActive(true);
                ShopButton shopButton = buttonObj.GetComponent<ShopButton>();
                if (shopButton)
                {
                    shopButtons[i] = shopButton;
                }
                Button button = buttonObj.GetComponent<Button>();
                if (button)
                {
                    button.onClick.AddListener(() =>
                    {
                        if (userData.cookieCount > config.GetBuildCost(userData.processData[(int)processType].count) && purchaseTask == null)
                        {
                            purchaseTask = CookieServer.Instance.MakePurchase(CookieGameManager.Instance.UserId, processType, 1);
                        }
                    });
                }
            }
        }
        UpdateShop();
        UpdateUpgrades();
    }

    void UpdateShop()
    {
        UserData userData = CookieGameManager.Instance.UserData;
        for (int i = 0; i < (int)ProcessType.COUNT; ++i)
        {
            if (shopButtons[i] != null)
            { 
                BuildingConfig config = CookieGameManager.Instance.ConfigData.configData[i];
                ProcessType processType = (ProcessType)i;
                if (userData.processData[i].count > 0 || userData.cookieCount > config.GetBuildCost(userData.processData[i].count))
                {
                    shopButtons[i].Init(config, userData.processData[i].count);
                }
            }
        }
    }

    void UpdateUpgrades()
    {
        while (UpgradeRoot.childCount > 0)
        {
            GameObject child = UpgradeRoot.GetChild(0).gameObject;
            child.transform.SetParent(null);
            GameObject.Destroy(child);
        }
        UserData userData = CookieGameManager.Instance.UserData;
        for (int i = 0; i < (int)ProcessType.COUNT; ++i)
        {
            ProcessType processType = (ProcessType)i;
            BuildingConfig config = CookieGameManager.Instance.ConfigData.configData[i];
            if (config)
            { 
                int currentLevel = userData.processData[i].level;
                if (currentLevel < config.levels.Length - 1)
                {
                    if (userData.processData[i].count >= config.levels[currentLevel + 1].minCount)
                    {
                        GameObject upgradeObj = GameObject.Instantiate(UpgradePrefab);
                        upgradeObj.transform.SetParent(UpgradeRoot);
                        upgradeObj.SetActive(true);
                        Button button = upgradeObj.GetComponent<Button>();
                        if (button)
                        {
                            button.onClick.AddListener(() =>
                            {
                                if (userData.cookieCount > config.levels[currentLevel + 1].cost && purchaseTask == null)
                                {
                                    purchaseTask = CookieServer.Instance.PurchaseUpgrade(CookieGameManager.Instance.UserId, processType);
                                }
                            });
                        }
                        UpgradeButton upgradeButton = upgradeObj.GetComponent<UpgradeButton>();
                        if (upgradeButton)
                        {
                            upgradeButton.Init(config.levels[currentLevel + 1].displayName, config.levels[currentLevel + 1].cost);
                        }
                    }
                }
            }
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
            countTask = CookieServer.Instance.ClickCookie(CookieGameManager.Instance.UserId, bufferedClicks);
            bufferedClicks = 0;
        }
    }
}
