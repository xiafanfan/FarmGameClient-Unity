
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconWithUsesController : MonoBehaviour
{
    public Instance itemInstance;
    [SerializeField]
    private string ID;
    public Text usesText;

    private void Start()
    {
        if (itemInstance == null)
        {
            itemInstance = new Instance(ID, "", 0);
        }
    }

    public int GetUses()
    {
        return itemInstance.uses;
    }

    public bool SetUses(int u)
    {
        if (u < 0)
        {
            Debug.Log("illegal input: uses cannot be negative");
            return false;
        }
        itemInstance.uses = u;
        RefreshUsesText();
        return true;
    }
    public bool ModifyUses(int u)
    {
        if (itemInstance.uses + u < 0)
        {
            Debug.Log("illegal input: uses cannot be negative");
            return false;
        }
        itemInstance.uses += u;
        RefreshUsesText();
        return true;
    }
    public void RefreshUsesText()
    {
        usesText.text = "x " + itemInstance.uses.ToString();
    }
}