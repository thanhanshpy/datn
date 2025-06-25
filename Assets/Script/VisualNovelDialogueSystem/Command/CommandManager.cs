using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using Characters;

namespace Commands
{
    public class CommandManager : MonoBehaviour
    {
        private const char subCommandIdentifier = '.';
        public const string databaseCharatersBase = "characters";
        public const string databaseCharatersSprite = "charactersprite";
        public const string databaseCharatersLive2D = "characterlive2D";
        public const string databaseCharatersModel3D = "charactermodel3D";

        public static CommandManager instance { get; private set; }                
        private CommandDatabase database;
        private Dictionary<string, CommandDatabase> subDatabases = new Dictionary<string, CommandDatabase>();
        private List<CommandProcess> activeProcesses = new List<CommandProcess>();
        private CommandProcess topProcess => activeProcesses.Last();
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;

                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DatabaseExtention))).ToArray();

                foreach (Type extension in extensionTypes)
                {
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(null, new object[] { database });
                }
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        public CoroutineWrapper Execute(string commandName, params string[] args)
        {
            if(commandName.Contains(subCommandIdentifier))
            {
                return ExecuteSubCommand(commandName, args);
            }

            Delegate command = database.GetCommand(commandName);

            if (command == null)
            {
                return null;
            }

            return StartProcess(commandName, command, args);
        }

        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            string[] parts = commandName.Split(subCommandIdentifier);
            string databaseName = string.Join(subCommandIdentifier, parts.Take(parts.Length - 1));
            string subCommandName = parts.Last();

            if (subDatabases.ContainsKey(databaseName))
            {
                Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
                if(command != null)
                {
                    return StartProcess(commandName, command, args);
                }
                else
                {
                    Debug.LogError($"no command called '{subCommandName}' was found in sub database '{databaseName}'");
                    return null;
                }
            }

            string characterName = databaseName;
            //try to run as a character commnand
            if (CharacterManager.instance.HasCharacter(characterName))
            {
                List<string> newArgs = new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();

                return ExecuteCharacterCommand(subCommandName, args);
            }

            Debug.LogError($"no sub database called '{databaseName}' exist, command '{subCommandName}' could not be run");
            return null;
        }

        private CoroutineWrapper ExecuteCharacterCommand(string commandName,params string[] args)
        {
            Delegate command = null;

            CommandDatabase db = subDatabases[databaseCharatersBase];
            if(db.HasCommand(commandName))
            {
                command = db.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }

            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
            switch(characterConfigData.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    db = subDatabases[databaseCharatersSprite];
                    break;
                case Character.CharacterType.Live2D:
                    db = subDatabases[databaseCharatersLive2D];
                    break;
                case Character.CharacterType.Model3D:
                    db = subDatabases[databaseCharatersModel3D];
                    break;
            }

            command = db.GetCommand(commandName);

            if(command != null)
            {
                return StartProcess(commandName, command, args);
            }

            Debug.LogError($"command manager was unable to execute '{commandName}' on character '{args[0]}'");
            return null;
        }
        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            System.Guid processID = System.Guid.NewGuid();
            CommandProcess cmd = new CommandProcess(processID, commandName, command, null, args, null);
            activeProcesses.Add(cmd);

            Coroutine co = StartCoroutine(RunningProcess(cmd));

            cmd.runningProcess = new CoroutineWrapper(this, co);

            return cmd.runningProcess;
        }
        public void StopCurrentProcess()
        {
            if (topProcess != null)
            {
                KillProcess(topProcess);
            }
        }
        public void StopAllProcess()
        {
            foreach(var c in activeProcesses)
            {
                if(c.runningProcess != null && !c.runningProcess.isDone)
                {
                    c.runningProcess.Stop();
                }

                c.onTerminateAction?.Invoke();
            }

            activeProcesses.Clear();
        }
        private IEnumerator RunningProcess(CommandProcess process)
        {
            yield return WaitingForProcessToComplete(process.command, process.args);

            KillProcess(process);
        }
        public void KillProcess(CommandProcess cmd)
        {
            activeProcesses.Remove(cmd);

            if(cmd.runningProcess != null && !cmd.runningProcess.isDone)
            {
                cmd.runningProcess.Stop();
            }

            cmd.onTerminateAction?.Invoke();
        }

        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            if (command is Action)
            {
                command.DynamicInvoke();
            }
            else if (command is Action<string>)
            {
                command.DynamicInvoke(args.Length == 0? string.Empty : args[0]);
            }
            else if (command is Action<string[]>)
            {
                command.DynamicInvoke((object)args);
            }
            else if (command is Func<IEnumerator>)
            {
                yield return ((Func<IEnumerator>)command)();
            }
            else if (command is Func<string, IEnumerator>)
            {
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);
            }
            else if (command is Func<string[], IEnumerator>)
            {
                yield return ((Func<string[], IEnumerator>)command)(args);
            }
        }
        public void AddTerminationActionToCurrentProcess(UnityAction action)
        {
            CommandProcess process = topProcess;

            if(process == null)
            {
                return;
            }

            process.onTerminateAction = new UnityEvent();
            process.onTerminateAction.AddListener(action);
        }
        public CommandDatabase CreateSubDatabase(string name)
        {
            name = name.ToLower();

            if(subDatabases.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($" a database by the name of '{name}' already exist");
                return db;
            }

            CommandDatabase newDatabase = new CommandDatabase();
            subDatabases.Add(name, newDatabase);

            return newDatabase;
        }
    }
}