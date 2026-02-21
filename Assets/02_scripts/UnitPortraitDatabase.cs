using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPortraitDatabase : MonoBehaviour
{

    [SerializeField]
    private TextAsset m_portraitPathCsv;

    private Dictionary<string,Sprite> m_portraitByName;
    //private bool m_isLoaded;

    public Sprite tempSP;

    void Awake()
    {
        //m_isLoaded = false;
        m_portraitByName = new Dictionary<string, Sprite>(128);
        LoadFromCsv();
        
    }

    public void LoadFromCsv()
    {
        m_portraitByName.Clear();
        string tempText;
        tempText = m_portraitPathCsv.text;

        string[] tempLines;
        tempLines = tempText.Split('\n');

        for(int i = 0 ; i < tempLines.Length ; i++)
        {
            string tempLine;
            tempLine = tempLines[i].Trim();

            string[] cols;
            cols = tempLine.Split(',');

            string unitName;
            string portraitPath;
            unitName = cols[1].Trim();
            portraitPath = cols[2].Trim();

            Sprite sp;
            sp = Resources.Load<Sprite>(portraitPath);

            m_portraitByName.Add(unitName,sp);
        }

        //m_isLoaded = true; 
        //Debug.Log("load from csv completed..");

    }

    public Sprite GetPortraitSprite(string currentUnitName)
    {
        if(m_portraitByName.TryGetValue(currentUnitName, out tempSP) != false)
        {
            return tempSP;
        }

        Debug.Log("couldn't find portraitByName : " + currentUnitName);
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
