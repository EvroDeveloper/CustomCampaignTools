using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCampaignTools
{
    public static class Texture2DResizeExtension
    {
        public static Texture2D ProperResize(this Texture2D texture2D, int width, int height)
        {
            var rt = new RenderTexture(width, height, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            var newTexture = new Texture2D(width, height);
            newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            newTexture.Apply();
            UnityEngine.Object.Destroy(rt);
            return newTexture;
        }
    }
}
