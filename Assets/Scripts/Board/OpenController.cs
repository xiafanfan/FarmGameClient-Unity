using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenController : MonoBehaviour
{
    [SerializeField]
    string grade;
    private void OnMouseDown()
    {
        Game.instance.OpenTreasureChest(grade,gameObject);
    }
}
