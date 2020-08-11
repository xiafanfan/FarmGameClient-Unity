using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commodity
{
    public int price;
    public string ID;
    public Commodity(string _ID, int _price)
    {
        ID = _ID;
        price = _price;
    }
}
