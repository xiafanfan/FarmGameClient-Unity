using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitController : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponent<Image>().color = Color.gray;
        GetComponent<BoxCollider2D>().enabled = false;
        Game.instance.Sync(new Game.NextDo(Exit));
    }
    private void Exit()
    {
        Application.Quit();
    }
}
