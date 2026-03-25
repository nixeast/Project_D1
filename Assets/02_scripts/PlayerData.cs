// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public int nDwarfGold;
    public int nDwarfHonor;
    public int nForgedEssence;

    public UnitSaveData[] currentUnits;
    //public Item[] m_storageItem;
    public StorageSaveData m_storage;

}
