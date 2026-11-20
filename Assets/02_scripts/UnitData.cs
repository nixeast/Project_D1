using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitData
{
    public const int TRAIT_SLOT_COUNT = 6;
    public const int ACTIVE_SLOT_COUNT = 3;

    public int m_UnitID;
    public string m_UnitName;
    public string m_unitType;
    public int m_unitTier;
    public string m_PortraitPath;
    public int m_level;
    public int m_stat_HP;
    public int m_stat_ATK;
    public int m_stat_DEF;
    public int m_stat_HIT;
    public int m_stat_EVA;
    public int m_stat_MOV;

    // 특성 6칸, 액티브 3칸. 값은 data_skill.csv의 skillName_en (빈 칸은 "(none)")
    public string[] m_traits = new string[TRAIT_SLOT_COUNT];
    public string[] m_activeSkills = new string[ACTIVE_SLOT_COUNT];

    public string m_equip_weapon;
    public string m_equip_armor;
    public string m_equip_accessary;
    public string m_equip_weapon_path;
    public string m_equip_armor_path;
    public string m_equip_accessary_path;

    public string m_unitIcon_path;
    public int m_stat_attackRange;

}
