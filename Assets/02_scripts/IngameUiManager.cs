using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameUiManager : MonoBehaviour
{
    public GameObject panel_openingScreen;
    public GameObject panel_tileInfo;
    public GameObject panel_unitcard;

    [Header("Mision Data InFo")]
    public int m_missionNumber;
    public GameObject panel_missionBrief;
    public TMP_Text text_missionName;
    public TMP_Text text_missionObjective;

    [Header("Uniy Deployment")]
    public GameObject panel_deployPhase;

    // Start is called before the first frame update
    void Start()
    {
        LoadMissionInfo();
        SetMissionBriefContent(m_missionNumber);


    }

    public void OnClickBattleStart()
    {
        panel_deployPhase.SetActive(false);
    }

    public void LoadMissionInfo()
    {
        if(GameRoot.s_instance == null)
        {
            return;
        }
        m_missionNumber = GameRoot.s_instance.GetStartMissionNumber();
    }

    public void SetMissionBriefContent(int nMissionNumber)
    {
        if (GameRoot.s_instance == null)
        {
            return;
        }

        MissionData newData;
        bool isSuccess = false;
        isSuccess = MissionDatabase.s_instance.m_missionDataDic.TryGetValue(nMissionNumber, out newData);
        if(isSuccess == true)
        {
            text_missionName.text = newData.m_missionName;
            text_missionObjective.text = newData.m_missionObjective;

        }
        else
        {
            Debug.Log("Load mission info from missionDatabase failed..");
        }
    }

    public void OnClickToContinue()
    {
        panel_openingScreen.SetActive(false);
    }

    public void OnClickCloseMissionBriefPanel()
    {
        panel_missionBrief.SetActive(false);
        panel_deployPhase.SetActive(true);
    }
}
