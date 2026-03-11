using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ItemDataLoater : MonoBehaviour
{
    [SerializeField]
    private string jsonFilePath = "items";
    private List<ItemData> itemList;

    void Start()
    {
        LoadItemData();
    }

    private string EncodeKorean(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        byte[] bytes = Encoding.Default.GetBytes(input);
        return Encoding.UTF8.GetString(bytes);
    }

    void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFilePath);

        if (jsonFile == null)
        {
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string ucrrnetText = Encoding.UTF8.GetString(bytes);

            itemList = JsonConvert.DeserializeObject<List<ItemData>>(ucrrnetText);

            Debug.Log($"로드된 아이템 수 : {itemList.Count}");

            foreach (var item in itemList)
            {
                Debug.Log($"아이템: {EncodeKorean(item.itemName)}, 설명: {EncodeKorean(item.description)}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {jsonFilePath}");
        }
    }

}
