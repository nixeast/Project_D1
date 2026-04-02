using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public IngameUiManager m_ingameUiManager;

    [Header("StageData Source")]
    public StageData targetStage;

    [Header("Prefab List")]
    public List<GameObject> m_terrainPrefabList;
    public List<GameObject> m_unitPrefabList;
    public List<GameObject> m_areaPrefabList;

    [Header("Generate roots")]
    public Transform tileRoot_terrain;
    public Transform tileRoot_object;
    public Transform tileRoot_area;

    void Start()
    {
        if (targetStage != null)
        {
            GenerateStage();
        }
    }

    public void GenerateStage()
    {
        if (targetStage == null) return;

        int nTileCount = targetStage.m_terrainList.Count;
        for (int i = 0; i < nTileCount; i++)
        {
            s_TerrainInfo newTerrain = targetStage.m_terrainList[i];

            GameObject prefab = FindPrefabByName(m_terrainPrefabList, newTerrain.m_name);
            if (prefab != null)
            {
                
                Vector3 spawnPos = new Vector3(newTerrain.x, newTerrain.y, 0);
                Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_terrain);
            }
        }
        
        int nUnitCount = targetStage.m_unitList.Count;
        for (int j = 0; j < nUnitCount; j++)
        {
            
            s_UnitInfo newUnit = targetStage.m_unitList[j];

            GameObject prefab = FindPrefabByName(m_unitPrefabList, newUnit.m_name);
            if (prefab != null)
            {
                Vector3 spawnPos = new Vector3(newUnit.x, newUnit.y, 0);
                Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_object);
            }
        }

        int nAreaCount = targetStage.m_areaList.Count;
        for(int k = 0; k < nAreaCount; k++)
        {
            s_AreaInfo newArea = targetStage.m_areaList[k];
            GameObject prefab = FindPrefabByName(m_areaPrefabList, newArea.m_name);
            StartingPointButton tempStartingPoint = prefab.GetComponent<StartingPointButton>();
            if (prefab != null)
            {
                Vector3 spawnPos = new Vector3(newArea.x, newArea.y, 0);
                tempStartingPoint.m_ingameUiManager = m_ingameUiManager;
                tempStartingPoint.obj_tilemap_object = tileRoot_object.gameObject;
                Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_area);
            }
        }

    }

    GameObject FindPrefabByName(List<GameObject> prefabs, string tempName)
    {
        int prefabCount = prefabs.Count;

        for (int i = 0; i < prefabCount; i++)
        {
            GameObject p = prefabs[i];

            // �������� ����ִ��� ��� �ڵ� (C�� NULL üũ)
            //if (p == null) continue;

            if (p.name.Contains(tempName))
            {
                return p;
            }
        }

        //Debug.LogWarning($">> [������ƮD] ID {tempName}�� �ش��ϴ� �������� ã�� �� �����ϴ�.");
        return null;
    }
}
