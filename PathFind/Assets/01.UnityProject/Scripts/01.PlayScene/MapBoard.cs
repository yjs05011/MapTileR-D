using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoard : MonoBehaviour
{
    // Start is called before the first frame update
    private const string TERRAIN_MAP_OBJ_NAME = "TerrainMap";
    private const string OBSTACLE_MAP_OBJ_NAME = "ObstacleMap";

    public Vector2Int MapCellSize { get; private set; } = default;
    public Vector2 MapCellGap { get; private set; } = default;
    private TerrainMap terrainMap = default;
    private ObstacleMap obstacleMap = default;

    private void Awake()
    {

        // { 각종 매니저를 모두 초기화 한다.}
        ResManager.Instance.Create();
        PathFinder.Instance.Create();
        //  각종 매니저를 모두 초기화 한다.}
        // PathFinder에 맵 보드 컨트롤러를 캐싱한다.
        PathFinder.Instance.mapBoard = this;

        // 맵의 지형을 초기화 하여 배치한다.
        terrainMap = gameObject.FindChildComponent<TerrainMap>(TERRAIN_MAP_OBJ_NAME);
        terrainMap.InitAwake(this);
        MapCellSize = terrainMap.GetCellSize();
        MapCellGap = terrainMap.GetCellGap();
        // 맵에 지물을 초기화하여 배치한다.
        obstacleMap = gameObject.FindChildComponent<ObstacleMap>(OBSTACLE_MAP_OBJ_NAME);
        obstacleMap.InitAwake(this);

    }

    //! 타일 인덱스를 받아서 해당 타일을 리턴하는 함수
    public TerrainControler GetTerrain(int idx1D)
    {
        return terrainMap.GetTile(idx1D);
    }
    //! 맵의 x 좌표를 받아서 해당 열의 타일을 리스트로 가져오는 함수
    public List<TerrainControler> GetTerrains_Colum(int xidx2D)
    {
        return GetTerrains_Colum(xidx2D, false);
    }
    //! 맵의 x 좌표를 받아서 해당 열의 타일을 리스트로 가져오는 함수
    public List<TerrainControler> GetTerrains_Colum(int xidx2D, bool isSortReverse)
    {
        List<TerrainControler> terrains = new List<TerrainControler>();
        TerrainControler tempTile = default;
        int tileIdx1D = 0;

        for (int y = 0; y < MapCellSize.y; y++)
        {
            tileIdx1D = y * MapCellSize.x + xidx2D;

            tempTile = terrainMap.GetTile(tileIdx1D);
            terrains.Add(tempTile);
        }
        if (terrains.IsValid())
        {
            if (isSortReverse)
            {
                terrains.Reverse();
            }
            else
            {

            }
            return terrains;
        }
        else
        {
            return default;
        }
    }

    //! 지형의 인덱스를 2D 좌표로 리턴하는 함수
    public Vector2Int GetTileIdx2D(int idx1D)
    {
        Vector2Int tileIdx2D = Vector2Int.zero;
        tileIdx2D.x = idx1D % MapCellSize.x;
        tileIdx2D.y = idx1D / MapCellSize.x;
        return tileIdx2D;
    }
    //! 지형의 2D 좌표를 인덱스로 리턴하는 함수
    public int GetTileIdx1D(Vector2Int idx2D)
    {
        int tileIdx1D = (idx2D.y * MapCellSize.x) + idx2D.x;
        return tileIdx1D;
    }
    //! 두 지형 사이의 타일 거리를 리턴하는 함수
    public Vector2Int GetDistance2D(GameObject targetTerrainObj, GameObject destTerrainObj)
    {
        Vector2 localDistance = destTerrainObj.transform.localPosition - targetTerrainObj.transform.localPosition;
        Vector2Int distance2D = Vector2Int.zero;
        distance2D.x = Mathf.RoundToInt(localDistance.x / MapCellGap.x);
        distance2D.y = Mathf.RoundToInt(localDistance.y / MapCellGap.y);
        distance2D = GFunc.Abs(distance2D);
        return distance2D;

    }
    //! 2D 좌표를 기준으로 주변 4방향 타읠의 인덱스를 리턴하는 함수
    public List<int> GetTileIdx2D_Around4ways(Vector2Int targetIdx2D)
    {
        List<int> idx1D_around4ways = new List<int>();
        List<Vector2Int> idx2D_around4ways = new List<Vector2Int>();
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x - 1, targetIdx2D.y));
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x + 1, targetIdx2D.y));
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x, targetIdx2D.y - 1));
        idx2D_around4ways.Add(new Vector2Int(targetIdx2D.x, targetIdx2D.y + 1));

        foreach (var idx2D in idx2D_around4ways)
        {
            if (idx2D.x.IsInRange(0, MapCellSize.x) == false)
            {
                continue;
            }
            if (idx2D.y.IsInRange(0, MapCellSize.y) == false)
            {
                continue;
            }
            idx1D_around4ways.Add(GetTileIdx1D(idx2D));

        }
        return idx1D_around4ways;


    }
}

