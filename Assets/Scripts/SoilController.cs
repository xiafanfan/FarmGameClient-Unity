using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilController : MonoBehaviour
{
    public GameObject plant; //it is used to show plant image 
    public Sprite soil_ready;
    private long plantTime;
    public long PlantTime
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

    public int growthStage;



    public string instanceID;
    public bool ready = false;
    public PlantController plantController;
    void Start()
    {
        plantController = plant.GetComponent<PlantController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ActiveSoil()
    {
        ready = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = soil_ready;
    }


    public static long ConvertDateTimeToInt(System.DateTime time)
    {
        long t = (time.Ticks - 621355968000000000) / 10000;
        return t;
    }

    public void UpdateGrowthStage()
    {
        long now = ConvertDateTimeToInt(DateTime.UtcNow);

        int growth_time = (int)(now - plantTime) / 1000 + acceleration;
        int new_stage = getGrowthStage(species, growth_time);
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
        if (growth_time >= SingleDemo.instance.seeds[species.ToLower() + "_seed"].growthTime)
        {
            new_stage = 3;
        }
        else if (growth_time >= SingleDemo.instance.seeds[species.ToLower() + "_seed"].growthTime * SingleDemo.instance.growthStageRatio[2])
        {
            new_stage = 2;
        }
        else if (growth_time >= SingleDemo.instance.seeds[species.ToLower() + "_seed"].growthTime * SingleDemo.instance.growthStageRatio[1])
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
        Debug.Log(species);
    }

    public delegate void Opearte();
    public bool Accelerate(Fertilizer fertilizer)
    {
        Acceleration += fertilizer.acceleration;
        return true;
    }
    public void Sow(Seed seed)
    {
        long now = ConvertDateTimeToInt(DateTime.UtcNow);
        PlantTime = now;
        Acceleration = 0;
        Species = seed.ID.Split('_')[0];
    }
    public void Harvest()
    {
        if (growthStage >= 3)
        {
            Debug.Log("harvest");
            //grant
        }
        else
        {
            Debug.Log("eradicate");
        }
        PlantTime = 0;
        Acceleration = 0;
        Species = null;
    }
}
