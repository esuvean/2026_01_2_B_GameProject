using System;
using UnityEngine;

[Serializable]
public class ItemData 
{
    public int id;
    public string itemName;
    public string description;
    public string namaEng;
    public string itemTypeString;

    [NonSerialized]
    public ItemType itemType;
    public int price;
    public int power;
    public int level;
    public bool isStackable;
    public string iconPath;

    public void InitalizeEnums()
    {
        if (Enum.TryParse(itemTypeString, out ItemType parsedItemType))
        {
            itemType = parsedItemType;
        }
        else
        {
            Debug.LogError($"아이템 ' {itemName} 에 유효허자 않은 아이템 타입 :  {itemTypeString}");
            //기본값 설정
            itemType = ItemType.Consumable;
        }

    }

}
