using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonTile : MonoBehaviour
{
    public GameManager m_gameManager;
    public IngameUiManager m_ingameUiManager;
    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameManager.instance;
        m_ingameUiManager = m_gameManager.m_ingameUiManager;
    }

    public void OnMouseDown()
    {

        //Debug.Log("common tile clicked");
        //m_ingameUiManager.ShowTileInfoPanel();
        //m_ingameUiManager.obj_currentSelectedTile = this.gameObject;
        //m_ingameUiManager.RefreshTileInfoPanelContent();

    }
}
