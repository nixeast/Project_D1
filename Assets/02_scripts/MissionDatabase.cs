using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDatabase : MonoBehaviour
{
    public static MissionDatabase s_instance;

    public TextAsset csv_missionData;
    public Dictionary<int, MissionData> m_missionDataDic = new Dictionary<int, MissionData>();

    private void Awake()
    {
        MakeSingletonPattern();

        m_missionDataDic.Clear();
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

    public void LoadFromCsv()
    {
        m_missionDataDic.Clear();
        string csvPath = csv_missionData.text;
        string[] dataLines = csvPath.Split('\n');
        int nLineCount = dataLines.Length;

        for (int i = 1; i < nLineCount; i++)
        {
            string dataLine = dataLines[i].Trim();
            string[] dataColums = dataLine.Split(',');
            MissionData newData = new MissionData();
            newData.m_missionID = int.Parse(dataColums[0]);
            newData.m_doomName = dataColums[1];
            newData.m_doomName_en = dataColums[2];
            newData.m_doomIcon_path = dataColums[3];
            newData.m_missionTheme = dataColums[4];
            newData.m_missionTitle_en = dataColums[5];
            newData.m_missionName = dataColums[6];
            newData.m_missionDescription = dataColums[7];
            newData.m_missionObjective = dataColums[8];
            newData.m_missionDifficulty = dataColums[9];
            newData.m_missionGimmick = dataColums[10];
            newData.missionGimmick_desc = dataColums[11];
            newData.m_enemyTotalCount = int.Parse(dataColums[12]);

            string[] arr_enemyList = dataColums[13].Split('/');
            int nCount = arr_enemyList.Length;
            newData.m_enemyList_ID = new string[nCount];

            for (int j = 0; j < nCount; j++)
            {
                newData.m_enemyList_ID[j] = arr_enemyList[j];
            }
            //newData.m_enemyList_ID = dataColums[13];
            m_missionDataDic.Add(newData.m_missionID, newData);

        }

        //Debug.Log("Load from csv_missionData complete..");
    }
}
