#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Assertions.Must;

public class JsonToScriptableConverter : EditorWindow
{
    private void ConvertJsonToScriptableObjects()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        try
        {
            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO>createdItem = new List<ItemSO>();

            foreach (ItemData itemData in itemDataList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();

                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.nameEng;
                itemSO.description = itemData.description;

                if (System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {itemData,itemName}의 유효하지 않은 타입 : {itemData.itemTypeString}");
                }

                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackble = itemData.isStackable;

                if (!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAllAssetsAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");

                    if (itemSO.icon = null)
                    {
                        Debug.LogWarning($"아이템 {itemData.namaEng}의 아이콘을 찾을 수 없습니다. : {itemData.iconPath}");
                    }
                }

                string assetPath = $"{outputFolder}/Item_{itemData.id.ToString("D4")}_{itemData.namaEng}.asset";
                AssetDatabase.CreateAsset( itemSO, assetPath );

                itemSO.name = $"Item_{itemData.id.ToString("D4")}+{itemData.namaEng}";
                createdItem.Add( itemSO );

                EditorUtility.SetDirty( itemSO );

                if(createDatabase && createdItem.Count > 0 )
                {
                    ItemDataBaseSO dataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();
                    dataBaseSO.iteams = createdItem;

                    AssetDatabase.CreateAsset(dataBaseSO, $"{outputFolder}/ItemDatabase.asset");
                    EditorUtility.SetDirty ( dataBaseSO );
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Sucess", $"Created {createdItem.Count} scriptable dbjects!", "OK");
            }
        }
        catch(System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }

    private string jsonFilePath = "";
    private string outputFolder = "Assets/ScriptableObjects/Items";
    private bool createDatabase = true;

    [MenuItem("Tools/JSON to Scriptable Objects")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("JSON to Scriptable Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Seriptable object Conerter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();
        outputFolder = EditorGUILayout.TextField("Output Foloder : ", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Databse Asset", createDatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert to Scriptable Odjects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Pease Select a JSON file first", "OK");
                return;
            }
            ConvertJsonToScriptableObjects();
        }
    }
}

#endif