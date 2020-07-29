using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fertilizer : Item
{
    public int acceleration=0;
    public Fertilizer(string _ID, bool _isStackable, uint _price, string _displayName, int _acceleration) : base(_ID, _isStackable, _price, _displayName)
    {
        acceleration = _acceleration;
    }

}
