using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public GameObject BGMask;
    private int x;
    public void Show()
    {
        Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        Game.instance.operationType = Game.OperationType.None;
        Game.instance.operatedItem = null;
        BGMask.SetActive(true);
        gameObject.SetActive(true);
    }
    public void CloseWindow()
    {
        BGMask.SetActive(false);
        gameObject.SetActive(false);
    }
    public void ResetWindow()
    {
        foreach (ItemController itemController in GetComponentsInChildren<ItemController>())
        {
            itemController.SetNum(0);
        }
    }
}
