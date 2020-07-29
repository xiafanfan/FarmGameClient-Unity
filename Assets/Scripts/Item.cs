using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;

public class Item 
{
    public string displayName;
    public bool isStackable;
    public uint price;
    public string ID;

    public Item(string _ID, bool _isStackable, uint _price, string _displayName)
    {
        displayName = _displayName;
        ID = _ID;
        isStackable = _isStackable;
        price = _price;
    }
}
