using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public IngameUiManager m_ingameUiManager;
    public GameManager m_gameManager;

    [Header("StageData Source")]
    public StageData targetStage;

    [Header("Prefab List")]
    public List<GameObject> m_terrainPrefabList;
    public List<GameObject> m_unitPrefabList;
    public List<GameObject> m_areaPrefabList;
    public List<GameObject> m_objectPrefabList;

    [Header("Generate roots")]
    public Transform tileRoot_terrain;
    public Transform tileRoot_object;
    public Transform tileRoot_area;



    public void Awake()
    {
        //GenerateStage();
        //Debug.Log("generate map");

    }

    public void GenerateTerrainTile()
    {
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
    }

    public void GenerateUnitTile()
    {
        int nUnitCount = targetStage.m_unitList.Count;
        for (int j = 0; j < nUnitCount; j++)
        {
            s_UnitInfo newUnit = targetStage.m_unitList[j];
            GameObject prefab = FindPrefabByName(m_unitPrefabList, newUnit.m_name);
            if (prefab != null)
            {
                Vector3 spawnPos = new Vector3(newUnit.x, newUnit.y, newUnit.z);
                GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_object);
            }
        }

        GenerateAssaultExtraUnits();
    }

    // 버민킨 대공세: 위협도 20%마다 적 유닛 +1 (GDD 03 §3.2)
    // 기존 적을 차례로 골라, 그 적에게 가장 가까운 빈 지형 칸에 같은 적을 추가합니다.
    private void GenerateAssaultExtraUnits()
    {
        int nExtraCount = Campaign.GetCurrentAssaultExtraEnemyCount();
        int nUnitCount = targetStage.m_unitList.Count;
        if (nExtraCount <= 0 || nUnitCount == 0) return;

        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        foreach (s_UnitInfo unit in targetStage.m_unitList) occupied.Add(new Vector2Int(unit.x, unit.y));
        foreach (s_ObjectInfo obj in targetStage.m_objectList) occupied.Add(new Vector2Int(obj.x, obj.y));
        foreach (s_AreaInfo area in targetStage.m_areaList) occupied.Add(new Vector2Int(area.x, area.y));

        for (int i = 0; i < nExtraCount; i++)
        {
            s_UnitInfo source = targetStage.m_unitList[i % nUnitCount];
            GameObject prefab = FindPrefabByName(m_unitPrefabList, source.m_name);
            if (prefab == null) continue;

            bool isFound = false;
            Vector2Int best = Vector2Int.zero;
            int bestDistance = int.MaxValue;
            foreach (s_TerrainInfo terrain in targetStage.m_terrainList)
            {
                Vector2Int pos = new Vector2Int(terrain.x, terrain.y);
                if (occupied.Contains(pos)) continue;

                int distance = Mathf.Abs(pos.x - source.x) + Mathf.Abs(pos.y - source.y);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = pos;
                    isFound = true;
                }
            }
            if (isFound == false) return;

            occupied.Add(best);
            Vector3 spawnPos = new Vector3(best.x, best.y, source.z);
            Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_object);
        }
    }

    public void GenerateAreaTile()
    {
        int nAreaCount = targetStage.m_areaList.Count;
        for (int k = 0; k < nAreaCount; k++)
        {
            s_AreaInfo newArea = targetStage.m_areaList[k];
            GameObject prefab = FindPrefabByName(m_areaPrefabList, newArea.m_name);
            Vector3 spawnPos = new Vector3(newArea.x, newArea.y, -2.0f);

            if (prefab.GetComponent<StartingPointButton>() != null)
            {
                GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_area);
                StartingPointButton tempStartingPoint = newObject.GetComponent<StartingPointButton>();
                tempStartingPoint.m_ingameUiManager = m_ingameUiManager;
                tempStartingPoint.obj_tilemapRoot_object = tileRoot_object.gameObject;
                newObject.SetActive(false);

                m_ingameUiManager.m_startingPointList.Add(tempStartingPoint);
            }
            else if (prefab.GetComponent<EnemySpawnTile>() != null)
            {
                GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_area);
                EnemySpawnTile newTile = newObject.GetComponent<EnemySpawnTile>();
                newTile.m_createTurn = newArea.m_spawnTurn;

                //m_gameManager.m_enemySpawnTileList.Add(newTile);
            }

            //Debug.Log("area created");
        }
    }

    public void GenerateObjectTile()
    {
        int nObjectCount = targetStage.m_objectList.Count;
        for (int j = 0; j < nObjectCount; j++)
        {
            s_ObjectInfo newObj = targetStage.m_objectList[j];

            GameObject prefab = FindPrefabByName(m_objectPrefabList, newObj.m_name);

            if (prefab != null)
            {
                Vector3 spawnPos = new Vector3(newObj.x, newObj.y, -1.0f);
                GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.identity, tileRoot_object);
            }
        }
    }

    public void GenerateStage()
    {
        if (targetStage == null) return;

        GenerateTerrainTile();
        GenerateUnitTile();
        GenerateAreaTile();
        GenerateObjectTile();
    }

    GameObject FindPrefabByName(List<GameObject> prefabs, string tempName)
    {
        int prefabCount = prefabs.Count;

        for (int i = 0; i < prefabCount; i++)
        {
            GameObject p = prefabs[i];

            if (p.name.Contains(tempName))
            {
                return p;
            }
        }

        return null;
    }
}
