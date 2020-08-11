using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    public GameObject storeBoard;

    private void OnMouseDown()
    {
        storeBoard.GetComponent<BoardController>().Show();
    }
}
