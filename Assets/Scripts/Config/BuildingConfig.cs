using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcessTrigger
{
    OnTick,
    OnClick
};

[System.Serializable]
public class BuildingLevel
{
    public int minCount;
    public int cost;
    public int rate;
    public string displayName;
}

[CreateAssetMenu(menuName = "Cookie/BuildingConfig")]
public class BuildingConfig : ScriptableObject
{
    public BuildingLevel[] levels;
    [Tooltip ("Miliseconds per tick for this building")]
    public int tickRate;
    public ProcessTrigger processTrigger;
    public long buildCost;
    public string displayName;
}

public class ConfigData
{
    public BuildingConfig[] configData;
    public ConfigData()
    {
        configData = new BuildingConfig[(int)ProcessType.COUNT];
        configData[(int)ProcessType.Grandma] = Resources.Load<BuildingConfig>("config/GrandmaConfig");
    }
}
