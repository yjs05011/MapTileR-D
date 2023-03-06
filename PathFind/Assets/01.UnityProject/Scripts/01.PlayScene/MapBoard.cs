using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoard : MonoBehaviour
{
    // Start is called before the first frame update
    private const string TERRAIN_MAP_OBJ_NAME = "TerrainMap";

    public Vector2Int MapCellSize { get; private set; } = default;
    public Vector2 MapCellGap { get; private set; } = default;
    private TerrainMap terrainMap = default;


    private void Awake()
    {

        // { 각종 매니저를 모두 초기화 한다.}
        ResManager.Instance.Create();
        //  각종 매니저를 모두 초기화 한다.}

        // 맵의 지형을 초기화 하여 배치한다.
        Debug.Log(TERRAIN_MAP_OBJ_NAME);
        terrainMap = gameObject.FindChildComponent<TerrainMap>(TERRAIN_MAP_OBJ_NAME);

        terrainMap.InitAwake(this);
        MapCellSize = terrainMap.GetCellSize();
        MapCellGap = terrainMap.GetCellGap();

    }
}
