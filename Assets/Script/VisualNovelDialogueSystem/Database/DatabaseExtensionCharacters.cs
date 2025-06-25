using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UIElements;


namespace Commands
{
    public class DatabaseExtensionCharacters : DatabaseExtention
    {
        private static string[] paramImmediate => new string[] { "-i", "-immediate" };
        private static string[] paramEnable => new string[] { "-e", "-enable" };
        private static string[] paramSpeed => new string[] { "-spd", "-speed" };
        private static string[] paramSmooth => new string[] { "-sm", "-smooth" };
        private static string paramXPos => "-x";
        private static string paramYPos => "-y";
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));
            database.AddCommand("sort", new Action<string[]>(Sort));

            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.databaseCharatersBase);
            baseCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
            baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));
            baseCommands.AddCommand("setpriority", new Action<string[]> (SetPrority));
            baseCommands.AddCommand("setposition", new Action<string[]>(SetPosition));
            baseCommands.AddCommand("setcolor", new Func<string[], IEnumerator>(SetColor));
            baseCommands.AddCommand("highlight", new Func<string[], IEnumerator>(Highlight));
            baseCommands.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnHighlight));
        }
        private static void Sort(string[] data)
        {
            CharacterManager.instance.SortCharacters(data);
        }
        public static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = data[0];
            Character character = CharacterManager.instance.GetCharacter(characterName);

            if (character == null)
            {
                yield break;
            }

            float x = 0, y = 0;
            float speed = 1;
            bool smooth = false;
            bool immediate = false;

            var parameters =  ConvertDataToParameters(data);

            parameters.TryGetValue(paramXPos, out x);

            parameters.TryGetValue(paramYPos, out y);

            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1);

            parameters.TryGetValue(paramSmooth, out smooth, defaultValue: false);

            parameters.TryGetValue(paramImmediate, out immediate, defaultValue: false);

            Vector2 position = new Vector2(x, y);

            if (immediate)
            {
                character.SetPosition(position);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character.SetPosition(position); });
                yield return character.MoveToPosition(position, speed, smooth);
            }
        }
        public static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            bool enable = false;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramEnable, out enable, defaultValue: false);
            parameters.TryGetValue(paramImmediate, out immediate, defaultValue: false);

            Character character = CharacterManager.instance.CreateCharacter(characterName);

            if(!enable)
            {
                return;
            }

            if (immediate)
            {
                character.isVisible = true;
            }
            else
            {
                character.Show();
            }
        }
        public static IEnumerator ShowAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if(character != null)
                {
                    characters.Add(character);
                }                
            }

            if (characters.Count == 0)
            {
                yield break;
            }

            //convert data array to parameter container
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramImmediate, out immediate, defaultValue: false);
            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1f);

            //call logic on all characters
            foreach (Character character in characters)
            {
                if (immediate)
                {
                    character.isVisible = true;
                }
                else
                {
                    character.Show();
                }
            }
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => 
                {
                    foreach (Character character in characters)
                    {
                        character.isVisible = true;
                    }
                });

                while (characters.Any(c => c.isRevealing))
                {
                    yield return null;
                }
            }
        }
        public static IEnumerator HideAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if (character != null)
                {
                    characters.Add(character);
                }                
            }
            if (characters.Count == 0)
            {
                yield break;
            }

            //convert data array to parameter container
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(paramImmediate, out immediate, defaultValue: false);
            parameters.TryGetValue(paramSpeed, out speed, defaultValue: 1f);

            //call logic on all characters
            foreach (Character character in characters)
            {
                if (immediate)
                {
                    character.isVisible = false;
                }
                else
                {
                    character.Hide();
                }
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                    {
                        character.isVisible = false;
                    }
                });

                while (characters.Any(c => c.isHinding))
                {
                    yield return null;
                }
            }
        }
        private static IEnumerator Show(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
            {
                yield break;
            }

            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
            {
                character.isVisible = true;
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = true; });

                yield return character.Show();
            }

        }
        private static IEnumerator Hide(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
            {
                yield break;
            }

            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
            {
                character.isVisible = false;
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = false; });

                yield return character.Hide();
            }

        }
        public static void SetPrority(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            int priority;

            if(character == null || data.Length < 2)
            {
                return;
            }

            if(!int.TryParse(data[1], out priority))
            {
                priority = 0;
            }

            character.SetPriority(priority);
        }
        public static IEnumerator SetColor(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            string colorName;
            float speed;
            bool immediate;

            if (character == null || data.Length < 2)
            {
                yield break;
            }            
            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            // Try to get the color name
            parameters.TryGetValue(new string[] { "-c", "   -color" }, out colorName);

            // Try to get the speed of the transition
            bool specifiedSpeed = parameters.TryGetValue(new string[] { "-spd", "-speed" }, out speed, defaultValue: 1f);

            // Try to get the instant value
            if (!specifiedSpeed)
            {
                parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: true);
            }
            else
            {
                immediate = false;
            }

            // Get the color value from the name
            Color color = Color.white;
            color = color.GetColorFromName(colorName);

            if (immediate)
            {
                character.SetColor(color);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetColor(color); });
                character.TransitionColor(color, speed);
            }                
            yield break;
        }
        public static IEnumerator Highlight(string[] data)
        {
            // format: SetSprite(character sprite)
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;

            if (character == null)
            {
                yield break;
            }

            bool immediate = false;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
            {
                character.Highlight(immediate: true);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Highlight(immediate: true); });
                yield return character.Highlight();
            }
        }
        public static IEnumerator UnHighlight(string[] data)
        {
            // format: SetSprite(character sprite)
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;

            if (character == null)
            {
                yield break;
            }

            bool immediate = false;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
            {
                character.UnHighlight(immediate: true);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.UnHighlight(immediate: true); });
                yield return character.UnHighlight();
            }
        }
        public static void SetPosition(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            float x = 0, y = 0;

            if (character == null || data.Length < 2)
            {
                return;
            }

            var parameters = ConvertDataToParameters(data, 1);

            parameters.TryGetValue(paramXPos, out x, defaultValue: 0);
            parameters.TryGetValue(paramYPos, out y, defaultValue: 0);

            character.SetPosition(new Vector2(x, y));
        }
    }
}