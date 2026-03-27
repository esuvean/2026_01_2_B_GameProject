using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogDatabaseSO", menuName = "Scriptable Objects/DialogDatabaseSO")]
public class DialogDatabaseSO : ScriptableObject
{
    public List<DialogSO> dialogs = new List<DialogSO>();

    private Dictionary<int, DialogSO> dialogsByld;

    public void Initailize()
    {
        dialogsByld = new Dictionary<int, DialogSO>();
        foreach (var dialog in dialogs)
        {
            if (dialogs != null)
            {
                dialogsByld[dialog.id] = dialog;
            }
        }
    }
    public DialogSO GetDialongByld(int id)
    {
        if (dialogsByld == null)
            Initailize();
        if (dialogsByld.TryGetValue(id, out DialogSO dialog))
        {
            return dialog;
        }
        return null;
    }
}
