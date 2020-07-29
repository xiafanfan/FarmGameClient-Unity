using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;


public class SingleDemo : MonoBehaviour
{
    private string playFabID;
    public Dictionary<string, Item> catalogItems;
    public Dictionary<string, Seed> seeds;
    public Dictionary<string, Fertilizer> fertilizers;
    public Dictionary<string, TreasureChest> treasureChests;

    public GameObject soilPrefab;
    private GameObject[] soils = new GameObject[15];
    public GameObject tomatoIcon;
    public GameObject eggplantIcon;
    public GameObject sunflowerIcon;
    public GameObject strawberryIcon;
    public GameObject commonFertilizer;
    public GameObject uncommonFertilizer;
    public GameObject tomatoSeedIcon;
    public GameObject eggplantSeedIcon;
    public GameObject sunflowerSeedIcon;
    public GameObject strawberrySeedIcon;
    public GameObject GDIcon;
    private Dictionary<string, GameObject> ItemIcons = new Dictionary<string, GameObject>();
    public int[] growthStageTime = new int[] { 30, 80, 150 };
    public double[] growthStageRatio = new double[] { 0.2, 0.4, 0.8 };
    public static SingleDemo instance;
    private int x = 0;
    private SingleDemo()
    {

    }
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            ItemIcons.Add("GD", GDIcon);
            ItemIcons.Add("tomato", tomatoIcon);
            ItemIcons.Add("eggplant", eggplantIcon);
            ItemIcons.Add("sunflower", sunflowerIcon);
            ItemIcons.Add("strawberry", strawberryIcon);
            ItemIcons.Add("common_fertilizer", commonFertilizer);
            ItemIcons.Add("uncommon_fertilizer", uncommonFertilizer);
            ItemIcons.Add("tomato_seed", tomatoSeedIcon);
            ItemIcons.Add("eggplant_seed", eggplantSeedIcon);
            ItemIcons.Add("sunflower_seed", sunflowerSeedIcon);
            ItemIcons.Add("strawberry_seed", strawberrySeedIcon);
            InitSoils();
            Login();

        }
    }
    public int CC()
    {
        x++;
        foreach (string key in ItemIcons.Keys)
        {
            ItemIcons[key].GetComponent<ItemIconWithUsesController>().ModifyUses(1);
        }
        return x;
    }
    void InitSoils()
    {
        for (int i = 0; i < 15; i++)
        {
            soils[i] = Instantiate(soilPrefab, new Vector2((-3.2f - 2.3f * (i % 3) + 2.65f * (i / 3)), 0.93f * (2 - i % 3) - 1.05f * (i / 3)), Quaternion.identity);
        }

    }

    void Login()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = "FAN",
            CreateAccount = false
        }, result =>
        {
            playFabID = result.PlayFabId;
            Debug.Log("Successfully logged in a player with PlayFabId: " + result.PlayFabId);
            GetCatalogITem();

        }, error => Debug.LogError(error.GenerateErrorReport()));
    }


    void GetCatalogITem()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
        {
            CatalogVersion = "main"
        }, result =>
        {
            catalogItems = new Dictionary<string, Item>(result.Catalog.Count);
            seeds = new Dictionary<string, Seed>();
            fertilizers = new Dictionary<string, Fertilizer>();
            treasureChests = new Dictionary<string, TreasureChest>();
            foreach (CatalogItem catalogItem in result.Catalog)
            {
                if (catalogItem.ItemClass == "seed")
                {

                    JObject obj = JObject.Parse(catalogItem.CustomData);
                    string p = obj["product"] == null ? null : obj["product"].ToString();
                    int g = obj["grow_time"] == null ? 0 : Convert.ToInt32(obj["grow_time"]);
                    Seed item = new Seed(catalogItem.ItemId, catalogItem.IsStackable, catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName, g, p);
                    seeds.Add(catalogItem.ItemId, item);
                    catalogItems.Add(catalogItem.ItemId, item);
                    continue;
                }
                if (catalogItem.ItemClass == "fertilizer")
                {
                    JObject obj = JObject.Parse(catalogItem.CustomData);
                    int a = obj["acceleration"] == null ? 0 : Convert.ToInt32(obj["acceleration"]);
                    Fertilizer item = new Fertilizer(catalogItem.ItemId, catalogItem.IsStackable, catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName, a);
                    fertilizers.Add(catalogItem.ItemId, item);
                    catalogItems.Add(catalogItem.ItemId, item);
                    continue;
                }
                if (catalogItem.ItemClass == "props")
                {
                    if (catalogItem.Container != null)
                    {
                        TreasureChest item = new TreasureChest(catalogItem.ItemId, catalogItem.IsStackable, catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName, catalogItem.Container.KeyItemId);
                        treasureChests.Add(catalogItem.ItemId, item);
                        catalogItems.Add(catalogItem.ItemId, item);
                        continue;
                    }
                    else
                    {
                        catalogItems.Add(catalogItem.ItemId, new Item(catalogItem.ItemId, catalogItem.IsStackable, catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName));
                    }
                }
                if (catalogItem.ItemClass == "product")
                {
                    catalogItems.Add(catalogItem.ItemId, new Item(catalogItem.ItemId, catalogItem.IsStackable, catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName));
                }
            }
            GetInventory();
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void GetInventory()
    {
        int index = 0;
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest { }, result =>
        {
            ItemIconWithUsesController GDcontroller = GDIcon.GetComponent<ItemIconWithUsesController>();
            GDcontroller.itemInstance = new Instance("GD", "", result.VirtualCurrency["GD"]);
            GDcontroller.RefreshUsesText();
            foreach (ItemInstance instance in result.Inventory)
            {
                switch (instance.ItemClass)
                {
                    case "soil":
                        soils[index].GetComponent<Animator>().SetTrigger("Active Soil");
                        soils[index].GetComponent<SoilController>().instanceID = instance.ItemInstanceId;
                        if (instance.CustomData.ContainsKey("species") && instance.CustomData.ContainsKey("plantTime"))
                        {
                            SoilController soilController = soils[index].GetComponent<SoilController>();
                            soilController.Species = instance.CustomData["species"];
                            soilController.PlantTime = Convert.ToInt64(instance.CustomData["plantTime"]);
                            soilController.Acceleration = instance.CustomData.ContainsKey("acceleration") ? Convert.ToInt32(instance.CustomData["acceleration"]) : 0;
                            soilController.UpdateGrowthStage();
                        }
                        index++;
                        break;
                    default:
                        if (ItemIcons.ContainsKey(instance.ItemId))
                        {
                            ItemIconWithUsesController controller = ItemIcons[instance.ItemId].GetComponent<ItemIconWithUsesController>();
                            controller.itemInstance = new Instance(instance.ItemId, instance.ItemInstanceId, (int)instance.RemainingUses);
                            controller.RefreshUsesText();

                        }
                        break;
                }
            }

        }, error => Debug.LogError(error.GenerateErrorReport()));
    }


    public static T ParseFromJson<T>(string szJson)
    {
        if (typeof(T) == typeof(IEnumerable<>)) { }
        T obj = Activator.CreateInstance<T>();
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(szJson)))
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType()); return (T)serializer.ReadObject(ms);
        }
    }
}
