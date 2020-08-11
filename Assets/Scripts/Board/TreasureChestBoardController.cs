using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestBoardController : BoardController
{
    public GameObject gold_treasure_chest;
    public GameObject silver_treasure_chest;
    public GameObject bronze_treasure_chest;
    public GameObject gold_key;
    public GameObject silver_key;
    public GameObject bronze_key;
    public Dictionary<string, ItemIconWithUsesController> propControllers;

    public void Init()
    {
        propControllers = new Dictionary<string, ItemIconWithUsesController>();
        propControllers["gold_treasure_chest"] = gold_treasure_chest.GetComponent<ItemIconWithUsesController>();
        propControllers["silver_treasure_chest"] = silver_treasure_chest.GetComponent<ItemIconWithUsesController>();
        propControllers["bronze_treasure_chest"] = bronze_treasure_chest.GetComponent<ItemIconWithUsesController>();
        propControllers["gold_key"] = gold_key.GetComponent<ItemIconWithUsesController>();
        propControllers["silver_key"] = silver_key.GetComponent<ItemIconWithUsesController>();
        propControllers["bronze_key"] = bronze_key.GetComponent<ItemIconWithUsesController>();
    }

}
