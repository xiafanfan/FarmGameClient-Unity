using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instance
{
    public string ID;
    public string instanceID;
    public int uses;

    public Instance(string _ID, string _instanceID, int _uses)
    {
        ID = _ID;
        instanceID = _instanceID;
        uses = _uses;
    }
}
