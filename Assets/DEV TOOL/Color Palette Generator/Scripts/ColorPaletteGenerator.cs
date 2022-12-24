using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColorPaletteGenerator : MonoBehaviour
{
    public Material[] materials;
    public ImageFormat format;

    private int dimension;
    private List<ColorInfo> colorInfo;

    public void Run()
    {
        colorInfo = new List<ColorInfo>();

        foreach (var mat in materials) 
            colorInfo.Add(new ColorInfo(mat.name, mat.color));

        int rootSize = Mathf.CeilToInt(Mathf.Sqrt((float)colorInfo.Count));
        dimension = rootSize;

        GenerateImage();
    }

    private void GenerateImage() 
    {
        string directoryAddress = Application.persistentDataPath + "/color_palettes";
        if (!Directory.Exists(directoryAddress)) Directory.CreateDirectory(directoryAddress);

        string savePath = directoryAddress + $"/color-pallette-{System.DateTime.UtcNow.ToFileTimeUtc()}.{format}";

        Texture2D tex2D = new Texture2D(dimension, dimension);
        tex2D.SetPixels(ToColorArray());
        byte[] image = TextureToByteArray(tex2D, format);
        File.WriteAllBytes(savePath, image);

        Debug.Log($"<color=cyan>Color palette created with {format} encoding.</color>");
    }

    private byte[] TextureToByteArray(Texture2D texture, ImageFormat format) 
    {
        return format switch
        {
            ImageFormat.JPG => texture.EncodeToJPG(),
            ImageFormat.TGA => texture.EncodeToTGA(),
            _ => texture.EncodeToPNG(),
        };
    }

    private Color[] ToColorArray() 
    {
        Color[] array = new Color[dimension * dimension];
        for (int i = 0; i < colorInfo.Count; i++)
        {
            array[i] = colorInfo[i].color;
        }
        return array;
    }

    [System.Serializable]
    public enum ImageFormat 
    { 
        PNG = 0, 
        TGA = 1,
        JPG = 2,
    }

    internal struct ColorInfo 
    {
        public string matName;
        public Color color;

        public ColorInfo(string matName, Color color) 
        {
            this.matName = matName;
            this.color = color;
        }
    }

}
