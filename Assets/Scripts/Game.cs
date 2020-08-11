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
using Newtonsoft.Json;


public class Game : MonoBehaviour
{
    private string playFabID;
    public Dictionary<string, Commodity> commodities;
    public Dictionary<string, Item> catalogItems;
    public Dictionary<string, Seed> seeds;
    public Dictionary<string, Fertilizer> fertilizers;
    public Dictionary<string, TreasureChest> treasureChests;

    public GameObject resultPanel;
    public GameObject treasureChestBoard;
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
    public GameObject syncButton;
    public Dictionary<string, GameObject> ItemIcons = new Dictionary<string, GameObject>();
    public double[] growthStageRatio = new double[] { 0.2, 0.4, 0.8 };
    public static Game instance;
    private int x = 0;
    public enum OperationType { None, Sow, Harvest, Fertilizer };
    public OperationType operationType;
    public Instance operatedItem;
    public Dictionary<string, int> purchaseList = new Dictionary<string, int>();
    public Dictionary<string, int> sellList = new Dictionary<string, int>();
    public List<ExecuteCloudScriptRequest> syncReqQueue = new List<ExecuteCloudScriptRequest>();
    public Dictionary<string, UpdatedSoil> soilUpdates = new Dictionary<string, UpdatedSoil>();
    public Dictionary<string, ConsumedItem> itemConsumes = new Dictionary<string, ConsumedItem>();
    public List<string> itemGrants = new List<string>();
    private bool syncing = false;

    private Game()
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

    public void Login(string username, Login loginController)
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = username.ToUpper(),
            CreateAccount = true
        }, result =>
        {
            if (result.NewlyCreated == true)
            {
                ShowResult("Hello new friend, we have prepared a novice pacage for you! \nplease wait a moment");
            }
            playFabID = result.PlayFabId;
            loginController.OK();
            Invoke("Init", result.NewlyCreated ? 3 : 0);
        }, error => {
            loginController.Wrong(error.GenerateErrorReport());
            });
    }

    private void Init()
    {
        GetCatalogItem();
        GetStoreItem();
    }

    void GetStoreItem()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            StoreId = "storeA"
        }, result =>
        {
            commodities = new Dictionary<string, Commodity>();
            foreach (StoreItem item in result.Store)
            {
                commodities[item.ItemId] = new Commodity(item.ItemId, (int)item.VirtualCurrencyPrices["GD"]);
            }
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    void GetCatalogItem()
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
                    Seed item = new Seed(catalogItem.ItemId, catalogItem.IsStackable, (int)catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName, g, p);
                    seeds.Add(catalogItem.ItemId, item);
                    catalogItems.Add(catalogItem.ItemId, item);
                    continue;
                }
                if (catalogItem.ItemClass == "fertilizer")
                {
                    JObject obj = JObject.Parse(catalogItem.CustomData);
                    int a = obj["acceleration"] == null ? 0 : Convert.ToInt32(obj["acceleration"]);
                    Fertilizer item = new Fertilizer(catalogItem.ItemId, catalogItem.IsStackable, (int)catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName, a);
                    fertilizers.Add(catalogItem.ItemId, item);
                    catalogItems.Add(catalogItem.ItemId, item);
                    continue;
                }
                if (catalogItem.ItemClass == "props")
                {
                    if (catalogItem.Container != null)
                    {
                        TreasureChest item = new TreasureChest(catalogItem.ItemId, catalogItem.IsStackable, (int)catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName, catalogItem.Container.KeyItemId);
                        treasureChests.Add(catalogItem.ItemId, item);
                        catalogItems.Add(catalogItem.ItemId, item);
                        continue;
                    }
                    else
                    {
                        catalogItems.Add(catalogItem.ItemId, new Item(catalogItem.ItemId, catalogItem.IsStackable, (int)catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName));
                    }
                }
                if (catalogItem.ItemClass == "product")
                {
                    catalogItems.Add(catalogItem.ItemId, new Item(catalogItem.ItemId, catalogItem.IsStackable, (int)catalogItem.VirtualCurrencyPrices["GD"], catalogItem.DisplayName));
                }
                if (catalogItem.ItemClass == "product_bundle")
                {
                    catalogItems.Add(catalogItem.ItemId, new ItemBundle(catalogItem.ItemId, catalogItem.IsStackable, 0, catalogItem.DisplayName, catalogItem.Bundle.BundledItems));
                }
            }
            GetInventory();
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void GetInventory(NextDo nextDo=null)
    {
        int index = 0;
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest { }, result =>
        {
            treasureChestBoard.GetComponent<TreasureChestBoardController>().Init();
            ItemIconWithUsesController GDcontroller = GDIcon.GetComponent<ItemIconWithUsesController>();
            GDcontroller.itemInstance = new Instance("GD", "", result.VirtualCurrency["GD"]);
            GDcontroller.RefreshUsesText();
            foreach (ItemInstance instance in result.Inventory)
            {
                switch (instance.ItemClass)
                {
                    case "soil":
                        soils[index].GetComponent<SoilController>().ActiveSoil();
                        soils[index].GetComponent<SoilController>().instanceID = instance.ItemInstanceId;
                        if (instance.CustomData!=null&&instance.CustomData.ContainsKey("species") && instance.CustomData.ContainsKey("plantTime"))
                        {
                            SoilController soilController = soils[index].GetComponent<SoilController>();
                            soilController.Species = instance.CustomData["species"];
                            soilController.PlantTime = Convert.ToInt64(instance.CustomData["plantTime"]);
                            soilController.Acceleration = instance.CustomData.ContainsKey("acceleration") ? Convert.ToInt32(instance.CustomData["acceleration"]) : 0;
                            soilController.UpdateGrowthStage();
                        }
                        index++;
                        break;
                    case "props":
                        if (treasureChestBoard.GetComponent<TreasureChestBoardController>().propControllers.ContainsKey(instance.ItemId))
                        {
                            ItemIconWithUsesController controller = treasureChestBoard.GetComponent<TreasureChestBoardController>().propControllers[instance.ItemId];
                            controller.itemInstance = new Instance(instance.ItemId, instance.ItemInstanceId, (int)instance.RemainingUses);
                            controller.RefreshUsesText();
                        };
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
            if (nextDo != null)
            {
                nextDo();
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

    public void Sync(NextDo nextDo = null)
    {
        ExecuteCloudScriptRequest syncRequest;
        if (soilUpdates.Keys.Count + itemConsumes.Keys.Count + itemGrants.Count > 0)
        {
            syncRequest = new ExecuteCloudScriptRequest
            {
                FunctionName = "syncData",
                RevisionSelection = CloudScriptRevisionOption.Live,
                FunctionParameter = new
                {
                    soilUpdates = soilUpdates.Keys.Count > 0 ? soilUpdates : null,
                    itemGrants = itemGrants.Count > 0 ? itemGrants : null,
                    itemConsumes = itemConsumes.Keys.Count > 0 ? itemConsumes : null
                },
                GeneratePlayStreamEvent = true
            };
            soilUpdates = new Dictionary<string, UpdatedSoil>();
            itemConsumes = new Dictionary<string, ConsumedItem>();
            itemGrants = new List<string>();
            syncReqQueue.Add(syncRequest);
        }
        syncing = true;
        syncButton.GetComponent<SpriteRenderer>().color = Color.gray;
        syncButton.GetComponent<BoxCollider2D>().enabled = false;
        PostSyncReq(nextDo);
    }

    public void PostSyncReq(NextDo nextDo = null)
    {
        if (syncReqQueue.Count > 0)
        {
            ExecuteCloudScriptRequest syncRequest = syncReqQueue[0];
            PlayFabClientAPI.ExecuteCloudScript(syncRequest, result =>
            {
                if (result.Error != null)
                {
                    syncing = false;
                    syncButton.GetComponent<SpriteRenderer>().color = Color.white;
                    syncButton.GetComponent<BoxCollider2D>().enabled = true;
                    Debug.Log(result.Error.StackTrace);
                }
                else
                {
                    syncReqQueue.RemoveAt(0);
                    PostSyncReq(nextDo);
                }
            }, error =>
            {
                syncing = false;
                syncButton.GetComponent<SpriteRenderer>().color = Color.white;
                syncButton.GetComponent<BoxCollider2D>().enabled = true;
                Debug.LogError(error.GenerateErrorReport());
            });
        }
        else
        {
            syncing = false;
            syncButton.GetComponent<SpriteRenderer>().color = Color.white;
            syncButton.GetComponent<BoxCollider2D>().enabled = true;
            if (nextDo != null)
            {
                nextDo();
            }
        }
    }

    public void Purchase(GameObject buttonBuy)
    {
        if (purchaseList.Keys.Count == 0)
        {
            return;
        }
        buttonBuy.GetComponent<SpriteRenderer>().color = Color.gray;
        buttonBuy.GetComponent<BoxCollider2D>().enabled = false;
        int price = 0;
        foreach (string key in purchaseList.Keys)
        {
            price += commodities[key].price * purchaseList[key];
        }
        if (price <= GDIcon.GetComponent<ItemIconWithUsesController>().GetUses())
        {
            ExecuteCloudScriptRequest buyReq = new ExecuteCloudScriptRequest
            {
                FunctionName = "buy2",
                RevisionSelection = CloudScriptRevisionOption.Live,
                FunctionParameter = new
                {
                    itemBuys = purchaseList
                },
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(buyReq, result =>
            {
                if (result.Error == null)
                {
                    PlayFab.Json.JsonObject functionResult = (PlayFab.Json.JsonObject)result.FunctionResult;
                    PlayFab.Json.JsonObject grantResult = (PlayFab.Json.JsonObject)functionResult["Result"];
                    if (functionResult["Result"].GetType() == "".GetType())
                    {
                        return;
                    }
                    int _price = Convert.ToInt32(grantResult["Price"]);
                    JArray grantedItems = (JArray)JsonConvert.DeserializeObject(grantResult["ItemGrantResults"].ToString());
                    for (int i = 0; i < grantedItems.Count; i++)
                    {
                        JToken gItem = grantedItems[i];
                        if (gItem["ItemClass"]!=null&&( gItem["ItemClass"].ToString() == "fertilizer" || gItem["ItemClass"].ToString() == "seed"))
                        {
                            ItemIconWithUsesController controller = ItemIcons[gItem["ItemId"].ToString()].GetComponent<ItemIconWithUsesController>();
                            controller.ModifyUses((int)gItem["UsesIncrementedBy"]);
                            controller.itemInstance.instanceID = gItem["ItemInstanceId"].ToString();
                        }
                    }
                    purchaseList.Clear();
                    buttonBuy.GetComponentInParent<BoardController>().ResetWindow();
                    GDIcon.GetComponent<ItemIconWithUsesController>().ModifyUses(-_price);
                    buttonBuy.GetComponent<SpriteRenderer>().color = Color.white;
                    buttonBuy.GetComponent<BoxCollider2D>().enabled = true;
                }
                else
                {
                    Debug.Log(result.Error.StackTrace);
                    buttonBuy.GetComponent<SpriteRenderer>().color = Color.white;
                    buttonBuy.GetComponent<BoxCollider2D>().enabled = true;
                }
            }, error =>
            {
                Debug.LogError(error.GenerateErrorReport());
                buttonBuy.GetComponent<SpriteRenderer>().color = Color.white;
                buttonBuy.GetComponent<BoxCollider2D>().enabled = true;
            });
        }
        else
        {
            Debug.Log("No Enough Money");
            buttonBuy.GetComponent<SpriteRenderer>().color = Color.white;
            buttonBuy.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void Sell(GameObject buttonSell)
    {
        if (sellList.Keys.Count == 0)
        {
            return;
        }
        buttonSell.GetComponent<SpriteRenderer>().color = Color.gray;
        buttonSell.GetComponent<BoxCollider2D>().enabled = false;
        int income = 0;
        List<SoldItem> soldItems = new List<SoldItem>();
        foreach (string key in sellList.Keys)
        {
            soldItems.Add(new SoldItem(key, ItemIcons[key].GetComponent<ItemIconWithUsesController>().itemInstance.instanceID, sellList[key]));
            income += catalogItems[key].price * sellList[key];
        }
        ExecuteCloudScriptRequest sellReq = new ExecuteCloudScriptRequest
        {
            FunctionName = "sell",
            RevisionSelection = CloudScriptRevisionOption.Live,
            FunctionParameter = new
            {
                needRefresh = true,
                itemSells = soldItems,
            },
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(sellReq, result =>
        {
            if (result.Error == null)
            {
                foreach (string key in sellList.Keys)
                {
                    ItemIcons[key].GetComponent<ItemIconWithUsesController>().ModifyUses(-sellList[key]);
                }
                sellList.Clear();
                buttonSell.GetComponentInParent<BoardController>().ResetWindow();
                GDIcon.GetComponent<ItemIconWithUsesController>().ModifyUses(income);
                buttonSell.GetComponent<SpriteRenderer>().color = Color.white;
                buttonSell.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                Debug.Log(result.Error.StackTrace);
                buttonSell.GetComponent<SpriteRenderer>().color = Color.white;
                buttonSell.GetComponent<BoxCollider2D>().enabled = true;
            }
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
            buttonSell.GetComponent<SpriteRenderer>().color = Color.white;
            buttonSell.GetComponent<BoxCollider2D>().enabled = true;
        });
    }

    public void OpenTreasureChest(string grade,GameObject buttonOpen)
    {
        string treasureChestId = grade + "_treasure_chest";
        string keyId = grade + "_key";
        Dictionary<string, ItemIconWithUsesController> propControllers = treasureChestBoard.GetComponent<TreasureChestBoardController>().propControllers;
        var y = propControllers;

        if (propControllers[treasureChestId].itemInstance.uses > 0 && propControllers[keyId].itemInstance.uses > 0)
        {
            buttonOpen.GetComponent<SpriteRenderer>().color = Color.gray;
            buttonOpen.GetComponent<BoxCollider2D>().enabled = false;
            PlayFabClientAPI.UnlockContainerItem(new UnlockContainerItemRequest
            {
                ContainerItemId = treasureChestId

            }, result =>
            {
                propControllers[treasureChestId].ModifyUses(-1);
                propControllers[keyId].ModifyUses(-1);
                string str = "Congratulations, you got:\n";
                foreach (ItemInstance instance in result.GrantedItems)
                {
                    ItemIcons[instance.ItemId].GetComponent<ItemIconWithUsesController>().ModifyUses((int)instance.UsesIncrementedBy);
                    ItemIcons[instance.ItemId].GetComponent<ItemIconWithUsesController>().itemInstance.instanceID = instance.ItemInstanceId;
                    str += "\n\t" + instance.UsesIncrementedBy + "  " + instance.DisplayName;
                }
                ShowResult(str);
                buttonOpen.GetComponent<SpriteRenderer>().color = Color.white;
                buttonOpen.GetComponent<BoxCollider2D>().enabled = true;

            }, error =>
             {
                 Debug.LogError(error.GenerateErrorReport());
                 buttonOpen.GetComponent<SpriteRenderer>().color = Color.white;
                 buttonOpen.GetComponent<BoxCollider2D>().enabled = true;
             });
        }
    }
    
    private void ShowResult(string result)
    {
        resultPanel.GetComponent<ResultPanelController>().ShowResult(result);
    }

    public delegate void NextDo();
}

public class SoldItem
{
    public string id;
    public string instanceId;
    public int consumeCount;
    public SoldItem(string _id,string _instanceId,int _consumeCount)
    {
        id = _id;
        instanceId = _instanceId;
        consumeCount = _consumeCount;
    }
}

public class UpdatedSoil
{
    public string species;
    public long? plantTime;
    public int acceleration;
    public UpdatedSoil(string _species, long? _plantTime, int _acceleration)
    {
        species = _species;
        plantTime = _plantTime;
        acceleration = _acceleration;
    }       
}

public class ConsumedItem
{
    public string instanceId;
    public int consumeCount;
    public ConsumedItem(string _instanceID,int _consumeCount)
    {
        instanceId = _instanceID;
        consumeCount = _consumeCount;
    }
}