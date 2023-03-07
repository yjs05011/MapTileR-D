using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarNode
{
    public TerrainControler Terrain { get; private set; }
    public GameObject DestinationObj { get; private set; }

    #region  Node 변수
    public float AstarF { get; private set; } = float.MaxValue;
    public float AstarG { get; private set; } = float.MaxValue;
    public float AstarH { get; private set; } = float.MaxValue;

    public AstarNode AstarPrevNode { get; private set; }
    #endregion


    public AstarNode(TerrainControler terrain_, GameObject destinationObj_)
    {
        Terrain = terrain_;
        DestinationObj = destinationObj_;
    }

    // ! AStar 알고리즘에 사용할 비용을 설정한다.
    public void UpdateCost_Astar(float gCost, float heuristic, AstarNode PrevNode)
    {
        float aStarF = gCost + heuristic;
        if (aStarF < AstarF)
        {
            AstarG = gCost;
            AstarH = heuristic;
            AstarF = aStarF;
            AstarPrevNode = PrevNode;
        }
        else
        {

        }
    }
    public void ShowCost_Astar()
    {
        GFunc.Log($"TileIdx1D: {Terrain.TileIdx1D}," + $"F: {AstarF}, G: {AstarG}, H: {AstarH}");
    }

}
