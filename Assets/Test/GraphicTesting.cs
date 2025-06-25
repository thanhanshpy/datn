using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicTesting : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("BackGround");
        GraphicLayer layer = panel.GetLayer(0, true);

        yield return new WaitForSeconds(1);

        Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/blackHole");
        layer.SetTexture("BackGround/phancanh3.3", blendingTexture: blendTex);

        yield return new WaitForSeconds(1);

        layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", useAudio: true);

        yield return new WaitForSeconds(3);

        layer.currentGraphic.FadeOut();

        yield return new WaitForSeconds(1);

        Debug.Log(layer.currentGraphic);

    }
}
