using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GraphicObject 
{
    private const string nameFormat = "Graphic - [{0}]";
    private const string defaultUIMaterial = "Default UI Material";
    private const string materialPath = "Materials/layerTransitionMaterial";
    private const string materialFieldColor = "_Color";
    private const string materialFieldMainTex = "_MainTex";
    private const string materialFieldBlendTexX = "_BlendTex";
    private const string materialFieldBlend = "_Blend";
    private const string materialFieldAlpha = "_Alpha";

    private GraphicLayer layer;
    public RawImage renderer;

    public VideoPlayer video = null;
    public bool useAudio => (audio != null ? !audio.mute : false);
    public bool isVideo { get { return video != null; } }

    public AudioSource audio = null;

    public string graphicPath = "";
    public string graphicName { get; private set; } 

    private Coroutine co_fadingin = null;
    private Coroutine co_fadingout = null;

    public GraphicObject(GraphicLayer layer ,string graphicPath, Texture tex, bool immediate)
    {
        this.graphicPath = graphicPath;
        this.layer = layer;

        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        renderer = ob.AddComponent<RawImage>();

        graphicName = tex.name;

        InitGraphic(immediate);

        renderer.name = string.Format(nameFormat, graphicName);

        renderer.material.SetTexture(materialFieldMainTex, tex);
    }

    public GraphicObject(GraphicLayer layer, string graphicPath, VideoClip clip, bool useAudio, bool immediate)
    {
        this.graphicPath = graphicPath;
        this.layer = layer;

        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        renderer = ob.AddComponent<RawImage>();

        graphicName = clip.name;
        renderer.name = string.Format(nameFormat, graphicName);

        InitGraphic(immediate);

        RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
        renderer.material.SetTexture(materialFieldMainTex, tex);

        video = renderer.AddComponent<VideoPlayer>();
        video.playOnAwake = true;
        video.source = VideoSource.VideoClip;
        video.clip = clip;
        video.renderMode = VideoRenderMode.RenderTexture;
        video.targetTexture = tex;
        video.isLooping = true;

        video.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audio = video.AddComponent<AudioSource>();

        audio.volume = immediate ? 1 : 0;
        if (!useAudio)
        {
            audio.mute = true;
        }

        video.SetTargetAudioSource(0, audio);

        video.frame = 0;
        video.Prepare();
        video.Play();

        video.enabled = false;
        video.enabled = true;
    }

    private void InitGraphic(bool immediate)
    {
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect = renderer.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        renderer.material = GetTransitonMaterial();

        float startingOpacity = immediate ? 1.0f : 0.0f;
        renderer.material.SetFloat(materialFieldBlend, startingOpacity);
        renderer.material.SetFloat(materialFieldAlpha, startingOpacity);

    }
    private Material GetTransitonMaterial()
    {
        Material mat = Resources.Load<Material>(materialPath);

        if(mat != null)
        {
            return new Material(mat);
        }

        return null;
    }

    GraphicPanelManager PanelManager => GraphicPanelManager.instance;
    public Coroutine FadeIn(float speed = 1f, Texture blend = null)
    {
        if(co_fadingout != null)
        {
            PanelManager.StopCoroutine(co_fadingout);
        }

        if(co_fadingin != null)
        {
            return co_fadingin;
        }

        co_fadingin = PanelManager.StartCoroutine(Fading(1f, speed, blend));

        return co_fadingin;
    }

    public Coroutine FadeOut(float speed = 1f, Texture blend = null)
    {
        if (co_fadingin != null)
        {
            PanelManager.StopCoroutine(co_fadingin);
        }

        if (co_fadingout != null)
        {
            return co_fadingout;
        }

        co_fadingout = PanelManager.StartCoroutine(Fading(0f, speed, blend));

        return co_fadingout;
    }

    public IEnumerator Fading(float target, float speed, Texture blend = null)
    {
        bool isBlending = blend != null;
        bool fadingIn = target > 0;

        if(renderer.material.name == defaultUIMaterial)
        {
            Texture tex = renderer.material.GetTexture(materialFieldMainTex);
            renderer.material = GetTransitonMaterial();
            renderer.material.SetTexture(materialFieldMainTex, tex);
        }

        renderer.material.SetTexture(materialFieldBlendTexX, blend);
        renderer.material.SetFloat(materialFieldAlpha, isBlending ? 1 : fadingIn ? 0 : 1);
        renderer.material.SetFloat(materialFieldBlend, isBlending ? fadingIn ? 0 : 1 : 1);
        
        string opacityParam = isBlending ? materialFieldBlend : materialFieldAlpha;

        while(renderer.material.GetFloat(opacityParam) != target)
        {
            float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
            renderer.material.SetFloat(opacityParam, opacity);

            if(isVideo)
            {
                audio.volume = opacity;
            }

            yield return null;
        }

        co_fadingin = null;
        co_fadingout = null;

        if(target == 0)
        {
            Destroy();
        }
        else
        {
            DestroyBackGroundGraphicOnLayer();
            renderer.texture = renderer.material.GetTexture(materialFieldMainTex);
            renderer.material = null;
        }
    }
    public void Destroy()
    {
        if(layer.currentGraphic != null && layer.currentGraphic.renderer == renderer)
        {
            layer.currentGraphic = null;
        }

        if (layer.oldGraphics.Contains(this))
        {
            layer.oldGraphics.Remove(this);
        }

        GameObject.Destroy(renderer.gameObject);
    }

    private void DestroyBackGroundGraphicOnLayer()
    {
        layer.DestroyOldGraphics();
    }
}
