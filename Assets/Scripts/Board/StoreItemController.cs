using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemController : ItemController
{
    public override void Add()
    {
        SetNum(num + 1);
        if (Game.instance.purchaseList.ContainsKey(ID))
        {
            Game.instance.purchaseList[ID]++;
        }
        else
        {
            Game.instance.purchaseList.Add(ID, 1);
        }
    }
    public override void Sub()
    {
        if (num > 0)
        {
            SetNum(num - 1);
            if (Game.instance.purchaseList.ContainsKey(ID))
            {
                Game.instance.purchaseList[ID]--;
                if (Game.instance.purchaseList[ID] == 0)
                {
                    Game.instance.purchaseList.Remove(ID);
                }
            }
        }
        //GameObject x= GameObject.Find("StoreBoard");
        //GameObject y = GameObject.Find("tomato_seed");
        //StoreItemController[] res= x.GetComponentsInChildren<StoreItemController>();
    }
}
