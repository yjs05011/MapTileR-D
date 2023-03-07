using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : GSingleton<PathFinder>
{
    #region 지형 탐색을 위한 변수
    public GameObject sourceObj = default;
    public GameObject destinationObj = default;
    public MapBoard mapBoard = default;

    #endregion
    #region A star 알고리즘으로 최단거리를 찾기 위한 변수
    private List<AstarNode> aStarResultPath = default;
    private List<AstarNode> aStarOpenPath = default;
    private List<AstarNode> aStarClosePath = default;
    #endregion
    //! 출발지와 목적지 정보를 길을 찾는 함수
    public void FindPath_Astar()
    {
        StartCoroutine(DelayFindPath_Astar(1.0f));
    }
    //! 탐색 알고리즘에 딜레이를 건다.
    private IEnumerator DelayFindPath_Astar(float delay_)
    {
        // A star 알고리즘을 사용하기 위해서 패스 리스트를 초기화한다.
        aStarOpenPath = new List<AstarNode>();
        aStarClosePath = new List<AstarNode>();
        aStarResultPath = new List<AstarNode>();
        TerrainControler targetTerrain = default;
        // 출발지의 인덱스를 구해서, 출발지 노드를 찾아온다.
        string[] sourceObjNameParts = sourceObj.name.Split('_');
        int sourceIdx1D = -1;
        int.TryParse(sourceObjNameParts[sourceObjNameParts.Length - 1], out sourceIdx1D);
        targetTerrain = mapBoard.GetTerrain(sourceIdx1D);
        // 찾아온 출발지 노드를 Open 리스트에 추가한다.
        AstarNode targetNode = new AstarNode(targetTerrain, destinationObj);
        Add_AstarOpenList(targetNode);

        int loopIdx = 0;
        bool isFoundDestination = false;
        bool isNoWayToGo = false;
        // while (loopIdx < 10)
        while (isFoundDestination == false && isNoWayToGo == false)
        {
            //{Open 리스트를 순회해서 가장 코스트가 낮은 노드를 선택한다.}
            AstarNode minCostNode = default;
            foreach (var terrainNode in aStarOpenPath)
            {
                if (minCostNode == default)
                {
                    minCostNode = terrainNode;
                }   // 가장 작은 코스트의 노드가 비어있는경우
                else
                {
                    // terrainNode가 더 작은 코스트를 가지는경우 minCostNode를 업데이트한다.
                    if (terrainNode.AstarF < minCostNode.AstarF)
                    {
                        minCostNode = terrainNode;
                    }
                    else
                    {
                        continue;
                    }
                } // 가장 작은 코스트의 노드가 캐싱 되어있는경우
            }
            //Open 리스트를 순회해서 가장 코스트가 낮은 노드를 선택한다.}
            minCostNode.ShowCost_Astar();
            minCostNode.Terrain.SetTileActiveColor(RDefine.TileStatusColor.SEARCH);
            // 선택한 노드가 목적지에 도달했는지 확인한다.
            bool isArriveDest = mapBoard.GetDistance2D(minCostNode.Terrain.gameObject, destinationObj).Equals(Vector2Int.zero);

            if (isArriveDest)
            {
                //{ 목적지에 도착했다면 aStarResultPath 리스트를 설정한다.
                AstarNode resulNode = minCostNode;
                bool isSet_aStarResultPathOk = false;
                while (!isSet_aStarResultPathOk)
                {
                    aStarResultPath.Add(resulNode);
                    if (resulNode.AstarPrevNode == default || resulNode.AstarPrevNode == null)
                    {
                        isSet_aStarResultPathOk = true;
                        break;
                    }
                    else { }
                    resulNode = resulNode.AstarPrevNode;
                }

                //목적지에 도착했다면 aStarResultPath 리스트를 설정한다.}
                //OpenList 와 Close List를 정리한다.
                aStarOpenPath.Clear();
                aStarClosePath.Clear();
                isFoundDestination = true;
                break;

            }//선택한 노드가 목적지에 도착한경우
            else
            {
                //{도착하지 않았다면 현재 타일을 기준으로 4방향 노드를 찾아온다.
                List<int> nextSearchIx1Ds = mapBoard.GetTileIdx2D_Around4ways(minCostNode.Terrain.TileIdx2D);
                //찾아온 노드 중에서 이동 가능한 노드는 Open list에 추가한다
                AstarNode nextNode = default;
                foreach (var nextIdx1D in nextSearchIx1Ds)
                {
                    nextNode = new AstarNode(mapBoard.GetTerrain(nextIdx1D), destinationObj);
                    Debug.Log($"name : {nextNode.Terrain.name}, isPassable {nextNode.Terrain.IsPassable}");

                    if (nextNode.Terrain.IsPassable == false) { continue; }
                    Add_AstarOpenList(nextNode, minCostNode);

                }
                // 탐색이 끝난 노드는 CloseList에 추가하고, OpenList에서 제거한다.
                // 이 때, OpenList가 비어 있다면 더 이상 탐색할 수 있는 길이 존재하지 않는 것이다.
                aStarOpenPath.Add(minCostNode);
                aStarClosePath.Remove(minCostNode);
                if (aStarOpenPath.IsValid() == false)
                {
                    GFunc.LogWarning("there are no more tiles to Explore.");
                    isNoWayToGo = true;
                }
                foreach (var tempNode in aStarOpenPath)
                {
                    GFunc.Log($"Idx: {tempNode.Terrain.TileIdx1D}" + $"Cost: {tempNode.AstarF}");
                }


                //도착하지 않았다면 현재 타일을 기준으로 4방향 노드를 찾아온다.}
                loopIdx++;
                yield return new WaitForSeconds(delay_);
            } //선택한 노드가 목적지에 도착하지 못한경우
        }

    }
    //! 비용을 설정한 노드를 Open 리스트에 추가한다.
    private void Add_AstarOpenList(AstarNode targetTerrain_, AstarNode prevNode = default)
    {
        Update_AstarCostToTerrain(targetTerrain_, prevNode);

        AstarNode closeNode = aStarClosePath.FindNode(targetTerrain_);
        if (closeNode != default && closeNode != null)
        {

            //이미 탐색이 끝난 좌표의 노드가 존재하는 경우에는 OpenList에 추가 하지않는다.
        }
        else
        {
            AstarNode openedNode = aStarOpenPath.FindNode(targetTerrain_);
            if (openedNode != default && openedNode != null)
            {
                // 타겟 노드의 코스트가 더 작은 경우에는 Open list 에서 노드를 교체한다.
                // 타겟 노드의 코스트가 더 큰 경우에는 Open list에 추가하지 않는다.
                if (targetTerrain_.AstarF < openedNode.AstarF)
                {


                    aStarOpenPath.Remove(openedNode);
                    aStarOpenPath.Add(targetTerrain_);

                }
                else
                {
                    Debug.Log("!!!");
                }


            }
            else
            {
                aStarOpenPath.Add(targetTerrain_);
            }
        }

        // AstarNode closeNode = aStarClosePath
    }

    //! target 지형 정보와 Destination 지형 정보로 Distance와 Heuristic을 설정하는 함수
    private void Update_AstarCostToTerrain(AstarNode targetNode, AstarNode prevNode)
    {
        Vector2Int distance2D = mapBoard.GetDistance2D(targetNode.Terrain.gameObject, destinationObj);
        int totalDistance2D = distance2D.x + distance2D.y;

        //heuristic은 직선거리로 고정한다
        Vector2 localDistance = destinationObj.transform.localPosition - targetNode.Terrain.transform.localPosition;
        float heuristic = Mathf.Abs(localDistance.magnitude);
        //{이전 노드가 존재하는 경우 이전 노드의 코스트를 추가해서 연산한다}
        if (prevNode == default || prevNode == null) { }
        else
        {
            totalDistance2D = Mathf.RoundToInt(prevNode.AstarG + 1.0f);

        }
        targetNode.UpdateCost_Astar(totalDistance2D, heuristic, prevNode);
        //이전 노드가 존재하는 경우 이전 노드의 코스트를 추가해서 연산한다}

    }
}
