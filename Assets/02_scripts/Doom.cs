using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum eDoomLevel
{
    Default = 0,
    Stable = 1,
    Low = 2,
    Threat = 3,
    Critical = 4,
}

public class Doom : MonoBehaviour
{
    public string m_doomName;
    public int m_doomValue;
    public Image m_doomLevelBar;
    public eDoomLevel m_doomLevel;
    public TMP_Text text_doomLevel;

    void Awake()
    {
        m_doomValue = 0;
        m_doomLevel = eDoomLevel.Stable;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void UpdateDoom()
    {
        UpdateDoomLevelBar();
        UpdateDoomLevel();
    }
    
    public void IncreaseDoomValue(eDoomType doomType, int Amount)
    {
        int nDoomNumber = (int)doomType;
        m_doomValue += Amount;
    }
    
    public void DecreaseDoomValue(eDoomType doomType, int Amount)
    {
        int nDoomNumber = (int)doomType;
        m_doomValue -= Amount;
    }
    
    public void UpdateDoomLevelBar()
    {
        m_doomLevelBar.fillAmount = m_doomValue * 0.01f;
    }
    
    public void UpdateDoomLevel()
    {
        if(m_doomValue < 25)
        {
            m_doomLevel = eDoomLevel.Stable;
            text_doomLevel.text = "Stable";
        }
        else if(m_doomValue < 50)
        {
            m_doomLevel = eDoomLevel.Low;
            text_doomLevel.text = "Low";
        }
        else if(m_doomValue < 75)
        {
            m_doomLevel = eDoomLevel.Threat;
            text_doomLevel.text = "Threat";
        }
        else
        {
            m_doomLevel = eDoomLevel.Critical;
            text_doomLevel.text = "Critical";
        }
    }
    
    public void AddDoomEffect()
    {
        if(m_doomLevel == eDoomLevel.Stable)
        {
            //add purifyingBreath
        }
        else if(m_doomLevel == eDoomLevel.Threat)
        {
            //add plagueTile
        }
        else if(m_doomLevel == eDoomLevel.Critical)
        {
            //add ringOfCorruption, plagueSpread
        }
    }

    
}
