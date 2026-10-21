using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCampaignTools.Utilities;

public static class Texture2DResizeExtension
{
    public static Texture2D ProperResize(this Texture2D texture2D, int width, int height)
    {
        var previousRenderTexture = RenderTexture.active;
        var rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

        try
        {
            Graphics.Blit(texture2D, rt);
            RenderTexture.active = rt;

            var newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            newTexture.Apply();

            return newTexture;
        }
        finally
        {
            RenderTexture.active = previousRenderTexture;
            RenderTexture.ReleaseTemporary(rt);
        }
    }

    public static Texture2D CPUResize(this Texture2D texture2D, int width, int height)
    {
        var pixels = texture2D.GetPixels32();
        var result = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            int sourceY = y * texture2D.height / height;

            for (int x = 0; x < width; x++)
            {
                int sourceX = x * texture2D.width / width;

                result[y * width + x] =
                    pixels[sourceY * texture2D.width + sourceX];
            }
        }

        var resized = new Texture2D(width, height, TextureFormat.RGBA32, false);
        resized.SetPixels32(result);
        resized.Apply();

        return resized;
    }

    public static Task<Color32[]> CPUResizeBackground(this Texture2D texture2D, int width, int height)
    {
        return Task.Run(() => ResizeTexture(texture2D.GetPixels32(), texture2D.width, texture2D.height, width, height));
    }

    private static Color32[] ResizeTexture(Color32[] pixels, int srcWidth, int srcHeight, int width, int height)
    {
        var result = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            int sourceY = y * srcHeight / height;

            for (int x = 0; x < width; x++)
            {
                int sourceX = x * srcWidth / width;

                result[y * width + x] = pixels[sourceY * srcWidth + sourceX];
            }
        }

        return result;
    }
}
