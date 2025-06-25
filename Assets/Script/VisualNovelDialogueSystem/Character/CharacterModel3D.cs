using UnityEngine;

namespace Characters
{
    public class CharacterModel3D : Character
    {
        public CharacterModel3D(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            Debug.Log($"create 3d model '{name}'");
        }
    }
}