using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoilController : MonoBehaviour
{
    public GameObject plant; //it is used to show plant image 
    public string instanceID;
    public bool ready = false;
    public PlantController plantController;
    public int growthStage;

    private long? plantTime;
    public long? PlantTime
    {
        get
        {
            return plantTime;
        }
        set
        {
            plantTime = value;
            UpdateGrowthStage();
        }
    }

    private string species;
    public string Species
    {
        get
        {
            return species;
        }
        set
        {
            if (species != value)
            {
                if (value != null)
                {
                    plantController.SetSpecies(TransferStr(value));
                    InvokeRepeating("UpdateGrowthStage", 0, 2);
                }
                else
                {
                    plantController.Eradicate();
                    CancelInvoke("UpdateGrowthStage");
                }

            }
            species = value;
        }
    }

    private int acceleration;
    public int Acceleration
    {
        get
        {
            return acceleration;
        }
        set
        {
            acceleration = value;
            UpdateGrowthStage();
        }
    }


    void Start()
    {
        plantController = plant.GetComponent<PlantController>();
    }

    public void ActiveSoil()
    {
        ready = true;
        GetComponent<Animator>().SetTrigger("Active Soil");
        //gameObject.GetComponent<SpriteRenderer>().sprite = soil_ready;
    }


    public static long ConvertDateTimeToInt(System.DateTime time)
    {
        long t = (time.Ticks - 621355968000000000) / 10000;
        return t;
    }

    public void UpdateGrowthStage()
    {
        int new_stage;
        if (species == null || plantTime == null)
        {
            new_stage = 0;
        }
        else
        {
            long now = ConvertDateTimeToInt(DateTime.UtcNow);
            int growth_time = (int)(now - plantTime) / 1000 + acceleration;
            new_stage = getGrowthStage(species, growth_time);
        }
        if (new_stage != growthStage)
        {
            growthStage = new_stage;
            plantController.SetGrowthStage(growthStage);
        }
    }

    static int getGrowthStage(string species, int growth_time)
    {
        long now = ConvertDateTimeToInt(DateTime.UtcNow);
        int new_stage = 0;
        if (growth_time >= Game.instance.seeds[species.ToLower() + "_seed"].growthTime)
        {
            new_stage = 3;
        }
        else if (growth_time >= Game.instance.seeds[species.ToLower() + "_seed"].growthTime * Game.instance.growthStageRatio[2])
        {
            new_stage = 2;
        }
        else if (growth_time >= Game.instance.seeds[species.ToLower() + "_seed"].growthTime * Game.instance.growthStageRatio[1])
        {
            new_stage = 1;
        }
        else
        {
            new_stage = 0;
        }

        return new_stage;
    }


    static string TransferStr(string input)
    {
        string output = "";
        string str = input.Split('-')[0];
        output = (str.Substring(0, 1).ToUpper() + str.Substring(1)) + "Seed";

        return output;
    }

    private void OnMouseDown()
    {
        if (ready)
        {
            //Debug.Log(species);
            if (Game.instance.operationType == Game.OperationType.Fertilizer)
            {
                Accelerate();
            }
            else if (Game.instance.operationType == Game.OperationType.Harvest)
            {
                Harvest();
            }
            else if (Game.instance.operationType == Game.OperationType.Sow)
            {
                Sow();
            }
        }
    }
    //public delegate void Opearte();
    public void Accelerate()
    {
        if (Species != null&&growthStage<3)
        {
            Fertilizer fertilizer = Game.instance.fertilizers[Game.instance.operatedItem.ID];
            if (Game.instance.ItemIcons[fertilizer.ID].GetComponent<ItemIconWithUsesController>().ModifyUses(-1))
            {
                Acceleration += fertilizer.acceleration;
                UpdateSoil();
                ConsumeItem(Game.instance.operatedItem, 1);
            }
        }
    }
    public void Sow()
    {
        if (species == null)
        {
            Seed seed = Game.instance.seeds[Game.instance.operatedItem.ID];
            if (Game.instance.ItemIcons[seed.ID].GetComponent<ItemIconWithUsesController>().ModifyUses(-1))
            {
                long now = ConvertDateTimeToInt(DateTime.UtcNow);
                PlantTime = now;
                Acceleration = 0;
                Species = seed.ID.Split('_')[0];
                UpdateSoil();
                ConsumeItem(Game.instance.operatedItem, 1);
            }
        }
    }
    public void Harvest()
    {
        if (species != null)
        {
            if (growthStage >= 3)
            {
                Debug.Log("harvest");
                ItemBundle productBundle = (ItemBundle)Game.instance.catalogItems[Game.instance.seeds[species.ToLower() + "_seed"].product];
                Game.instance.ItemIcons[productBundle.itemContains].GetComponent<ItemIconWithUsesController>().ModifyUses(productBundle.usesContains);
                Game.instance.itemGrants.Add(Game.instance.seeds[species.ToLower() + "_seed"].product);
            }
            else
            {
                Debug.Log("eradicate");
            }
            Species = null;
            PlantTime = null;
            Acceleration = 0;
            UpdateSoil();
        }
    }
    void UpdateSoil()
    {
        if (Game.instance.soilUpdates.ContainsKey(instanceID))
        {
            Game.instance.soilUpdates[instanceID].species = species;
            Game.instance.soilUpdates[instanceID].plantTime = plantTime;
            Game.instance.soilUpdates[instanceID].acceleration = acceleration;
        }
        else
        {
            Game.instance.soilUpdates.Add(instanceID, new UpdatedSoil(species, plantTime, acceleration));
        }
    }
    void ConsumeItem(Instance item,int _uses)
    {
        if (Game.instance.itemConsumes.ContainsKey(item.ID))
        {
            Game.instance.itemConsumes[item.ID].consumeCount += _uses;
        }
        else
        {
            Game.instance.itemConsumes.Add(item.ID, new ConsumedItem(item.instanceID, _uses));
        }
    }
}
