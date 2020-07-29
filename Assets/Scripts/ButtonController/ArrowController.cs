using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    public Texture2D arrow;
    //    Cursor.SetCursor(tx, Vector2.zero, CursorMode.Auto);
    private void OnMouseDown()
    {

        Cursor.SetCursor(arrow, new Vector2(30,0), CursorMode.Auto);
      
        Debug.Log("sync clicked");
    }
}
