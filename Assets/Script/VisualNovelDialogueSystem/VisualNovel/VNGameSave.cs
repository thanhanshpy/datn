using Dialouge;
using History;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VisualNovel
{
    [System.Serializable]
    public class VNGameSave
    {
        public static VNGameSave activeFile = null;

        public const string fileType = ".txt";

        public const string screenshotFileType = ".jpg";

        public const bool encryptFile = false;

        public const bool encrypt = true;

        public string filePath => $"{FilePath.gameSaves}{slotNumber}{fileType}";

        public string screenshotPath => $"{FilePath.gameSaves}{slotNumber}{screenshotFileType}";

        public string playerName;
        public int slotNumber = 1;

        public bool newGame = true;
        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLogs;
        public VNVariableData[] variables;

        public static VNGameSave Load(string filePath, bool activateOnLoad = false)
        {
            VNGameSave save = FileManager.Load<VNGameSave>(filePath, encrypt);

            activeFile = save;

            if (activateOnLoad)
            {
                save.Activate();
            }

            return save;
        }
        public void Save()
        {
            newGame = false;

            activeState = HistoryState.Capture();
            historyLogs = HistoryManager.instance.history.ToArray();
            activeConversations = GetConversationData();
            variables = GetVariableData();

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON, encrypt);
        }

        public void Activate()
        {
            if(activeState != null)
            {
                activeState.Load();
            }

            HistoryManager.instance.history = historyLogs.ToList();

            SetVariableData();

            SetConversationData();

            DialougeSystem.instance.prompt.Hide();
        }

        private string[] GetConversationData()
        {
            List<string> retData = new List<string>();
            var conversations = DialougeSystem.instance.conversationManager.GetConversationQueue();

            for(int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if(conversation.file != string.Empty)
                {
                    var compressedData = new VNConversationDataCompresss();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endIndex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }
                else
                {
                    var fullData = new VNConversationData();
                    fullData.conversation = conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }
                
                retData.Add(data);
            }

            return retData.ToArray();
        }

        private void SetConversationData()
        {
            for(int i = 0; i < activeConversations.Length; i++)
            {
                try
                {
                    string data = activeConversations[i];
                    Conversation conversation = null;

                    var fullData = JsonUtility.FromJson<VNConversationData>(data);
                    if (fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                    {
                        conversation = new Conversation(fullData.conversation, fullData.progress);
                    }
                    else
                    {
                        var compressedData = JsonUtility.FromJson<VNConversationDataCompresss>(data);
                        if (compressedData != null && compressedData.fileName != string.Empty)
                        {
                            TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);

                            int count = compressedData.endIndex - compressedData.startIndex;

                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();

                            conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                        }
                        else
                        {
                            Debug.LogError($"unable to reload conversation from VnGameSave using data '{data}'");
                        }
                    }

                    if(conversation != null && conversation.GetLines().Count > 0)
                    {
                        if(i == 0)
                        {
                            DialougeSystem.instance.conversationManager.StartConversation(conversation);
                        }
                        else
                        {
                            DialougeSystem.instance.conversationManager.Enqueue(conversation);
                        }
                    }
                }
                catch(System.Exception e)
                {
                    Debug.LogError($"error while extracting saved conversation data {e}");
                    continue;
                }
            }
        }

        private VNVariableData[] GetVariableData()
        {
            List<VNVariableData> retData = new List<VNVariableData>();

            foreach( var database in VariableStore.databases.Values )
            {
                foreach ( var variable in database.variables )
                {
                    VNVariableData variableData = new VNVariableData();
                    variableData.name = $"{database.name}.{variable.Key}";
                    string val = $"{variable.Value.Get()}";
                    variableData.value = val;
                    variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString();
                    retData.Add( variableData );
                }
            }

            return retData.ToArray();
        }

        private void SetVariableData()
        {
            foreach(var variable in variables )
            {
                string val = variable.value;

                switch (variable.type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(val, out bool b_val))
                        {
                            VariableStore.TrySetValue(variable.name, b_val);
                            continue;
                        }
                        break;

                    case "System.Int32":
                        if (int.TryParse(val, out int i_val))
                        {
                            VariableStore.TrySetValue(variable.name, i_val);
                            continue;
                        }
                        break;

                    case "System.Single":
                        if (float.TryParse(val, out float f_val))
                        {
                            VariableStore.TrySetValue(variable.name, f_val);
                            continue;
                        }
                        break;

                    case "System.String":
                        VariableStore.TrySetValue(variable.name, val);
                        continue;  
                }

                Debug.LogError($"can not interpret variable type {variable.name} = {variable.type}");
            }
        }
    }
}