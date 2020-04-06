using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Text nameText;
    public Text costText;

    public void Init(string displayName, long cost)
    {
        nameText.text = displayName;
        costText.text = cost.ToString();
    }
}
