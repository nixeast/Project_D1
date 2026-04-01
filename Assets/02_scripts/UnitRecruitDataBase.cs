using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRecruitDataBase : MonoBehaviour
{
    [SerializeField]
    private TextAsset m_recruitInfoPath;
    public Dictionary<int, UnitRecruitData> m_recruitDictionary;
    public UnitRecruitData m_outUnitRecruitData;

    void Awake()
    {
        //m_isLoaded = false;
        m_recruitDictionary = new Dictionary<int, UnitRecruitData>();
        LoadFromCsv();
        
    }
    
    public UnitRecruitData GetRecruitData(int nRecruitID)
    {
        if(m_recruitDictionary.TryGetValue(nRecruitID, out m_outUnitRecruitData) != false)
        {
            return m_outUnitRecruitData;
        }

        Debug.Log("couldn't find unitRecruitID Data");
        return null;
    }
    
    public void LoadFromCsv()
    {
        m_recruitDictionary.Clear();
        
        string tempPath;
        tempPath = m_recruitInfoPath.text;

        string[] tempLines;
        tempLines = tempPath.Split('\n');

        for (int i = 1; i < tempLines.Length; i++)
        {
            string tempLine;
            tempLine = tempLines[i].Trim();

            string[] cols;
            cols = tempLine.Split(',');

            string recruitId = cols[0].Trim();
            string unitName = cols[1].Trim();
            string unitType = cols[2].Trim();
            string unitName_en = cols[3].Trim();
            string unitType_en = cols[4].Trim();
            string portrait_image_path = cols[5].Trim();
            string cost_gold = cols[6].Trim();
            string cost_honor = cols[7].Trim();
            string basicTraitName = cols[8].Trim();
            string basicTrait_image_path = cols[9].Trim();

            UnitRecruitData newData = new UnitRecruitData();
            newData.m_recruitID = int.Parse(recruitId);
            newData.m_unitName = unitName;
            newData.m_unitType = unitType;
            newData.m_unitName_en = unitName_en;
            newData.m_unitType_en = unitType_en;

            Sprite sp_portrait;
            sp_portrait = Resources.Load<Sprite>(portrait_image_path);
            newData.m_portrait_image = sp_portrait;
            
            newData.m_cost_gold = int.Parse(cost_gold);
            newData.m_cost_honor = int.Parse(cost_honor);
            newData.m_basicTraitName = basicTraitName;

            Sprite sp_basicTrait;
            sp_basicTrait = Resources.Load<Sprite>(basicTrait_image_path);
            newData.m_basicTrait_image = sp_basicTrait;

            m_recruitDictionary.Add(int.Parse(recruitId), newData);
        }

        //Debug.Log("load unitRecruitData success");
    }
    
}
