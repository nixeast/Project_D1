using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot s_instance;
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private PlayerDataManager m_playerDataManager;
    [SerializeField] private int m_startMissionNumber;

    private void Awake()
    {
        MakeGameRootSingleTonPtn();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerData();
    }

    public void SetMissionNumber(int nMissionNumber)
    {
        m_startMissionNumber = nMissionNumber;
    }

    private void LoadPlayerData()
    {
        if (m_playerDataManager != null)
        {
            m_playerData = m_playerDataManager.GetPlayerData();
        }
    }

    private void MakeGameRootSingleTonPtn()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public PlayerData GetPlayerData()
    {
        return m_playerData;
    }

}
