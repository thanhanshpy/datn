using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MonoBehaviour
{
    public static InputPanel instance { get; private set; } = null; 

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController cg;
    public string lastInput {  get; private set; } = string.Empty;

    public bool isWaitingOnUserInput { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteracableState(active:  false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        cg.Show();
        cg.SetInteracableState(active: true);
        isWaitingOnUserInput = true;
    }

    public void Hide()
    {
        cg.Hide();
        cg.SetInteracableState(active: false);
        isWaitingOnUserInput=false; 
    }
    public void OnAcceptInput()
    {
        if(inputField.text == string.Empty)
        {
            return;
        }

        lastInput = inputField.text;
        Hide();
    }
    
    public void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }
    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
}
