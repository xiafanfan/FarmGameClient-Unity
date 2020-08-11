using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubController : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    private void OnMouseDown()
    {
        
        InvokeRepeating("Sub", 0, 0.1f);
    }
    private void OnMouseExit()
    {
        CancelInvoke("Sub");
    }
    private void OnMouseUp()
    {
        CancelInvoke("Sub");
    }
    private void Sub()
    {
        target.GetComponent<ItemController>().Sub();
    }
}
