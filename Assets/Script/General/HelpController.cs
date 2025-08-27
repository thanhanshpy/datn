using UnityEngine;

public class HelpController : MonoBehaviour
{
    public GameObject HelpUI;
  
    public void ShowHelpUI()
    {
        HelpUI.SetActive(true);
    }

    public void HideHelpUI()
    {
        HelpUI.SetActive(false);
    }
}
