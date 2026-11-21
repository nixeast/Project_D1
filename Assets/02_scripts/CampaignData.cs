using System;
using System.Collections.Generic;

public enum eCampaignState
{
    InProgress = 0,
    GameOver = 1,   // 버민킨 대공세 패배
    Completed = 2,  // 50일차 '왕의 관문' 승리
}

// 캠페인 진행 상태. PlayerData에 포함되어 함께 저장됩니다. (GDD 03 캠페인 §3)
[Serializable]
public class CampaignData
{
    public bool m_isStarted = false;        // 미션 데이터가 준비된 뒤 처음 접근할 때 시작
    public int m_phase = 1;                 // 현재 둠 페이즈 (1~5)
    public int m_daysUsedInPhase = 0;       // 현재 페이즈에서 지난 일수 (0~10)
    public int m_threat = 0;                // 위협도 게이지 (0~100%)
    public bool m_isAssaultPending = false; // 페이즈가 끝나 대공세만 남은 상태
    public eCampaignState m_state = eCampaignState.InProgress;

    public List<int> m_phaseMissionIDs = new List<int>();   // 이번 페이즈에 발생한 둠 미션
    public List<int> m_clearedMissionIDs = new List<int>(); // 이번 페이즈에 클리어한 둠 미션
    public List<int> m_failedMissionIDs = new List<int>();  // 이번 페이즈에 패배로 사라진 둠 미션

    // 마지막 전투 결과 문구 (결과 화면 표시용)
    public string m_lastResultMessage = "";
}
