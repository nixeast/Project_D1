using System.Collections.Generic;
using UnityEngine;

// 캠페인 진행 규칙 (GDD 03 캠페인 §3). 상태는 CampaignData(PlayerData.m_campaign)에 저장합니다.
// 수치는 GDD의 초기값이며 밸런싱 때 조정합니다.
public static class Campaign
{
    public const int DaysPerPhase = 10;
    public const int PhaseCount = 5;
    public const int ThreatPerUnclearedMission = 20;   // 페이즈 종료 시 미클리어 둠 미션 1개당
    public const int MaxThreat = 100;
    public const string AssaultDoomNameEn = "Verminkin Great Assault";

    // 대공세 강화 (03 §3.2): 적 수·ATK·DEF만 적용. 정예 교체·기믹 강화는 아직 미적용.
    public const int ThreatPerExtraEnemy = 20;          // 위협도 20%마다 적 +1
    public const int ThreatPerStatStep = 10;            // 위협도 10%마다
    public const int StatBonusPercentPerStep = 2;       // ATK·DEF +2%

    public static CampaignData Current
    {
        get
        {
            if (PlayerDataManager.s_instance == null) return null;
            PlayerData playerData = PlayerDataManager.s_instance.GetPlayerData();
            if (playerData == null) return null;
            if (playerData.m_campaign == null) playerData.m_campaign = new CampaignData();

            // 새 캠페인(또는 캠페인 정보가 없는 이전 세이브)은 미션 데이터가 준비된 뒤 시작합니다.
            if (playerData.m_campaign.m_isStarted == false && MissionDatabase.s_instance != null)
            {
                StartNewCampaign(playerData.m_campaign);
            }
            return playerData.m_campaign;
        }
    }

    // ---------- 미션 정보 ----------

    public static MissionData GetMission(int missionID)
    {
        if (MissionDatabase.s_instance == null) return null;
        MissionData data;
        MissionDatabase.s_instance.m_missionDataDic.TryGetValue(missionID, out data);
        return data;
    }

    public static bool IsAssault(MissionData mission)
    {
        return mission != null && mission.m_doomName_en == AssaultDoomNameEn;
    }

    // 미션 소요 일수 (03 §3.1)
    public static int GetMissionDays(MissionData mission)
    {
        if (mission == null) return 0;
        switch (mission.m_missionDifficulty)
        {
            case "쉬움": return 2;
            case "보통": return 3;
            case "어려움": return 4;
            case "매우 어려움": return 4;
            default: return 3;
        }
    }

    // 둠 미션 클리어 시 위협도 감소량 (03 §3.2)
    public static int GetThreatReduction(MissionData mission)
    {
        if (mission == null) return 0;
        switch (mission.m_missionDifficulty)
        {
            case "쉬움": return 5;
            case "보통": return 10;
            case "어려움": return 15;
            case "매우 어려움": return 20;
            default: return 10;
        }
    }

    public static int GetAssaultMissionID(int phase)
    {
        if (MissionDatabase.s_instance == null) return -1;
        foreach (MissionData mission in MissionDatabase.s_instance.m_missionDataDic.Values)
        {
            if (IsAssault(mission) && ContainsPhase(mission, phase)) return mission.m_missionID;
        }
        return -1;
    }

    static bool ContainsPhase(MissionData mission, int phase)
    {
        if (mission.m_appearPhases == null) return false;
        for (int i = 0; i < mission.m_appearPhases.Length; i++)
        {
            if (mission.m_appearPhases[i] == phase) return true;
        }
        return false;
    }

    // ---------- 날짜 ----------

    public static int GetRemainingDays(CampaignData c)
    {
        return Mathf.Max(0, DaysPerPhase - c.m_daysUsedInPhase);
    }

    // 표시용 현재 날짜 (1일차부터). 대공세 대기 중이면 페이즈 마지막 날(10·20·…일차).
    public static int GetCurrentDay(CampaignData c)
    {
        if (c.m_isAssaultPending) return c.m_phase * DaysPerPhase;
        return (c.m_phase - 1) * DaysPerPhase + Mathf.Min(c.m_daysUsedInPhase + 1, DaysPerPhase);
    }

    // ---------- 미션 선택 ----------

    public static bool IsMissionOpen(CampaignData c, int missionID)
    {
        return c.m_phaseMissionIDs.Contains(missionID)
            && c.m_clearedMissionIDs.Contains(missionID) == false
            && c.m_failedMissionIDs.Contains(missionID) == false;
    }

    // 로비에 표시할 미션 (남은 일수가 모자라는 미션도 표시하되 선택은 막음)
    public static List<int> GetVisibleMissionIDs(CampaignData c)
    {
        List<int> result = new List<int>();
        if (c == null || c.m_state != eCampaignState.InProgress) return result;

        if (c.m_isAssaultPending)
        {
            int assaultID = GetAssaultMissionID(c.m_phase);
            if (assaultID >= 0) result.Add(assaultID);
            return result;
        }

        for (int i = 0; i < c.m_phaseMissionIDs.Count; i++)
        {
            if (IsMissionOpen(c, c.m_phaseMissionIDs[i])) result.Add(c.m_phaseMissionIDs[i]);
        }
        return result;
    }

    public static bool CanSelectMission(CampaignData c, int missionID)
    {
        if (c == null || c.m_state != eCampaignState.InProgress) return false;

        if (c.m_isAssaultPending)
        {
            return missionID == GetAssaultMissionID(c.m_phase);
        }

        if (IsMissionOpen(c, missionID) == false) return false;
        return GetMissionDays(GetMission(missionID)) <= GetRemainingDays(c);
    }

    // ---------- 진행 ----------

    public static void StartNewCampaign(CampaignData c)
    {
        c.m_isStarted = true;
        c.m_threat = 0;
        c.m_state = eCampaignState.InProgress;
        c.m_lastResultMessage = "";
        StartPhase(c, 1);
    }

    static void StartPhase(CampaignData c, int phase)
    {
        c.m_phase = phase;
        c.m_daysUsedInPhase = 0;
        c.m_isAssaultPending = false;
        c.m_phaseMissionIDs.Clear();
        c.m_clearedMissionIDs.Clear();
        c.m_failedMissionIDs.Clear();

        if (MissionDatabase.s_instance != null)
        {
            foreach (MissionData mission in MissionDatabase.s_instance.m_missionDataDic.Values)
            {
                if (IsAssault(mission) == false && ContainsPhase(mission, phase))
                {
                    c.m_phaseMissionIDs.Add(mission.m_missionID);
                }
            }
            c.m_phaseMissionIDs.Sort();
        }

        CheckPhaseEnd(c);
    }

    // 남은 일수로 할 수 있는 미션이 없으면 페이즈를 끝냅니다.
    static void CheckPhaseEnd(CampaignData c)
    {
        if (c.m_isAssaultPending) return;

        int remaining = GetRemainingDays(c);
        for (int i = 0; i < c.m_phaseMissionIDs.Count; i++)
        {
            int id = c.m_phaseMissionIDs[i];
            if (IsMissionOpen(c, id) && GetMissionDays(GetMission(id)) <= remaining) return;
        }

        EndPhase(c);
    }

    // 결산: 클리어하지 못한 둠 미션 1개당 위협도 +20%, 대공세 발생
    static void EndPhase(CampaignData c)
    {
        int uncleared = c.m_phaseMissionIDs.Count - c.m_clearedMissionIDs.Count;
        c.m_threat = Mathf.Clamp(c.m_threat + uncleared * ThreatPerUnclearedMission, 0, MaxThreat);
        c.m_isAssaultPending = true;
    }

    // 탐사 이벤트 '시간 소요' 선택지(+1일)용. 탐사가 구현되면 연결합니다.
    public static void AddExtraDays(CampaignData c, int days)
    {
        if (c == null || c.m_isAssaultPending) return;
        c.m_daysUsedInPhase = Mathf.Min(DaysPerPhase, c.m_daysUsedInPhase + days);
    }

    // 전투 결과를 캠페인에 반영하고 결과 문구를 돌려줍니다.
    public static string ReportBattleResult(int missionID, bool isWin)
    {
        CampaignData c = Current;
        MissionData mission = GetMission(missionID);
        if (c == null || mission == null || c.m_state != eCampaignState.InProgress) return "";

        string message;

        if (IsAssault(mission))
        {
            if (c.m_isAssaultPending == false || missionID != GetAssaultMissionID(c.m_phase)) return "";

            if (isWin == false)
            {
                c.m_state = eCampaignState.GameOver;
                message = "아이언풋 요새가 함락되었습니다.\n게임 오버";
            }
            else if (c.m_phase >= PhaseCount)
            {
                c.m_state = eCampaignState.Completed;
                message = "왕의 관문을 지켜냈습니다.\n캠페인 완료";
            }
            else
            {
                StartPhase(c, c.m_phase + 1);
                message = "대공세를 막아냈습니다.\n둠 페이즈 " + c.m_phase + " 시작";
            }
        }
        else
        {
            if (c.m_isAssaultPending || IsMissionOpen(c, missionID) == false) return "";

            int days = GetMissionDays(mission);
            c.m_daysUsedInPhase = Mathf.Min(DaysPerPhase, c.m_daysUsedInPhase + days);

            if (isWin)
            {
                int reduction = GetThreatReduction(mission);
                c.m_clearedMissionIDs.Add(missionID);
                c.m_threat = Mathf.Clamp(c.m_threat - reduction, 0, MaxThreat);
                message = "미션 클리어 | " + days + "일 경과 | 위협도 -" + reduction + "%";
            }
            else
            {
                c.m_failedMissionIDs.Add(missionID);
                message = "미션 실패 | " + days + "일 경과";
            }

            int threatBefore = c.m_threat;
            CheckPhaseEnd(c);
            if (c.m_isAssaultPending)
            {
                message += "\n둠 페이즈 종료 | 위협도 +" + (c.m_threat - threatBefore) + "% | 버민킨 대공세 발생";
            }
        }

        c.m_lastResultMessage = message;
        return message;
    }

    // ---------- 대공세 강화 ----------

    static int GetCurrentAssaultThreat()
    {
        CampaignData c = Current;
        if (c == null || GameRoot.s_instance == null) return 0;
        if (IsAssault(GetMission(GameRoot.s_instance.GetStartMissionNumber())) == false) return 0;
        return c.m_threat;
    }

    public static int GetExtraEnemyCount(int threat)
    {
        return threat / ThreatPerExtraEnemy;
    }

    public static int GetStatBonusPercent(int threat)
    {
        return threat / ThreatPerStatStep * StatBonusPercentPerStep;
    }

    // 현재 전투가 대공세일 때만 0보다 큰 값
    public static int GetCurrentAssaultExtraEnemyCount()
    {
        return GetExtraEnemyCount(GetCurrentAssaultThreat());
    }

    public static int GetCurrentAssaultStatBonusPercent()
    {
        return GetStatBonusPercent(GetCurrentAssaultThreat());
    }
}
