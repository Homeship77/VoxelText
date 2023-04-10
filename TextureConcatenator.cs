using Assets._scripts.Baking;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace VoxelText.Baking
{
    public static class TextureConcatenator
    {
        static SymbolTexturesStore _store;

        static Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

        static bool _inited = false;
        public static void Init()
        {
            if (_inited)
                return;

            _store = Resources.Load("TexturesStore") as SymbolTexturesStore;

            _inited = true; 
        }

        public static void SetTexturesStore(SymbolTexturesStore input)
        {
            _store = input;
        }

        public static Texture2D GenerateSummaryTexture(string text)
        {
            Texture2D res = null;
            if (_textureCache.ContainsKey(text))
            {
                return _textureCache[text];
            }
            
            if (_store == null)
            {
                Init();
            }
            if (_store == null)
            {
                return res;
            }

            List<Texture2D> tex_list = new List<Texture2D>();
            foreach(var ch in text)
            {
                var tex = _store.GetTexture(ch);
                if (tex == null)
                    continue;
                tex_list.Add(tex);
            }

            int max_height = 0;
            foreach (var tex in tex_list)
            {
                if (max_height < tex.height)
                    max_height = tex.height;
            }
            int result_width = 0;
            foreach (var tex in tex_list)
            {
                if (tex.height < max_height)
                {
                    ScaleTextureHeight(tex, max_height);
                }
                result_width += tex.width;

            }
            res = new Texture2D(result_width, max_height, TextureFormat.ARGB32, false);
            var res_pixels = res.GetPixels32();
            int res_idx = 0;
            for (int i = 0; i < max_height; i++)
            {
                foreach(var tex in tex_list)
                {
                    var pixels = tex.GetPixels32();
                    Array.ConstrainedCopy(pixels, i * tex.width, res_pixels, res_idx, tex.width);
                    res_idx += tex.width;
                }
            }
            res.SetPixels32(res_pixels);
            _textureCache.Add(text, res);   
            return res;
        }

        static Texture2D ScaleTextureHeight(Texture2D tex, int height)
        {
            var res = new Texture2D(tex.width, height, TextureFormat.ARGB32, false);
            var pixels = tex.GetPixels32();
            var hDelta = height - tex.height;

            var add_raw = hDelta % 2;
            Color32[] prev_pixels = new Color32[(hDelta / 2 + add_raw) * tex.width];
            Color32[] next_pixels = new Color32[hDelta / 2 * tex.width];
            var summary_pixels = new Color32[prev_pixels.Length+ pixels.Length + next_pixels.Length];
            pixels.CopyTo(summary_pixels, prev_pixels.Length);
            res.SetPixels32(summary_pixels);
            return res;
        }
    }
}
