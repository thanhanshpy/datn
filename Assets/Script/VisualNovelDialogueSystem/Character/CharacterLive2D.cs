using UnityEngine;

namespace Characters
{
    public class CharacterLive2D : Character
    {
        public CharacterLive2D(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            Debug.Log($"create live 2d '{name}'");
        }
    }
}