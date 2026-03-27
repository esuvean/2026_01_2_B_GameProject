#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Assertions.Must;
using System;

public enum ConversionType
{
    items,
    Dialogs
}
[Serializable]
public class DialogRowData
{
    public int? id;
    public string characterName;
    public string text;
    public int? nextld;
    public string protraitPath;
    public string choiceText;
    public int? choiceNextld;
}

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
            string jsonText = File.ReadAllText(jsonFilePath);
            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO>createdItem = new List<ItemSO>();

            foreach (ItemData itemData in itemDataList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();

                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.namaEng;
                itemSO.description = itemData.description;

                if (System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {itemData.itemName}의 유효하지 않은 타입 : {itemData.itemTypeString}");
                }

                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackble = itemData.isStackable;

                if (!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");

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
                    dataBaseSO.items = createdItem;

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
    public ConversionType conversionType = ConversionType.items;

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

        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type : ", conversionType);

        if (conversionType = ConversionType.Items && outputFolder == "Assets/ScriptableObjects")
        {
            outputFolder = " Assets/ScriptableObjects/Items";
        }
        else if (conversionType == ConversionType.Dialogs && outputFolder == "Assets/ScriptableObjects")
        {
            outputFolder = "Assets/ScriptableObjects/Dialogs";
        }

        createDatabase = EditorGUILayout.Toggle("Create Databse Asset", createDatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert to Scriptable Odjects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Pease Select a JSON file first", "OK");
                return;
            }
            switch (conversionType)
            {
                case ConversionType.items
                    ConvertJsonToItemScriptableObjects();
                    break;
                case ConversionType.Dialogs:
                    ConvertJsonToDialogScriptableObjects();
                    break;
            }
            
        }
    }

    private void ConvertJsonToItemScriptableObjects()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        string JsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON 파싱
            List<DialogRowData>rowDataList = JsonConvert.DeserializeObject<List<DialogRowData>>(JsonText);
            //대화 데이터 재구성
            Dictionary<int, DialogSO> dialogMap = new Dictionary<int, DialogSO>();
            List<DialogSO>createDialogs = new List<DialogSO>();
            //1단계 : 대화 항목 생성
            foreach(var rowData in rowDataList)
            {
                if (!rowData.id.HasValue)       //id 없는 row 는 스킵
                    continue;
                //id 있는 행을 대화로 처리
                DialogSO dialogSO = ScriptableObject.CreateInstance<DialogSO>();
                //데이터 복사
                dialogSO.id = rowData.id.Value;
                dialogSO.characterName = rowData.characterName;
                dialogSO.text = rowData.text;
                dialogSO.nextld = rowData.nextld.HasValue ? rowData.nextld.Value :-1;
                dialogSO.portraitPath = rowData.protraitPath;
                dialogSO.choices = new List<DialogChoiceSO>();
                //초상화 로드(경로가 있는 경우)
                if (!string.IsNullOrEmpty(rowData.protraitPath))
                {
                    dialogSO.portrait = Resources.Load<Sprite>(rowData.protraitPath);

                    if (dialogSO.portrait == null)
                    {
                        Debug.LogWarning($"대화 {rowData.id}의 초상화를 찾을 수 없습니다.");
                    }
                }
                dialogMap[dialogSO.id] = dialogSO;
                createDialogs.Add(dialogSO);
            }
            //2단계 : 선책지 항목 처리 및 연결
            foreach (var rowData in rowDataList)
            {
                //id가 없고 choiceText가 있는 행은 선택지로 처리
                if(!rowData.id.HasValue && !string.IsNullOrEmpty(rowData.choiceText)&& rowData.choiceNextld.HasValue)
                {
                    //이전 행의 ID를 부모 ID로 사용 (연속되는 선택지의 경우)
                    int parentld = -1;

                    //이 선택지 바로 위에 있는 대화 (id가 있는 항목)을 찾음
                    int currentIndex = rowDataList.IndexOf(rowData);
                    for (int i = currentIndex -1; i>=0; i--)
                    {
                        if (rowDataList[i].id.HasValue)
                        {
                            parentld = rowDataList[i].id.Value;
                            break;
                        }
                    }

                    //부모 ID를 찾지 못했거나 부모 ID가 -1인 경우 (첫 번쨰 항목)
                    if (parentld == -1)
                    {
                        Debug.LogWarning($"선택지 {rowData.choiceText}의 부모 대화를 찾을 수 없습니다.");
                    }
                    if(dialogMap.TryGetValue(parentld, out DialogSO parentDialog))
                    {
                        DialogChoiceSO choiceSO = ScriptableObject.CreateInstance<DialogChoiceSO>();
                        choiceSO.text = rowData.choiceText;
                        choiceSO.nextld = rowData.choiceNextld.Value;

                        //선택지 에셋 저장
                        string choiceAssetPath = $"{outputFolder}/Choice_{parentld}_{parentDialog.choices.Count + 1}.asset";
                        AssetDatabase.CreateAsset(choiceSO, choiceAssetPath);
                        EditorUtility.SetDirty(choiceSO);
                        parentDialog.choices.Add(choiceSO);
                    }
                    else
                    {
                        Debug.LogWarning($"선택지 {rowData.choiceText}를 연결할 대화 (ID : {parentld}를 찾을 수 없습니다.");
                    }
                }
            }
            //3단계 : 대화 스크립터블 오브젝트 저장
            foreach(var dialog in createDialogs)
            {
                //스크립터블 오브젝트 저장 - ID 4자리 숫자
                string assetPath = $"{outputFolder}/Dialog {dialog.id.ToString("D4")}.asset";
                AssetDatabase.CreateAsset( dialog, assetPath );

                //에셋 이름 저장
                dialog.name = $"Dialog_{dialog.id.ToString("D4")}";

                EditorUtility.SetDirty(dialog);
            }
            //데이터 베이스 생성
            if (createDatabase && createDialogs.Count > 0)
            {
                DialogChoiceSO database = ScriptableObject.CreateInstance<DialogDatabaseSO>();
                database.dialogs = createDialogs;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/DialogDatabase.asset");
                EditorUtility.SetDirty(database);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", $"created {createDialogs.Count} dialog scriptable Oobjects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Faild to convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 :{e}");
        }
    }
}

#endif