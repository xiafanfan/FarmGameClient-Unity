using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyController : MonoBehaviour
{
    private void OnMouseDown()
    {
        Game.instance.Purchase(gameObject);
    }
}
