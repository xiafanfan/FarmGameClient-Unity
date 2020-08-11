using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBundle : Item
{
    public string itemContains;
    public int usesContains;
    public ItemBundle(string _ID, bool _isStackable, int _price, string _displayName,List<string> itemsContains ) : base(_ID, _isStackable, _price, _displayName)
    {
        itemContains = itemsContains[0];
        usesContains = itemsContains.Count;
    }
}
