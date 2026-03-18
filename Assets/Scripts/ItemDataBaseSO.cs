using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Inventory/ItemDataBase")]
public class ItemDataBaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>();

    public Dictionary<int, ItemSO> itemsByid;
    public Dictionary<int, ItemSO> itemsByName;

    public void Initialze()
    {
        itemsByid = new Dictionary<int, ItemSO>();
        itemsByName = new Dictionary<int, ItemSO>();

        foreach(var item in items)
        {
            itemsByid[item.id] = item;
            itemsByName[item.itemName] = item;
        }
    }
    public ItemSO GetItemByid(int id)
    {
        if(itemsByid == null)
        {
            Initialze();
        }
        if(itemsByName.TryGetValue(id, out ItemSO item))
            return item;

        return null;
    }
    public ItemSO GetItemByName(string name)
    {
        if(itemsByName == null)
        {
            Initialze();
        }

        if (itemsByName.TryGetValue(name, out ItemSO item) ) 
            return item;

        return null;
    }
    public List<ItemSO>GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemType==type);
    }
}
