using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtonController : MonoBehaviour
{
    public GameObject itemIcon;
    private ItemIconWithUsesController iconController;
    public Texture2D itemTexture;
    // Start is called before the first frame update
    void Start()
    {
        iconController = itemIcon.GetComponent<ItemIconWithUsesController>();
    }

    private void OnMouseDown()
    {
        Cursor.SetCursor(itemTexture, new Vector2(50,50), CursorMode.Auto);
        if (Game.instance.fertilizers.ContainsKey(iconController.itemInstance.ID))
        {
            Game.instance.operationType = Game.OperationType.Fertilizer;
            Game.instance.operatedItem = iconController.itemInstance;
        }else if (Game.instance.seeds.ContainsKey(iconController.itemInstance.ID))
        {
            Game.instance.operationType = Game.OperationType.Sow;
            Game.instance.operatedItem = iconController.itemInstance;
        }
    }
}
