using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

[CustomEditor(typeof(StageData))]
public class StageDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 기본 UI 출력

        StageData data = (StageData)target;

        if (GUILayout.Button("SAVE SCENE TO DATA", GUILayout.Height(40)))
        {
            SaveToData(data);
        }
    }

    // --- 데이터를 추출하는 핵심 로직 (절차적 함수) ---
    void SaveToData(StageData data)
    {
        // 1. 기존 리스트 비우기 (C의 메모리 초기화 느낌)
        data.tileList.Clear();
        data.unitList.Clear();

        // 2. 씬에 있는 모든 게임 오브젝트를 훑음
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject obj = allObjects[i];

            // 타일 저장 (이름이나 태그로 판별)
            if (obj.CompareTag("groundTile"))
            {
                TileInfo t;
                t.x = (int)obj.transform.position.x;
                t.y = (int)obj.transform.position.y;
                t.typeID = GetIDFromName(obj.name); // 이름 기반으로 ID 할당
                data.tileList.Add(t);
            }
            // 유닛 저장
            else if (obj.CompareTag("enemy"))
            {
                UnitInfo u;
                u.x = (int)obj.transform.position.x;
                u.y = (int)obj.transform.position.y;
                u.unitID = GetIDFromName(obj.name);
                data.unitList.Add(u);
            }
        }

        // 유니ti 에디터에 변경사항 알림 (핵심!)
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log(">> DATA SAVED SUCCESSFULLY.");
    }

    // 단순 문자열 비교를 통해 정수 ID를 반환하는 헬퍼 함수
    int GetIDFromName(string name)
    {
        if (name.Contains("Grass")) return 0;
        if (name.Contains("Stone")) return 1;
        if (name.Contains("Verminkin")) return 101;
        return -1;
    }
}
