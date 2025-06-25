using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    private const string sfxParentName = "SFX";
    private const string sfxNameFormat = "SFX - [{0}]";
    public const float trackTransitionSpeed = 1f;
    public static AudioManager instance { get; private set; }
    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxcMixer;
    public AudioMixerGroup voicesMixer;

    private Transform sfxRoot;
    private void Awake()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        sfxRoot = new GameObject(sfxParentName).transform;
        sfxRoot.SetParent(transform);
    }

    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"can not load audio file '{filePath}'");
            return null;
        }

        return PlaySoundEffect(clip, mixer, volume, pitch, loop);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioSource effectSource = new GameObject(string.Format(sfxNameFormat, clip.name)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;

        effectSource.clip = clip;

        if(mixer == null)
        {
            mixer = sfxcMixer;
        }

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0;
        effectSource.loop = loop;
        effectSource.pitch = pitch;

        effectSource.Play();

        if (!loop)
        {
            Destroy(effectSource.gameObject, (clip.length/pitch) +1);
        }
        return effectSource;
    }
    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);
    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            if(source.clip.name.ToLower()  == soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }
    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"can not load audio file '{filePath}'");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch,filePath);
    }
    public AudioTrack PlayTrack(AudioClip clip , int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfDoedNotExist: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch,filePath);
        return track;
    }
    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoedNotExist = false)
    {
        AudioChannel channel = null;

        if(channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if(createIfDoedNotExist)
        {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }
    public void StopTrack(int channel)
    {
        AudioChannel c = TryGetChannel(channel, createIfDoedNotExist:  false);

        if(c == null)
        {
            return;
        }

        c.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach(var channel in channels.Values)
        {
            if(channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }
}
