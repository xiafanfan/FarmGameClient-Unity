using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellController : MonoBehaviour
{

    private void OnMouseDown()
    {
        Game.instance.Sell(gameObject);
    }

}
