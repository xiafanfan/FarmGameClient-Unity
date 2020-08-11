using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest:Item
{
    public string keyID;
    public TreasureChest(string _ID, bool _isStackable, int _price, string _displayName, string _keyID) : base(_ID, _isStackable, _price, _displayName)
    {
        keyID = _keyID;
    }

}
