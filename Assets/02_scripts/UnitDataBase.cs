using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataBase : MonoBehaviour
{
    public static UnitDataBase s_instance;
    public TextAsset path_csv;
    public Dictionary<int, UnitData> m_unitDataDic = new Dictionary<int, UnitData>();


    private void Awake()
    {
        MakeSingletonPattern();
        m_unitDataDic.Clear();
        LoadFromCsv();
    }
    
    public void MakeSingletonPattern()
    {
        if(s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Sprite GetUnitIconSprite(int nUnitOriginNumber)
    {
        UnitData tempData;
        Sprite newSprite = null;
        bool isGet = false;
        isGet = m_unitDataDic.TryGetValue(nUnitOriginNumber, out tempData);

        if (isGet == true)
        {
            string newPath = tempData.m_unitIcon_path;
            newSprite = Resources.Load<Sprite>(newPath);
        }

        return newSprite;
    }


    public Sprite GetUnitPortrait(int nUnitOriginNumber)
    {
        UnitData tempData;
        Sprite newSprite = null;
        bool isGet = false;
        isGet = m_unitDataDic.TryGetValue(nUnitOriginNumber, out tempData);
        
        if (isGet == true)
        {
            string newPath = tempData.m_PortraitPath;
            newSprite = Resources.Load<Sprite>(newPath);    
        }
        
        return newSprite;
    }

    public void LoadFromCsv()
    {
        m_unitDataDic.Clear();
        string csvPath = path_csv.text;
        string[] dataLines = csvPath.Split('\n');
        int nLineCount = dataLines.Length;

        // 헤더 이름으로 열 위치를 찾습니다. 열 순서가 바뀌어도 읽을 수 있어요.
        string[] headers = dataLines[0].Trim().Split(',');
        Dictionary<string, int> col = new Dictionary<string, int>();
        for (int h = 0; h < headers.Length; h++)
        {
            col[headers[h]] = h;
        }

        for(int i = 1; i < nLineCount; i++)
        {
            string dataLine = dataLines[i].Trim();
            if (string.IsNullOrEmpty(dataLine)) continue;
            string[] dataColums = dataLine.Split(',');
            UnitData newData = new UnitData();
            newData.m_UnitID = int.Parse(dataColums[col["UnitID"]]);
            newData.m_UnitName = dataColums[col["UnitName"]];
            newData.m_unitType = dataColums[col["unitType"]];
            newData.m_unitTier = int.Parse(dataColums[col["unitTier"]]);
            newData.m_PortraitPath = dataColums[col["path_combatPortrait"]];
            newData.m_level = int.Parse(dataColums[col["level"]]);
            newData.m_stat_HP = int.Parse(dataColums[col["stat_HP"]]);
            newData.m_stat_ATK = int.Parse(dataColums[col["stat_ATK"]]);
            newData.m_stat_DEF = int.Parse(dataColums[col["stat_DEF"]]);
            newData.m_stat_HIT = int.Parse(dataColums[col["stat_HIT"]]);
            newData.m_stat_EVA = int.Parse(dataColums[col["stat_EVA"]]);
            newData.m_stat_MOV = int.Parse(dataColums[col["stat_MOV"]]);

            for (int t = 0; t < UnitData.TRAIT_SLOT_COUNT; t++)
            {
                newData.m_traits[t] = dataColums[col["trait_0" + (t + 1)]];
            }
            for (int a = 0; a < UnitData.ACTIVE_SLOT_COUNT; a++)
            {
                newData.m_activeSkills[a] = dataColums[col["activeSkill_0" + (a + 1)]];
            }

            newData.m_equip_weapon = dataColums[col["equip_weapon"]];
            newData.m_equip_armor = dataColums[col["equip_armor"]];
            newData.m_equip_accessary = dataColums[col["equip_accessary"]];
            newData.m_equip_weapon_path = dataColums[col["equip_weapon_path"]];
            newData.m_equip_armor_path = dataColums[col["equip_armor_path"]];
            newData.m_equip_accessary_path = dataColums[col["equip_accessary_path"]];

            newData.m_unitIcon_path = dataColums[col["path_icon"]];
            newData.m_stat_attackRange = int.Parse(dataColums[col["stat_atkRange"]]);

            m_unitDataDic.Add(newData.m_UnitID, newData);
            //Debug.Log("unitID: " + newData.m_UnitID);
        }

        //Debug.Log("Load unitData complete..");
    }

}
