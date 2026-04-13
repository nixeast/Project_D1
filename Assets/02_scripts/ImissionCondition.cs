using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ImissionCondition
{
    void Initialize();
    bool CheckVictory();
    bool CheckDefeat();

}

public class DefenseCondition : ImissionCondition
{
    private int _targetTurn;
    //private int _currentTurn;
    public GameManager m_gameManager;

    public DefenseCondition()
    {
        // Debug.Log("Defense condition set..");
        //_targetTurn = target;
        Initialize();
    }

    public void Initialize()
    {
        //_currentTurn = 1;
        _targetTurn = 3;
        m_gameManager = GameManager.instance;
    }

    public bool CheckVictory()
    {
        bool condition_01 = false;
        bool condition_02 = false;

        if (m_gameManager.m_currentTurn >= _targetTurn)
        {
            condition_01 = true;
        }

        if (m_gameManager.m_currentEnemyUnitCount <= 0)
        {
            condition_02 = true;
            Debug.Log("defense mission condition_02 true");
        }

        if (condition_01 == true && condition_02 == true)
        {
            m_gameManager.BattleWin();
            return true;
        }

        return false;
    }

    public bool CheckDefeat()
    {

        if(m_gameManager.m_currentPlayerUnitCount <= 0)
        {
            m_gameManager.BattleLose();
            return true;
        }

        return false;
    }

}
