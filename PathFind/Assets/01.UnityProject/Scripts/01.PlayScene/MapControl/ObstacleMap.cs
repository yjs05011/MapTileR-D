using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMap : TileMapController
{
    private const string OBSTACLE_TILEMAP_OBJ_NAME = "ObstacleTileMap";
    private GameObject[] castleObjs = default;

    //! Awake 타임에 초기화 할  내용을 재정의 한다.
    public override void InitAwake(MapBoard mapController_)
    {
        this.tileMapObjName = OBSTACLE_TILEMAP_OBJ_NAME;
        base.InitAwake(mapController_);


    }
    private void Start()
    {
        StartCoroutine(DelayStart(0f));
    }
    IEnumerator DelayStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoStart();
    }
    private void DoStart()
    {
        //! {출발지와 목적지를 설정해서 타일을 배치한다
        castleObjs = new GameObject[2];
        TerrainControler[] passableTerrains = new TerrainControler[2];
        List<TerrainControler> searchTerrains = default;
        int searchIdx = 0;
        TerrainControler foundTile = default;

        //출발지는 좌측에서 우측으로 y축을 서치해서 빈 지형을 받아온다.
        searchIdx = 0;
        foundTile = default;

        while (foundTile == null || foundTile == default)
        {

            searchTerrains = mapController.GetTerrains_Colum(searchIdx, true);
            // Debug.Log(searchTerrains);
            foreach (var searchTerrain in searchTerrains)
            {

                if (searchTerrain.IsPassable)
                {
                    foundTile = searchTerrain;
                    break;
                }
                else
                {

                }
            }
            if (foundTile != null || foundTile != default) { break; }
            if (mapController.MapCellSize.x - 1 <= searchIdx)
            {
                break;
            }
            searchIdx++;

        }

        passableTerrains[0] = foundTile;
        // 목적지는 우측에서 좌측으로 y축을 서치해서 빈 지형을 받아온다.
        searchIdx = mapController.MapCellSize.x - 1;
        foundTile = default;
        while (foundTile == null || foundTile == default)
        {
            searchTerrains = mapController.GetTerrains_Colum(searchIdx);
            foreach (var searchTerrain in searchTerrains)
            {
                if (searchTerrain.IsPassable)
                {
                    foundTile = searchTerrain;
                    break;
                }
                else
                {

                }
            }
            if (foundTile != null || foundTile != default)
            {
                break;
            }
            if (searchIdx <= 0) { break; }
            searchIdx--;
        }
        Debug.Log(foundTile);
        passableTerrains[1] = foundTile;
        //! 출발지와 목적지를 설정해서 타일을 배치한다}
        // 출발지와 목적지에 지물을 추가한다.
        GameObject changeTilePrefab = ResManager.Instance.obstaclePrefabs[RDefine.OBSTACLE_PREF_PLAIN_CASTLE];
        GameObject tempChangeTile = default;
        for (int i = 0; i < 2; i++)
        {
            tempChangeTile = Instantiate(changeTilePrefab, tileMap.transform);

            tempChangeTile.name = string.Format("{0}_{1}", changeTilePrefab.name, passableTerrains[i].TileIdx1D);
            tempChangeTile.SetLocalScale(passableTerrains[i].transform.localScale);
            tempChangeTile.SetLocalPos(passableTerrains[i].transform.localPosition);

            castleObjs[i] = tempChangeTile;
            Add_Obstacle(tempChangeTile);
            tempChangeTile = default;
        }
        Update_SourDestToPathFinder();

    }
    //! 지물을 추가한다.
    public void Add_Obstacle(GameObject obstacle_)
    {
        allTileObjs.Add(obstacle_);
    }
    //! 패스 파인더에 출발지와 목적지를 설정한다.
    public void Update_SourDestToPathFinder()
    {
        PathFinder.Instance.sourceObj = castleObjs[0];
        PathFinder.Instance.destinationObj = castleObjs[1];

    }
}
