using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct s_TerrainInfo
{
    public int x, y;
    public int m_terrainID;
    public string m_name;
}

[System.Serializable]
public struct s_UnitInfo
{
    public int x, y;      
    public int m_unitID;
    public string m_name;
}
[System.Serializable]
public struct s_AreaInfo
{
    public int x, y;
    public int m_areaID;
    public string m_name;
}

[CreateAssetMenu(fileName = "NewStage", menuName = "ProjectD/Stage")]
public class StageData : ScriptableObject
{
    public string stageName;
    public int mapWidth, mapHeight;

    // C의 동적 배열 대신 List 사용
    public List<s_TerrainInfo> m_terrainList = new List<s_TerrainInfo>();
    public List<s_UnitInfo> m_unitList = new List<s_UnitInfo>();
    public List<s_AreaInfo> m_areaList = new List<s_AreaInfo>();

}