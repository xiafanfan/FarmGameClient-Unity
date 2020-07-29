using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpadeController : MonoBehaviour
{

    public Texture2D spade;

    //    Cursor.SetCursor(tx, Vector2.zero, CursorMode.Auto);
    private void OnMouseDown()
    {
        Cursor.SetCursor(spade, new Vector2(25,60), CursorMode.Auto);
        Debug.Log("sync clicked");
    }
}
