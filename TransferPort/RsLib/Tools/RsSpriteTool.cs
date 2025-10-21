using System;
using UnityEngine;

namespace RsLib
{
    public static class RsSpriteTool
    {
        public static Texture2D Base64ToTexture2D(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(bytes);
            return texture2D;
        }

        public static Sprite Base64ToSprite(string base64)
        {
            var texture2D = Base64ToTexture2D(base64);
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        }
    }
}