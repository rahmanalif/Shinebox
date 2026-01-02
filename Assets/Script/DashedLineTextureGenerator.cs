using UnityEngine;

public static class DashedLineTextureGenerator
{
    public static Texture2D CreateDashedTexture(int width, int height, float dashRatio = 0.5f)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Repeat;

        Color[] pixels = new Color[width * height];
        int dashWidth = Mathf.RoundToInt(width * dashRatio);

        for (int x = 0; x < width; x++)
        {
            Color color = x < dashWidth ? Color.white : Color.clear;
            for (int y = 0; y < height; y++)
            {
                pixels[x + y * width] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
