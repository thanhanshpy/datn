using Dialouge;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        public Character[] allCharacters => characters.Values.ToArray();

        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfigSO config => DialougeSystem.instance.config.characterConfigurationAsset;
        private const string characterCastingID = " as ";
        private const string characterNameID = "<charname>";
        public string characterRootPathFormat => $"Characters/{characterNameID}";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{charaterPrefabNameFormat}";
        public string charaterPrefabNameFormat => $"Character - [{characterNameID}]";

        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;
        private void Awake()
        {
            instance = this;
        }
        public CharacterConfigData GetCharacterConfig(string characterName, bool getOriginal = false)
        {
            if(!getOriginal)
            {
                Character character = GetCharacter(characterName);
                if (character != null)
                {
                    return character.config;
                }
            }
                        
            return config.GetConfig(characterName);
        }
        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower())) 
            {
                return characters[characterName.ToLower()];
            }
            else if(createIfDoesNotExist)
            {
                return CreateCharacter(characterName);
            }
            
            return null;
        }
        public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());
        public Character CreateCharacter(string characterName, bool revealAfterCreation = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"a character name '{characterName}' already exist");
                return null;
            }

            CharacterInfo info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(info.name.ToLower(), character);

            if (revealAfterCreation)
            {
                character.Show();
            }

            return character;
        }

        private CharacterInfo GetCharacterInfo(string characterName)
        {
            CharacterInfo result = new CharacterInfo();

            string[] nameData = characterName.Split(characterCastingID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

            result.config = config.GetConfig(result.castingName);

            result.prefab = GetPrefabForCharacter(result.castingName);

            result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);

            return result;
        }
        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }

        public string FormatCharacterPath(string path, string characterName) => path.Replace(characterNameID, characterName);
       
        private Character CreateCharacterFromInfo(CharacterInfo info)
        {
            CharacterConfigData config = info.config;

            switch (info.config.characterType)
            {
               case Character.CharacterType.Text:
                    return new CharacterText(info.name, config);

               case Character.CharacterType.Sprite:
               case Character.CharacterType.SpriteSheet:
                    return new CharacterSprite(info.name, config, info.prefab, info.rootCharacterFolder);

               case Character.CharacterType.Live2D:
                    return new CharacterLive2D(info.name, config, info.prefab, info.rootCharacterFolder);

               case Character.CharacterType.Model3D:
                    return new CharacterModel3D(info.name, config, info.prefab, info.rootCharacterFolder);

               default:
                    return null;
            }
        }

        public void SortCharacters()
        {
            List<Character> activeCharacters = characters.Values.Where(c => c.root.gameObject.activeInHierarchy && c.isVisible).ToList();
            List<Character> inActiveCharacters = characters.Values.Except(activeCharacters).ToList();

            activeCharacters.Sort((a, b) => a.priority.CompareTo(b.priority));
            activeCharacters.Concat(inActiveCharacters);

            SortCharacters(activeCharacters);
        }

        public void SortCharacters(string[] characterNames)
        {
            List<Character> sortedCharacter = new List<Character>();

            sortedCharacter = characterNames.Select(name => GetCharacter(name)).Where(character => character != null).ToList();

            List<Character> remainingCharacters = characters.Values.Except(sortedCharacter).OrderBy(character => character.priority).ToList();

            sortedCharacter.Reverse();

            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c => c.priority) : 0;
            for (int i = 0; i < sortedCharacter.Count; i++)
            {
                Character character = sortedCharacter[i];
                character.SetPriority(startingPriority + i + 1, autoSortCharactersOnUI: false);
            }

            List<Character> allCharacters = remainingCharacters.Concat(sortedCharacter).ToList();
            SortCharacters(allCharacters);
        }
        private void SortCharacters(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach (Character character in charactersSortingOrder)
            {
                character.root.SetSiblingIndex(i++);
            }
        }
        private class CharacterInfo
        {
            public string name = "";

            public CharacterConfigData config= null;

            public GameObject prefab = null;

            public string castingName = "";

            public string rootCharacterFolder = "";
        }
    }
}