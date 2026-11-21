using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 위협도 게이지 표시 (GDD 03 §3.2). 이전의 둠 테마별 수치 구조는 단일 게이지로 바뀌었습니다.
// 씬 참조를 유지하기 위해 클래스·필드 이름은 그대로 둡니다.
public class Doom : MonoBehaviour
{
    public TMP_Text text_doomName;   // 게이지 제목
    public string m_doomName = "위협도";
    public Image m_doomLevelBar;     // 게이지 막대
    public TMP_Text text_doomLevel;  // 수치 (예: 40%)

    public void SetThreat(int threatPercent)
    {
        if (text_doomName != null) text_doomName.text = m_doomName;
        if (m_doomLevelBar != null) m_doomLevelBar.fillAmount = Mathf.Clamp01(threatPercent * 0.01f);
        if (text_doomLevel != null) text_doomLevel.text = threatPercent + "%";
    }
}
