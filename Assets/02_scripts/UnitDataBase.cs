using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataBase : MonoBehaviour
{
    public TextAsset path_csv;
    public Dictionary<int, UnitData> m_unitDataDic = new Dictionary<int, UnitData>();


    private void Awake()
    {
        m_unitDataDic.Clear();
        LoadFromCsv();
    }

    public void LoadFromCsv()
    {
        m_unitDataDic.Clear();
        string csvPath = path_csv.text;
        string[] dataLines = csvPath.Split('\n');
        int nLineCount = dataLines.Length;
        for(int i = 1; i < nLineCount; i++)
        {
            string dataLine = dataLines[i].Trim();
            string[] dataColums = dataLine.Split(',');
            UnitData newData = new UnitData();
            newData.m_UnitID = int.Parse(dataColums[0]);
            newData.m_UnitName = dataColums[1];
            newData.m_unitType = dataColums[2];
            newData.m_unitTier = int.Parse(dataColums[3]);
            newData.m_PortraitPath = dataColums[4];
            newData.m_level = int.Parse(dataColums[5]);
            newData.m_stat_HP = int.Parse(dataColums[6]);
            newData.m_stat_ATK = int.Parse(dataColums[7]);
            newData.m_stat_DEF = int.Parse(dataColums[8]);
            newData.m_stat_HIT = int.Parse(dataColums[9]);
            newData.m_stat_EVA = int.Parse(dataColums[10]);
            newData.m_stat_AP = int.Parse(dataColums[11]);
            newData.m_trait_01 = dataColums[12];
            newData.m_trait_02 = dataColums[13];
            newData.m_passiveSkill_01 = dataColums[14];
            newData.m_passiveSkill_02 = dataColums[15];
            newData.m_passiveSkill_03 = dataColums[16];
            newData.m_passiveSkill_04 = dataColums[17];
            newData.m_activeSkill_01 = dataColums[18];
            newData.m_activeSkill_02 = dataColums[19];
            newData.m_activeSkill_03 = dataColums[20];

            newData.m_trait_01_path = dataColums[21];
            newData.m_trait_02_path = dataColums[22];
            newData.m_passiveSkill_01_path = dataColums[23];
            newData.m_passiveSkill_02_path = dataColums[24];
            newData.m_passiveSkill_03_path = dataColums[25];
            newData.m_passiveSkill_04_path = dataColums[26];
            newData.m_activeSkill_01_path = dataColums[27];
            newData.m_activeSkill_02_path = dataColums[28];
            newData.m_activeSkill_03_path = dataColums[29];

            newData.m_equip_weapon = dataColums[30];
            newData.m_equip_armor = dataColums[31];
            newData.m_equip_accessary = dataColums[32];
            newData.m_equip_weapon_path = dataColums[33];
            newData.m_equip_armor_path = dataColums[34];
            newData.m_equip_accessary_path = dataColums[35];

            m_unitDataDic.Add(newData.m_UnitID, newData);
            //Debug.Log("unitID: " + newData.m_UnitID);
        }

        //Debug.Log("Load unitData complete..");
    }

}
