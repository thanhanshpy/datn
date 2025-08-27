using UnityEngine;
using TMPro;

namespace Dialouge
{
    [System.Serializable]
    public class DialougeContainer
    {
        private CanvasGroupController cgController;

        public GameObject root;
        public TextMeshProUGUI dialogueText;
        public NameContainer nameContainer;
        
        public void SetDialogueColor(Color color) => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;

        private bool initialized = false;
       
        public void Initialized()
        {
            if (initialized) return;

            cgController = new CanvasGroupController(DialougeSystem.instance, root.GetComponent<CanvasGroup>());
        }
        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }
}

