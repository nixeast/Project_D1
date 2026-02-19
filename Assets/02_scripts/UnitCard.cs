using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    [SerializeField]
    private Button m_infoButton;
    private UIManager m_uiManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitUnitCard(UIManager uiManager)
    {
        m_uiManager = uiManager;
        m_infoButton.onClick.AddListener(OnInfoButtonClicked);
    }

    public void OnInfoButtonClicked()
    {
        m_uiManager.ShowCharacterInfoPanel();
    }
}
