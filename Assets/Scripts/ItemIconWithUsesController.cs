
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconWithUsesController : MonoBehaviour
{
    public Instance itemInstance;
    public Text usesText;

    public void SetUses(int u)
    {
        if (u < 0)
        {
            Debug.Log("illegal input: uses cannot be negative");
        }
        itemInstance.uses = u;
        RefreshUsesText();
    }
    public void ModifyUses(int u)
    {
        if (itemInstance.uses < u)
        {
            Debug.Log("illegal input: uses cannot be negative");
        }
        itemInstance.uses += u;
        RefreshUsesText();
    }
    public void RefreshUsesText()
    {
        usesText.text = "x " + itemInstance.uses.ToString();
    }
}
