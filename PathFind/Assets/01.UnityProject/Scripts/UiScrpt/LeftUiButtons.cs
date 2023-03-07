using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftUiButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClickAstarFindBtn()
    {
        Debug.Log("Check");
        PathFinder.Instance.FindPath_Astar();
    }
}
