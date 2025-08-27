using System;
using UnityEngine;

namespace Commands
{
    public class DatabaseExtensionAudio : DatabaseExtention
    {
        private static string[] paramSfx = new string[] { "-s", "-sfx"};
        private static string[] paramVolume = new string[] { "-v", "-vol", "-volume" };
        private static string[] paramPitch = new string[] { "-p", "-pitch" };
        private static string[] paramLoop = new string[] { "-l", "-loop" };

        private static string[] paramChannel = new string[] { "-c", "-channel" };
        private static string[] paramImmediate = new string[] { "-v", "-immediate" };
        private static string[] paramStartVolume = new string[] { "-sv", "-startvolume" };
        private static string[] paramSong = new string[] { "-s", "-song" };
        private static string[] paramAmbience = new string[] { "-a", "-ambience" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySfx));
            database.AddCommand("stopsfx", new Action<string>(StopSfx));

            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("playambience", new Action<string[]>(PlayAmbience));

            database.AddCommand("stopsong", new Action<string>(StopSong));
            database.AddCommand("stopambience", new Action<string>(StopAmbience));
        }

        private static void PlaySfx(string[] data)
        {
            string filePath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramSfx, out filePath);

            parameters.TryGetValue(paramVolume, out volume, defaultValue: 1f);

            parameters.TryGetValue(paramPitch, out pitch, defaultValue: 1f);

            parameters.TryGetValue(paramLoop, out loop, defaultValue: false);

            AudioClip sound = Resources.Load<AudioClip>(FilePath.GetPathToResource(FilePath.resources_sfx, filePath));

            if(sound == null)
            {
                return;
            }

            AudioManager.instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop);
        }

        private static void StopSfx(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }
        private static void PlaySong(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // Try to get the name or path to the track
            parameters.TryGetValue(paramSong, out filepath);
            filepath = FilePath.GetPathToResource(FilePath.resources_music, filepath);

            // Try to get the channel for this track
            parameters.TryGetValue(paramChannel, out channel, defaultValue: 1);

            PlayTrack(filepath, channel, parameters);
        }

        private static void PlayAmbience(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // Try to get the name or path to the track
            parameters.TryGetValue(paramAmbience, out filepath);
            filepath = FilePath.GetPathToResource(FilePath.resources_ambience, filepath);

            // Try to get the channel for this track
            parameters.TryGetValue(paramChannel, out channel, defaultValue: 0);

            PlayTrack(filepath, channel, parameters);
        }
        private static void PlayTrack(string filepath, int channel, CommandParameters parameters)
        {
            bool loop;
            float volumeCap;
            float startVolume;
            float pitch;

            // Try to get the max volume of the track
            parameters.TryGetValue(paramVolume, out volumeCap, defaultValue: 1f);

            // Try to get the start volume of the track
            parameters.TryGetValue(paramStartVolume, out startVolume, defaultValue: 0f);

            // Try to get the pitch of the track
            parameters.TryGetValue(paramPitch, out pitch, defaultValue: 1f);

            // Try to get if this track loops
            parameters.TryGetValue(paramLoop, out loop, defaultValue: true);
         
            // Run the logic
            AudioClip sound = Resources.Load<AudioClip>(filepath);

            if (sound == null)
            {
                Debug.Log($"Was not able to load voice '{filepath}'");
                return;
            }

            AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filepath);
        }
        private static void StopTrack(string data)
        {
            if(int.TryParse(data, out int channel))
            {
                AudioManager.instance.StopTrack(channel);
            }
            else
            {
                AudioManager.instance.StopTrack(data);
            }
        }
        private static void StopSong(string data)
        {
            if(data == string.Empty)
            {
                StopTrack("1");
            }
            else
            {
                StopTrack(data);
            }
        }
        private static void StopAmbience(string data)
        {
            if (data == string.Empty)
            {
                StopTrack("0");
            }
            else
            {
                StopTrack(data);
            }
        }
    }
}