using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelText.Baking
{
    [Serializable]
    public struct SSymbolTexture
    {
        public char Symbol;
        public Texture2D Texture;
    }

    [CreateAssetMenu(fileName = "TexturesStore", menuName = "Symbol Textures/Store", order = 0)]
    public class SymbolTexturesStore: ScriptableObject
    {
        [SerializeField]
        List<SSymbolTexture> SymbolStore = new List<SSymbolTexture>();
        Dictionary<char, Texture2D> _symbols = new Dictionary<char, Texture2D>();

        private void OnEnable()
        {
            foreach(var item in SymbolStore)
            {
                _symbols.Add(item.Symbol, item.Texture);
            }
        }

        public Texture2D GetTexture(char symbol)
        {
            return _symbols[symbol];
        }
    }
}
