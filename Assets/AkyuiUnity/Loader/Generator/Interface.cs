using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace AkyuiUnity.Generator
{
    public interface IAssetLoader
    {
        Sprite LoadSprite(string name);
        RenderTexture LoadRenderTexture(string name);
        VideoClip LoadVideoClip(string name);
        Font LoadFont(string name);
        Dictionary<string, object> LoadMeta(string name);
    }
}