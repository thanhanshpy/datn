using Dialouge;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Commands
{
    public class DatabaseExtendsionGeneral : DatabaseExtention
    {
        private static readonly string[] paramSpeed = new string[] {"-s", "-spd" };
        private static readonly string[] paramImmdediate = new string[] { "-i", "-immediate" };
        private static readonly string[] paramFilePath = new string[] { "-f", "-file", "-filepath" };
        private static readonly string[] paramEnqueue = new string[] { "-q", "-enqueue" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            database.AddCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));

            database.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogueSystem));

            database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));
            database.AddCommand("setplayername", new Action<string>(SetPlayerNameVariable));
        }
        private static void SetPlayerNameVariable(string data)
        {
            VisualNovel.VNGameSave.activeFile.playerName = data;
        }
        private static void LoadNewDialogueFile(string[] data)
        {
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramFilePath, out fileName);
            parameters.TryGetValue(paramEnqueue, out enqueue, defaultValue: false);

            string filePath = FilePath.GetPathToResource(FilePath.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if(file == null)
            {
                Debug.LogWarning($"file '{filePath}' could not be loaded. Have to ensure it exist in the '{FilePath.resources_dialogueFiles}' file folder");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if (enqueue)
            {
                DialougeSystem.instance.conversationManager.Enqueue(newConversation);
            }
            else
            {
                DialougeSystem.instance.conversationManager.StartConversation(newConversation);
            }
        }
        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1f);
            parameters.TryGetValue(paramImmdediate, out immediate, defaultValue: false);

            yield return DialougeSystem.instance.dialougeContainer.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1f);
            parameters.TryGetValue(paramImmdediate, out immediate, defaultValue: false);

            yield return DialougeSystem.instance.dialougeContainer.Hide(speed, immediate);
        }

        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1f);
            parameters.TryGetValue(paramImmdediate, out immediate, defaultValue: false);

            yield return DialougeSystem.instance.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1f);
            parameters.TryGetValue(paramImmdediate, out immediate, defaultValue: false);

            yield return DialougeSystem.instance.Hide(speed, immediate);
        }

    }
}