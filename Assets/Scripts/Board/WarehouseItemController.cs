using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseItemController : ItemController
{
    public override void Add()
    {
        if (num < Game.instance.ItemIcons[ID].GetComponent<ItemIconWithUsesController>().itemInstance.uses)
        {
            SetNum(num + 1);
            if (Game.instance.sellList.ContainsKey(ID))
            {
                Game.instance.sellList[ID]++;
            }
            else
            {
                Game.instance.sellList.Add(ID, 1);
            }
        }
    }
    public override void Sub()
    {
        if (num > 0)
        {
            SetNum(num - 1);
            if (Game.instance.sellList.ContainsKey(ID))
            {
                Game.instance.sellList[ID]--;
            }
            if (Game.instance.sellList[ID] == 0)
            {
                Game.instance.sellList.Remove(ID);
            }
        }

    }
}

