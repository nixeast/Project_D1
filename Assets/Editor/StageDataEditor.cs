using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

[CustomEditor(typeof(StageData))]
public class StageDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StageData newStageData = (StageData)target;

        if (GUILayout.Button("SAVE SCENE TO DATA", GUILayout.Height(40)))
        {
            SaveToData(newStageData);
        }
    }

    void SaveToData(StageData newStageData)
    {
        newStageData.m_terrainList.Clear();
        newStageData.m_unitList.Clear();
        newStageData.m_areaList.Clear();

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int nCount = allObjects.Length;

        for (int i = 0; i < nCount; i++)
        {
            GameObject obj = allObjects[i];

            if (obj.CompareTag("terrain"))
            {
                s_TerrainInfo tempTerrainInfo;

                tempTerrainInfo.x = (int)obj.transform.position.x;
                tempTerrainInfo.y = (int)obj.transform.position.y;
                tempTerrainInfo.m_terrainID = GetIDFromName(obj.name);
                tempTerrainInfo.m_name = obj.name;
                newStageData.m_terrainList.Add(tempTerrainInfo);
            }
            
            else if (obj.CompareTag("enemy"))
            {
                s_UnitInfo tempUnitInfo;
                tempUnitInfo.x = (int)obj.transform.position.x;
                tempUnitInfo.y = (int)obj.transform.position.y;
                tempUnitInfo.m_unitID = GetIDFromName(obj.name);
                tempUnitInfo.m_name = obj.name;
                newStageData.m_unitList.Add(tempUnitInfo);
            }

            else if(obj.CompareTag("area"))
            {
                s_AreaInfo tempAreaInfo;
                tempAreaInfo.x = (int)obj.transform.position.x;
                tempAreaInfo.y = (int)obj.transform.position.y;
                tempAreaInfo.m_areaID = GetIDFromName(obj.name);
                tempAreaInfo.m_name = obj.name;
                newStageData.m_areaList.Add(tempAreaInfo);

            }
        }

        // Notice stageData file info modification to UnityEditor (important!)
        EditorUtility.SetDirty(newStageData);
        AssetDatabase.SaveAssets();
        Debug.Log(">> DATA SAVED SUCCESSFULLY.");
    }

    int GetIDFromName(string name)
    {
        if (name.Contains("tile_default")) return 0;
        if (name.Contains("tile_001")) return 1;
        if (name.Contains("tile_002")) return 2;
        if (name.Contains("tile_100")) return 100;
        if (name.Contains("tile_101")) return 101;
        if (name.Contains("tile_105")) return 105;
        if (name.Contains("tile_200")) return 200;

        return -1;
    }
}
