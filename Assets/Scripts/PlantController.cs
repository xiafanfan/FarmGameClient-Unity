using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }


    public void SetSpecies(string seed)
    {
        anim.SetTrigger("Sow" + seed);
    }

    public void SetGrowthStage(int s)
    {
        anim.SetFloat("GrowthStage", s);
    }
    public void Eradicate()
    {
        anim.SetTrigger("Eradicate");
    }
}

