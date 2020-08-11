using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddController : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    private void OnMouseDown()
    {
        InvokeRepeating("Add", 0, 0.1f);
    }
    private void OnMouseExit()
    {
        CancelInvoke("Add");
    }
    private void OnMouseUp()
    {
        CancelInvoke("Add");
    }
    private void Add()
    {
        target.GetComponent<ItemController>().Add();
    }
}
