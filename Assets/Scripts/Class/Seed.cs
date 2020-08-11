using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Item
{
    public string product;
    public int growthTime;
    public Seed(string _ID, bool _isStackable, int _price, string _displayName, int _growthTime, string _product) : base(_ID, _isStackable, _price, _displayName)
    {
        product = _product;
        growthTime = _growthTime;
    }
}
