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
        newStageData.m_objectList.Clear();

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
                tempUnitInfo.z = (int)obj.transform.position.z;
                tempUnitInfo.m_unitID = GetIDFromName(obj.name);
                tempUnitInfo.m_name = obj.name;
                newStageData.m_unitList.Add(tempUnitInfo);
            }

            else if(obj.CompareTag("area"))
            {
                s_AreaInfo tempAreaInfo = new s_AreaInfo();
                tempAreaInfo.x = (int)obj.transform.position.x;
                tempAreaInfo.y = (int)obj.transform.position.y;
                tempAreaInfo.m_areaID = GetIDFromName(obj.name);
                tempAreaInfo.m_name = obj.name;
                if(obj.GetComponent<EnemySpawnTile>() != null)
                {
                    tempAreaInfo.m_spawnTurn = obj.GetComponent<EnemySpawnTile>().m_createTurn;
                }
                newStageData.m_areaList.Add(tempAreaInfo);

            }

            else if (obj.CompareTag("object"))
            {
                s_ObjectInfo tempObjectInfo;
                tempObjectInfo.x = (int)obj.transform.position.x;
                tempObjectInfo.y = (int)obj.transform.position.y;
                tempObjectInfo.m_objectID = GetIDFromName(obj.name);
                tempObjectInfo.m_name = obj.name;
                newStageData.m_objectList.Add(tempObjectInfo);

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
        if (name.Contains("tile_102")) return 102;
        if (name.Contains("tile_103")) return 103;
        if (name.Contains("tile_104")) return 104;
        if (name.Contains("tile_105")) return 105;
        if (name.Contains("tile_106")) return 106;
        if (name.Contains("tile_107")) return 107;

        if (name.Contains("tile_200")) return 200;
        if (name.Contains("tile_201")) return 201;

        if (name.Contains("tile_300")) return 300;
        if (name.Contains("tile_301")) return 301;

        return -1;
    }
}
