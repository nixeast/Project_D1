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
    private int _currentTurn;

    public DefenseCondition()
    {
        Debug.Log("Defense condition set..");
        //_targetTurn = target;
    }

    public void Initialize()
    {
        _currentTurn = 1;
    }

    public bool CheckVictory()
    {
        if(GameManager.instance.m_currentTurn == 6)
        {
            GameManager.instance.BattleWin();
            return true;
        }

        return false;
    }

    public bool CheckDefeat()
    {
        return false;
    }

}
