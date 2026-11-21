using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionData
{
    public int m_missionID;
    public string m_doomName;
    public string m_doomName_en;
    public string m_doomIcon_path;
    public string m_missionTheme;
    public string m_missionTitle_en;
    public string m_missionName;
    public string m_missionDescription;
    public string m_missionObjective;
    public string m_missionDifficulty;
    public string m_missionGimmick;
    public string missionGimmick_desc;
    public int m_enemyTotalCount;
    public string[] m_enemyList_ID;
    public string m_missionType;
    public List<int> m_waveTurnList;
    public List<string> m_winConditionList;
    public List<string> m_loseConditionList;
    // 둠 미션: 등장하는 둠 페이즈 목록 / 버민킨 대공세: 발생하는 페이즈(페이즈 종료 시)
    public int[] m_appearPhases;

}
