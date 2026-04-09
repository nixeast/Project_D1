using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public Unit m_assignedUnit;
    public GameManager m_gameManager;

    public void Start()
    {
        m_gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            m_assignedUnit = other.gameObject.GetComponent<Unit>();
        }
    }

    public void OnMouseDown()
    {
        if (m_assignedUnit != null)
        {
            m_gameManager.AttackUnit(m_assignedUnit);
        }

    }
}
