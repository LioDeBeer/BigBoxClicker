using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public Text nameText;
    public Text costText;
    public Text countText;

    public void Init(BuildingConfig config, long count)
    {
        nameText.text = config.displayName;
        costText.text = config.GetBuildCost(count).ToString();
        countText.text = count.ToString();
    }


}
