using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    //public Texture2D arrow;
    private void OnMouseDown()
    {
        //Cursor.SetCursor(arrow, new Vector2(30,0), CursorMode.Auto);
        Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        Game.instance.operationType = Game.OperationType.None;
        Game.instance.operatedItem = null;
    }
}
