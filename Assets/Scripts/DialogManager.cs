using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance {  get; private set; }

    [Header("Dialog Reterences")]
    [SerializeField] private DialogDatabaseSO dialogDatabass;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private Image portraitImage;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button NextButton;

    

    private DialogSO currentDialog;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (dialogDatabass != null)
        {
            dialogDatabass.Initailize();
        }
        else
        {
            Debug.LogError("Dialog Database is not assinged to Dialog Manager");
        }

        if(NextButton != null)
        {
            //NextButton.onClick.AddListener(NextDialog);
        }
        else
        {
            Debug.LogError("Next Button is Not assigned!");
        }
    }
    //ID로 대화 시작
    public void StartDialog(int dialogId)
    {
        DialogSO dialog = dialogDatabass.GetDialongByld(dialogId);
        if (dialog != null)
        {
            StartDialog(dialog);
        }
        else
        {
            Debug.LogError($"Dialog with ID {dialogId} not found!");
        }
    }
    public void StartDialog(DialogSO dialog)
    {
        if (dialog == null) return;

        currentDialog= dialog;
        ShowDIalog();
        dialogPanel.SetActive(true);
    }
    public void ShowDIalog()
    {
        if (currentDialog == null) return;
        characterNameText.text = currentDialog.characterName;
        dialogText.text = currentDialog.text;

        if(currentDialog.portrait != null)
        {
            portraitImage.sprite = currentDialog.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else if (!string.IsNullOrEmpty(currentDialog.portraitPath))
        {
            Sprite portrait = Resources.Load<Sprite>(currentDialog.portraitPath);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;    
                portraitImage.gameObject.SetActive(true);   
            }
            else
            {
                Debug.LogWarning($"Portrait not found at path : {currentDialog.portraitPath}");
                portraitImage.gameObject.SetActive(false);
            }
        }
        else
        {
            portraitImage.gameObject.SetActive(false );
        }
    }
    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
        currentDialog = null;
    }
    public void NextDIalog()
    {
        if (currentDialog == null && currentDialog.nextld > 0)
        {
            DialogSO nextDIalog = dialogDatabass.GetDialongByld(currentDialog.nextld);
            if (nextDIalog != null)
            {
                currentDialog = nextDIalog;
                ShowDIalog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }
    void Start()
    {
        CloseDialog();
        StartDialog(1);
    }
}  
