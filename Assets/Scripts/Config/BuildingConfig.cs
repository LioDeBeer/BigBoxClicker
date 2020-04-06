using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
