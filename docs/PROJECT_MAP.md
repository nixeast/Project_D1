# Project_D1 프로젝트 맵

드워프(플레이어) vs 버민킨(적) 세력 구도의 **Unity 2D SRPG** 프로젝트 참조 문서.

- Unity 버전: **6000.3.7f1** (Unity 6)
- 주요 패키지: 2D Feature, Tilemap, TextMeshPro, uGUI, Input System
- 데이터 저장: `Application.persistentDataPath/playerData.json` (JsonUtility)

> 기획 내용은 [GDD](GDD/README.md)를 참고하세요.
>
> 이 문서는 폴더·파일 위치를 빠르게 찾기 위한 지도입니다. 구조가 바뀌면 함께 갱신하세요.

---

## 1. 루트 구조

```
Project_D1/
├─ Assets/            ← 실제 작업 영역 (아래 상세)
├─ Packages/          ← manifest.json (패키지 의존성)
├─ ProjectSettings/   ← 빌드 씬 목록, 태그/레이어 등
├─ docs/              ← 프로젝트 문서 (이 파일)
├─ Library/ Logs/ obj/ UserSettings/ .vs/   ← 자동 생성, git 제외
└─ Project_D1.sln, *.csproj                 ← IDE 솔루션 (자동 생성)
```

---

## 2. Assets 폴더 개요

번호 접두사로 정렬되어 있습니다.

| 폴더 | 내용 |
|---|---|
| `00_scenes/` | 씬 파일 |
| `01_sprites/` | 원본 스프라이트 (타일, 아이콘, UI 등) |
| `02_scripts/` | 런타임 C# 스크립트 전체 (하위 폴더 없음) |
| `03_Palette/` | Tile Palette 에셋 |
| `04_Prefab/` | UI 프리팹, 맵 타일·유닛 프리팹 |
| `05_csv/` | 게임 데이터 테이블 (CSV) |
| `06_StageData/` | 스테이지 ScriptableObject (`StageData`) |
| `Editor/` | 에디터 확장 스크립트 |
| `Resources/` | `Resources.Load`로 런타임 로딩하는 이미지 |
| `TextMesh Pro/` | TMP 기본 에셋 (서드파티) |

---

## 3. 씬 (`00_scenes/`)

| 씬 | 빌드 포함 | 역할 | 배치된 주요 컴포넌트 |
|---|---|---|---|
| `SampleScene.unity` | O | 기본 템플릿 씬 (사용 안 함) | - |
| `sc_00_lobby.unity` | O | 캠페인/로비: 미션 선택, 모집, 유닛 정보, 창고, 둠(Doom) | `GameRoot`, `PlayerDataManager`, `UIManager`, `Doom`, `MissionDatabase`, `UnitDataBase`, `UnitRecruitDataBase`, `UnitPortraitDatabase` |
| `sc_01_battleMap.unity` | O | 전투: 배치 → 턴제 전투 → 결과 | `GameManager`, `StageManager`, `IngameUiManager`, `BattleResultManager`, `CameraMovement`, `UnitDataBase`, `UnitPortraitDatabase` |
| `sc_02_mapCreator.unity` | **X** | 맵 제작용 작업 씬 (타일 배치 후 `StageData`로 저장) | `GameManager`, `StageManager`, `BattleResultManager`, `UnitDataBase`, `UnitPortraitDatabase` |

**씬 전환 흐름**

```
sc_00_lobby ──(UIManager.gotoBattleMapScene)──▶ sc_01_battleMap
     ▲                                              │
     └──(BattleResultManager / UIManager.gotoLobbyScene)──┘
```

`GameRoot`(DontDestroyOnLoad 싱글턴)가 로비에서 선택한 **미션 번호**와 **PlayerData**를 전투 씬으로 넘겨줍니다.

---

## 4. 스크립트 (`02_scripts/`)

### 4.1 코어 / 매니저

| 파일 | 줄 수 | 역할 |
|---|---:|---|
| `GameRoot.cs` | 57 | 씬 간 유지되는 싱글턴. 시작 미션 번호, PlayerData 전달 |
| `PlayerDataManager.cs` | 288 | 싱글턴. 플레이어 데이터 생성/저장/로드/삭제(JSON), 자원(골드·명예·Forged Essence), 유닛 생성, 창고 아이템 관리 |
| `GameManager.cs` | 1070 | **전투 핵심**. 게임 상태(`eGamePlayState`), 턴(`eTurnOwner`), 이동 범위 탐색(`FindMovableArea`), 공격·명중·반격, 전투 시퀀스, 적 AI 명령, 적 스폰, 승패 판정 |
| `StageManager.cs` | 140 | `StageData`를 읽어 지형/유닛/영역/오브젝트 타일 프리팹을 생성 |
| `UIManager.cs` | 913 | **로비 UI 전체**. 메뉴 패널, 미션 정보, 모집, 유닛 카드/정보 패널, 장비 슬롯, 창고, 씬 이동 |
| `IngameUiManager.cs` | 411 | **전투 UI**. 페이즈(`ePhaseType`), 배치 UI, 전투 예상창, 타일 정보, 미션 브리핑 |
| `BattleResultManager.cs` | 75 | 전투 결과 화면, 로비 복귀 |
| `Doom.cs` / `DoomEffect.cs` | 149 / 14 | 캠페인 위협도(둠) 수치·레벨(`eDoomLevel`)과 효과 |
| `CameraMovement.cs` | 20 | 전투 맵 카메라 이동 |

### 4.2 전투 맵 오브젝트

| 파일 | 역할 |
|---|---|
| `Unit.cs` | 맵 위 유닛. 조작 모드(`eUnitControlMode`), 상태(`eUnitState`), 스탯 설정, 배치, 클릭 처리, 사망 체크 |
| `CommonTile.cs` | 타일 공통 컴포넌트 |
| `MoveTarget.cs` | 이동 가능 표시 타일 (클릭 시 이동) |
| `AttackTarget.cs` | 공격 가능 표시 타일 |
| `EnemySpawnTile.cs` | 적 스폰 지점 (지정 턴에 스폰) |
| `StartingPointButton.cs` | 아군 배치 지점 |

### 4.3 데이터 정의 (순수 클래스 / SO)

| 파일 | 역할 |
|---|---|
| `PlayerData.cs` | 저장 대상 루트 데이터 (유닛 목록, 자원, 창고) |
| `UnitSaveData.cs` | 저장용 유닛 데이터 |
| `StorageSaveData.cs` | 저장용 창고 슬롯 데이터 |
| `UnitData.cs` | CSV에서 읽은 유닛 원본 데이터 |
| `UnitRecruitData.cs` | 모집 정보 데이터 |
| `MissionData.cs` | 미션 정보 데이터 |
| `Item.cs` | 아이템 데이터 |
| `StageData.cs` | 스테이지 ScriptableObject (`Create > ProjectD > Stage`). 지형/유닛/영역/오브젝트 리스트 |
| `ImissionCondition.cs` | 미션 승리조건 인터페이스 + `DefenseCondition` 구현 |

### 4.4 데이터베이스 (CSV 로더)

| 파일 | 읽는 CSV | 비고 |
|---|---|---|
| `UnitDataBase.cs` | `data_unit.csv` | 유닛 스탯·스킬·장비·아이콘 경로 |
| `UnitRecruitDataBase.cs` | `data_recruitInfo.csv` | 모집 가능 유닛 |
| `MissionDatabase.cs` | `data_mission.csv` | 미션 목록, `eMissionType` 정의 |
| `UnitPortraitDatabase.cs` | `data_unitPortrait.csv` | 유닛 ID → 초상화 스프라이트 |

### 4.5 UI 컴포넌트

| 파일 | 역할 |
|---|---|
| `UnitCard.cs` | 로비/전투 공용 유닛 카드 |
| `PlayerUnitCard.cs` | 배치 화면 아군 유닛 카드 |
| `EnemyUnitCard.cs` | 미션 정보의 적 유닛 카드 |
| `RecruitCard.cs` | 모집 카드 |
| `ItemSlotButton.cs` | 유닛 장비 슬롯 |
| `StorageSlotButton.cs` | 창고 슬롯 |

### 4.6 에디터 (`Editor/`)

| 파일 | 역할 |
|---|---|
| `StageDataEditor.cs` | `StageData` 인스펙터에 **SAVE SCENE TO DATA** 버튼 추가. 현재 씬의 태그(`terrain` 등) 오브젝트를 읽어 좌표와 ID를 SO에 기록 |

### 4.7 주요 enum 위치

| enum | 파일 | 값 |
|---|---|---|
| `eGamePlayState` | GameManager.cs | SetupBattleUnit, Battle, BattleResult |
| `eTurnOwner` | GameManager.cs | Player, enemy |
| `ePhaseType` | IngameUiManager.cs | UnitDeplyment, Battle |
| `eUnitControlMode`, `eUnitState` | Unit.cs | 유닛 조작/상태 |
| `eMissionType` | MissionDatabase.cs | Defense, Extraction, Explore, Destroy, Detonation, Elimination … |
| `eDoomType` | UIManager.cs | PlagueOfContamination, RiftStoneMutation, SubterraneanTunnels, SoulHarvest, GearsOfCalamity |
| `eDoomLevel` | Doom.cs | Stable, Low, Threat, Critical … |
| `eDataUpdateType` | UIManager.cs | Add, Remove |

---

## 5. 데이터 (`05_csv/`, `06_StageData/`)

| 파일 | 행 수 | 주요 컬럼 |
|---|---:|---|
| `data_unit.csv` | 40 | UnitID, UnitName, unitType, stat_HP/ATK/DEF/HIT/EVA/AP, trait·skill(이름+아이콘 경로), equip(이름+경로), path_icon, stat_mov, stat_atkRange |
| `data_mission.csv` | 20 | missionID, doomName, missionTheme, missionName/Description/Objective, missionDifficulty, missionGimmick, enemyTotalCount, enemyList_ID(`/` 구분), missionType |
| `data_recruitInfo.csv` | 12 | recruitID, unitName, unitType, img_portrait, cost_gold, cost_honor, basicTrait, img_basicTrait |
| `data_unitPortrait.csv` | 16 | UnitID, UnitName, PortraitPath |
| `06_StageData/stage_01.asset` | - | 스테이지 1 맵 데이터 (StageData SO) |

- CSV는 `TextAsset`으로 인스펙터에 연결 후 `,`/`\n`으로 직접 파싱합니다. **본문에 쉼표를 넣으면 컬럼이 밀립니다.**
- CSV 안의 경로 값은 모두 `Resources/` 기준 상대 경로(확장자 없음)입니다.

---

## 6. 프리팹 (`04_Prefab/`)

### 6.1 `00_GameUI/` – UI 프리팹

`Button_deploy`, `GameObject_recruitCard`, `Panel_characterInfo`, `Panel_characterList`, `Panel_expeditionRecruit`, `Panel_unitCard`, `btn_enemyUnitCard`, `btn_resultUnitSlot`, `btn_storageSlot`, `icon_closeCombat`, `test_unitCard`, `ui_deployment/btn_playerUnitCard`

### 6.2 `01_Tiles/` – 맵 타일 프리팹

파일명 규칙: **`tile_<ID>_<분류>_<이름>.prefab`**. `StageManager` / `StageDataEditor`가 이름에서 ID를 추출해 사용합니다.

| ID 대역 | 하위 폴더 | 분류 | 예시 |
|---|---|---|---|
| 001~099 | `tile_terrain/` | 지형 | 001 stoneGround, 002 brickGround |
| 100~199 | `tile_unit_verminkin/` | 적 유닛 | 100 VerminFootman ~ 107 PustuleAbomination |
| 200~299 | `tile_area/` | 영역 | 200 deployPoint, 201 enemySpawnPoint (+ 이동/공격 표시 타일) |
| 300~399 | `tile_object/` | 오브젝트 | 300 PurificationEngine, 301 Boulder |
| - | `tile_unit_player/` | 아군 유닛 | tile_unit_playerUnit |
| - | `etc_test/` | 테스트 | unit_enemy |

---

## 7. 이미지 리소스

### 7.1 `01_sprites/` – 직접 참조용 스프라이트

| 폴더 | 내용 |
|---|---|
| `icon/`, `item/`, `object/`, `tile/` | 아이콘, 아이템, 오브젝트, 타일 이미지 |
| `ui_battle/` | 전투 UI |
| `ui_campaign/00_mockups` ~ `08_unitStatPanel` | 로비 UI (목업, 메인, 둠 패널, 자원 패널, 사이드 메뉴, 옵션, 이벤트 메시지, 전투 준비, 유닛 스탯) |

### 7.2 `Resources/` – 코드에서 경로로 로딩

| 폴더 | 내용 |
|---|---|
| `res_portrait_dwarf/`, `res_portrait_verminkin/` | 전투/모집 초상화 |
| `res_icon_dwarfUnit/`, `res_icon_verminkinUnit/` | 유닛 아이콘 |
| `res_icon_trait/` | 특성·스킬 아이콘 |
| `res_icon_gear/` | 장비 아이콘 |
| `res_icon_dooms/` | 둠 아이콘 |
| `res_icon_object/` | 오브젝트 아이콘 |
| `ui_campaign_recruit/`, `ui_campaign_unitInfo/`, `ui_doomPanel/`, `ui_dwarfResources/` | 로비 UI 동적 이미지 |
| `background/`, `font/`, `sample_ui/` | 배경, 폰트, 샘플 UI |

> `Resources/` 안의 파일명이나 경로를 바꾸면 CSV의 경로 컬럼도 함께 고쳐야 합니다.

---

## 8. 기타

- `03_Palette/tile_palette/`: Tile Palette 에셋 (`tile_ground_normal_01/02`, `tile_ground_buff_01`, `tile_ground_debuff_01`, `test_wall`)
- 맵 제작 절차: `sc_02_mapCreator`에서 타일 프리팹 배치 → `06_StageData`의 StageData 선택 → 인스펙터의 **SAVE SCENE TO DATA** 클릭 → 전투 씬의 `StageManager.GenerateStage()`가 생성

---

## 9. 빠른 찾기 (작업 → 파일)

| 하고 싶은 작업 | 볼 곳 |
|---|---|
| 이동 범위 / 길찾기 | `GameManager.FindMovableArea`, `CheckNeighborTile` |
| 공격·명중·반격 | `GameManager.AttackUnit`, `CheckAttackHit`, `CheckCounterAttack`, `StartCombatSequence` |
| 적 AI | `GameManager.CommandEenmyUnits`, `AssisgnAttackTarget`, `CommandEnemyToFindMovePos` |
| 턴 진행 | `GameManager.OnClickEndTurn`, `SwitchTurnOwner`, `IncreaseTurn` |
| 승리 조건 추가 | `ImissionCondition.cs`, `GameManager.InitMissionCondition` |
| 유닛 스탯 수정 | `05_csv/data_unit.csv` → `UnitDataBase.cs` |
| 미션 추가 | `05_csv/data_mission.csv` → `MissionDatabase.cs` |
| 모집 유닛 | `05_csv/data_recruitInfo.csv`, `UIManager`의 Recruit 관련 메서드 |
| 세이브/로드 | `PlayerDataManager.SavePlayerData` / `LoadPlayerData` |
| 맵 생성 | `StageManager.GenerateStage`, `Editor/StageDataEditor.cs` |
| 전투 UI | `IngameUiManager.cs` |
| 로비 UI | `UIManager.cs` |
