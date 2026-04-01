using UnityEngine;
using System.Collections.Generic;

// --- C의 typedef struct와 유사한 데이터 정의 ---
[System.Serializable]
public struct TileInfo
{
    public int x, y;      // 좌표
    public int typeID;    // 타일 종류 (0: 흙, 1: 돌 등)
}

[System.Serializable]
public struct UnitInfo
{
    public int x, y;      // 배치 좌표
    public int unitID;    // 유닛 종류 (101: 버민킨 병사 등)
}

// --- 실제 데이터 파일이 될 클래스 (데이터 컨테이너) ---
[CreateAssetMenu(fileName = "NewStage", menuName = "ProjectD/Stage")]
public class StageData : ScriptableObject
{
    public string stageName;
    public int mapWidth, mapHeight;

    // C의 동적 배열 대신 List 사용
    public List<TileInfo> tileList = new List<TileInfo>();
    public List<UnitInfo> unitList = new List<UnitInfo>();
}