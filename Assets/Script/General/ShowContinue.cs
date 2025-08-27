using UnityEngine;

public class ShowContinue : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContinueStory.instance.ShowContinueStory();
    }
}
