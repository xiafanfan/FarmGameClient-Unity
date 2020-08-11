using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseController : MonoBehaviour
{
    public GameObject warehouseBoard;
    public GameObject buttonSell;

    private void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = Color.gray;
        GetComponent<BoxCollider2D>().enabled = false;
        Game.instance.Sync(new Game.NextDo(ShowBoard));
    }

    private void ShowBoard()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<BoxCollider2D>().enabled = true;
        buttonSell.GetComponent<SpriteRenderer>().color = Color.gray;
        buttonSell.GetComponent<BoxCollider2D>().enabled = false;
        warehouseBoard.GetComponent<BoardController>().Show();
        Game.instance.GetInventory(new Game.NextDo(EnableButton));
    }

    private void EnableButton()
    {
        buttonSell.GetComponent<SpriteRenderer>().color = Color.white;
        buttonSell.GetComponent<BoxCollider2D>().enabled = true;
    }
}
