using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseController : MonoBehaviour
{
    public GameObject parentBoard;
 
    private void OnMouseDown()
    {
        parentBoard.GetComponent<BoardController>().CloseWindow();
    }
}
