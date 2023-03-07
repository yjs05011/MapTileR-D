using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMap : TileMapController
{
    private const string TERRAIN_TILEMAP_OBJ_NAME = "TerrainTileMap";
    private Vector2Int mapCellSize = default;
    private Vector2 mapCellGap = default;

    private List<TerrainControler> allTerrains = default;
    //! Awake 타임에 초기할 내용을 재정의 한다.
    public override void InitAwake(MapBoard mapController_)
    {
        this.tileMapObjName = TERRAIN_TILEMAP_OBJ_NAME;
        base.InitAwake(mapController_);

        allTerrains = new List<TerrainControler>();
        //{ 타일의 x축 갯수와 전체 타일의 수로 맵의 가로, 세로 사이즈를 연산한다.
        mapCellSize = Vector2Int.zero;
        float tempTileY = allTileObjs[0].transform.localPosition.y;
        for (int i = 0; i < allTileObjs.Count; i++)
        {
            if (tempTileY.IsEquals(allTileObjs[i].transform.localPosition.y) == false)
            {
                mapCellSize.x = i;
                break;
            } // if: 첫번째 타일의 y 좌표와 달라지는 지점 전까지가 가로 셀 크기이다.
        }
        // 전체 타일의 수를 맵의 가로 셀 크리로 나눈 값이 맵의 세로 셀 크기이다.
        mapCellSize.y = Mathf.FloorToInt(allTileObjs.Count / mapCellSize.x);
        // 타일의 x축 갯수와 전체 타일의 수로 맵의 가로, 세로 사이즈를 연산한다.}


        //{x 축 상의 두 타일과 , y 축 상의 두 타일 사이의 로컬 포지션으로 타일 갭을 연산한다}
        mapCellGap = Vector2.zero;
        mapCellGap.x = allTileObjs[1].transform.localPosition.x - allTileObjs[0].transform.localPosition.x;
        mapCellGap.y = allTileObjs[mapCellSize.x].transform.localPosition.y - allTileObjs[0].transform.localPosition.y;
        //x 축 상의 두 타일과 , y 축 상의 두 타일 사이의 로컬 포지션으로 타일 갭을 연산한다}
    }
    private void Start()
    {
        // {타일맵의 일부를 일정 확률로 다른 타일로 교체하는 로직
        GameObject changeTilePrefab = ResManager.Instance.terrainPrefabs[RDefine.TERRAIN_PREF_OCEAN];
        // 타일맵 중에 어느정도를 바다로 교체할 것인지 결정한다.
        const float CHANGE_PERCENTAGE = 15.0f;
        float correctChangePercentage = allTileObjs.Count * (CHANGE_PERCENTAGE / 100.0f);
        List<int> changeTileResult = GFunc.CreateList(allTileObjs.Count, 1);
        changeTileResult.Shuffle();

        GameObject tempChangeTile = default;

        for (int i = 0; i < allTileObjs.Count; i++)
        {
            if (correctChangePercentage <= changeTileResult[i]) { continue; }
            tempChangeTile = Instantiate(changeTilePrefab, tileMap.transform);
            tempChangeTile.name = changeTilePrefab.name;
            tempChangeTile.SetLocalScale(allTileObjs[i].transform.localScale);
            tempChangeTile.SetLocalPos(allTileObjs[i].transform.localPosition);

            allTileObjs.Swap(ref tempChangeTile, i);
            tempChangeTile.DestroyObj();
        }

        // 타일맵의 일부를 일정 확률로 다른 타일로 교체하는 로직}

        // {기존에 존재하는 타일의 순서를 조정하고, 컨트롤러를 캐싱하는 로직
        TerrainControler tempTerrain = default;
        TerrainType terrainType = TerrainType.NONE;
        int loopCnt = 0;
        foreach (GameObject tile_ in allTileObjs)
        {
            tempTerrain = tile_.GetComponent<TerrainControler>();
            switch (tempTerrain.name)
            {
                case RDefine.TERRAIN_PREF_PLAIN:
                    terrainType = TerrainType.PLAIN_PASS;
                    break;
                case RDefine.TERRAIN_PREF_OCEAN:
                    terrainType = TerrainType.OCEAN_N_PASS;
                    break;
                default:
                    terrainType = TerrainType.NONE;
                    break;

            }
            tempTerrain.SetupTerrain(mapController, terrainType, loopCnt);
            tempTerrain.transform.SetAsFirstSibling();
            allTerrains.Add(tempTerrain);
            loopCnt++;
        }
        // 기존에 존재하는 타일의 순서를 조정하고, 컨트롤러를 캐싱하는 로직}

    }
    //! 초기화된 타일의 정보로 연산한 맵의 가로, 세로 크기를 리턴한다.
    public Vector2Int GetCellSize() { return mapCellSize; }
    //! 초기화된 타일의 정보로 연산한 타일 사이의 갭을 리턴한다.
    public Vector2 GetCellGap() { return mapCellGap; }
    //! 인덱스에 해당하는 타일을 리턴한다
    public TerrainControler GetTile(int tileIdx1D)
    {
        // Debug.Log(allTerrains.Count);
        if (allTerrains.IsValid(tileIdx1D))
        {
            return allTerrains[tileIdx1D];
        }
        return default;
    }

}
