using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncButtonController : MonoBehaviour
{

    private void OnMouseDown()
    {
        Game.instance.Sync();
    }
}
