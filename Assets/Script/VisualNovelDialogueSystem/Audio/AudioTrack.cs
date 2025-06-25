using UnityEngine;
using UnityEngine.Audio;

public class AudioTrack 
{
    public string path {  get; private set; }

    private const string trackNameFormat = "Track - [{0}]";
    public string name {  get; private set; }
    public GameObject root => source.gameObject;

    private AudioChannel channel;
    private AudioSource source;
    public bool isPlaying => source.isPlaying;
    public bool loop => source.loop;
    public float volumeCap { get; private set; }

    public float volume { get {  return source.volume; } set { source.volume = value; } }
    public float pitch { get { return source.pitch; } set { source.pitch = value; } }

    public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap,float pitch, AudioChannel channel, AudioMixerGroup mixer, string filePath)
    {
        name = clip.name;
        this.channel = channel;
        this.volumeCap = volumeCap;
        path = filePath;
        source = CreateSource();
        source.loop = loop;
        source.volume = startingVolume;
        source.clip = clip;
        source.outputAudioMixerGroup = mixer;
        source.pitch = pitch;
        this.path = path;
    }

    private AudioSource CreateSource()
    {
        GameObject go =new GameObject(string.Format(trackNameFormat, name));

        go.transform.SetParent(channel.trackContainer);
        AudioSource source = go.AddComponent<AudioSource>();

        return source;
    }

    public void Play()
    {
        source.Play();
    }
    public void Stop()
    {
        source.Stop();
    }

    
}
