using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestController : MonoBehaviour
{
    public GameObject treasureChestBoard;

    private void OnMouseDown()
    {
       treasureChestBoard.GetComponent<BoardController>().Show();
    }
}
