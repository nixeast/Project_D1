using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public Unit assignedUnit;
    public GameManager m_gameManager;

    public void Start()
    {
        m_gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(m_gameManager.m_currentSelectedUnit.gameObject.tag == "enemy" && other.CompareTag("Player"))
        {
            assignedUnit = other.gameObject.GetComponent<Unit>();
        }
        else if(m_gameManager.m_currentSelectedUnit.gameObject.tag == "Player" && other.CompareTag("enemy"))
        {
            assignedUnit = other.gameObject.GetComponent<Unit>();
        }
    }

    private void OnMouseDown()
    {
        if (assignedUnit != null)
        {
            m_gameManager.AttackUnit(assignedUnit);

        }

    }
}
