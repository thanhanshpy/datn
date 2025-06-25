using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{

    private const float buttonMinWidth = 50;
    private const float buttonMaxWidth = 1000;
    private const float buttonWidthPadding = 25;

    private const float buttonHeightPerLine = 50f;
    private const float buttonHeightPadding = 20f;
    public static ChoicePanel instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;

    private CanvasGroupController cg = null;
    private List<ChoiceButton> buttons = new List<ChoiceButton>();
    public string lastInput { get; private set; } = string.Empty;

    public bool isWaitingOnUserInput { get; private set; }

    public bool isWaitingOnUserChoice { get; private set; } = false;
    public ChoicePanelDecision lastDecision { get; private set; } = null;

    private void Awake()
    {
        instance = this;
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteracableState(false);
    }

    public void Show(string question, string[] choices)
    {
        lastDecision = new ChoicePanelDecision(question, choices);
        isWaitingOnUserChoice = true;

        cg.Show();
        cg.SetInteracableState(active: true);

        titleText.text = question;
        StartCoroutine(GenerateChoices(choices));
    }
    private IEnumerator GenerateChoices(string[] choices)
    {
        float maxWidth = 0;

        for (int i = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if(i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, layout = newLayout, title = newTitle };

                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choiceButton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(buttonWidthPadding + choiceButton.title.preferredWidth, buttonMinWidth, buttonMaxWidth);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }

        foreach (var button in buttons)
        {
            button.layout.preferredWidth = maxWidth;
        }

        for(int i = 0; i < buttons.Count; i++)
        {
            bool show = 1 < choices.Length;
            buttons[i].button.gameObject.SetActive(show);
        }

        yield return new WaitForEndOfFrame();

        foreach(var button in buttons)
        {
            int lines = button.title.textInfo.lineCount;
            button.layout.preferredHeight = buttonHeightPadding + (buttonHeightPerLine * lines);
        }
    }
    public void Hide()
    {
        cg.Hide();
        cg.SetInteracableState(false);
    }

    private void AcceptAnswer(int index)
    {
        if(index < 0 || index > lastDecision.choices.Length - 1)
        {
            return;
        }

        lastDecision.answerIndex = index;
        isWaitingOnUserChoice = false;
        Hide();
    }

    public class ChoicePanelDecision
    {
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string question, string[] choices)
        {
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton
    {
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }
}
