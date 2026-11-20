using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class SkillData
{
    public string m_nameEn;
    public string m_nameKo;
    public string m_skillType;   // Trait / Active
    public string m_faction;     // Dwarf / Verminkin
    public string m_description;
    public string m_iconPath;
}

// 특성·스킬 테이블 (Assets/Resources/data_skill.csv).
// 씬에 배치하지 않고 처음 사용할 때 Resources에서 읽어옵니다.
public static class SkillDataBase
{
    public const string NONE = "(none)";
    private const string RESOURCE_PATH = "data_skill";

    private static Dictionary<string, SkillData> s_skillDic;

    public static Dictionary<string, SkillData> SkillDic
    {
        get
        {
            if (s_skillDic == null)
            {
                Load();
            }
            return s_skillDic;
        }
    }

    public static SkillData Get(string nameEn)
    {
        if (string.IsNullOrEmpty(nameEn) || nameEn == NONE) return null;

        SkillData data;
        if (SkillDic.TryGetValue(nameEn, out data) == false)
        {
            Debug.LogWarning("no skill data: " + nameEn);
            return null;
        }
        return data;
    }

    public static Sprite GetIcon(string nameEn)
    {
        SkillData data = Get(nameEn);
        if (data == null || data.m_iconPath == NONE) return null;
        return Resources.Load<Sprite>(data.m_iconPath);
    }

    public static void Load()
    {
        s_skillDic = new Dictionary<string, SkillData>();
        TextAsset csv = Resources.Load<TextAsset>(RESOURCE_PATH);
        if (csv == null)
        {
            Debug.LogError("skill csv not found: Resources/" + RESOURCE_PATH);
            return;
        }

        List<List<string>> rows = ParseCsv(csv.text);
        if (rows.Count == 0) return;

        Dictionary<string, int> col = new Dictionary<string, int>();
        for (int h = 0; h < rows[0].Count; h++)
        {
            col[rows[0][h].Trim('﻿')] = h;
        }

        for (int i = 1; i < rows.Count; i++)
        {
            List<string> r = rows[i];
            if (r.Count < col.Count) continue;

            SkillData data = new SkillData();
            data.m_nameEn = r[col["skillName_en"]];
            data.m_nameKo = r[col["skillName_ko"]];
            data.m_skillType = r[col["skillType"]];
            data.m_faction = r[col["faction"]];
            data.m_description = r[col["description"]];
            data.m_iconPath = r[col["icon_path"]];
            s_skillDic[data.m_nameEn] = data;
        }
    }

    // 큰따옴표로 감싼 필드(쉼표 포함)를 지원하는 간단한 CSV 파서
    private static List<List<string>> ParseCsv(string text)
    {
        List<List<string>> rows = new List<List<string>>();
        List<string> row = new List<string>();
        StringBuilder field = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                row.Add(field.ToString());
                field.Length = 0;
            }
            else if (c == '\n' || c == '\r')
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(field.ToString());
                field.Length = 0;
                rows.Add(row);
                row = new List<string>();
            }
            else
            {
                field.Append(c);
            }
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }
        return rows;
    }
}
