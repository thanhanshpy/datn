using UnityEngine;

namespace Characters
{
    public class CharacterText : Character
    {
        public CharacterText(string name, CharacterConfigData config) : base(name, config, prefab: null)
        {
            Debug.Log($"create character name '{name}'");
        }
    }
}