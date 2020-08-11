using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public string ID;
    public int price;
    public int num;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Text numText;
    // Start is called before the first frame update
    void Start()
    {
        priceText.text = "GD  " + price;
        numText.text = num.ToString();
    }
    public virtual void Add()
    {
        SetNum(num + 1);
        Debug.Log("ADD");
    }
    public virtual void Sub()
    {
        SetNum(num - 1);
        Debug.Log("SUB");
    }
    public void SetNum(int n)
    {
        num = n;
        numText.text = num.ToString();
    } 

}
