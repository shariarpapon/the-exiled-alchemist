using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColorPaletteGenerator : MonoBehaviour
{
    public Material[] materials;

    [Space]
    public ImageFormat format;
    public string fileName = string.Empty;

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
        string dir = GetDirectory("color_palettes");
        long timedID = System.DateTime.UtcNow.ToFileTime();

        if (string.IsNullOrEmpty(fileName))
            fileName = $"palette";

        string savePath = dir + $"/{fileName}_{timedID}.{format}";

        Texture2D tex2D = new Texture2D(dimension, dimension);
        tex2D.SetPixels(ToColorArray());
        byte[] image = TextureToByteArray(tex2D, format);
        File.WriteAllBytes(savePath, image);

        Debug.Log($"<color=cyan>Color palette created with {format} encoding.</color>");

        GenerateDataFile(timedID);
    }

    private void GenerateDataFile(long timedID) 
    {
        string dir = GetDirectory("color_palette_data");
        string data = "<2D Index (x, y)> ----- <Material Name>\n\n";
        for (int i = 0; i < colorInfo.Count; i++)
            data += $"[{i / dimension}, {i % dimension}] ----- {colorInfo[i].matName}\n";
        File.WriteAllText(dir + $"/palette-data_{timedID}.txt", data);
        Debug.Log($"<color=cyan>Color palette data generated.</color>");
    }

    private string GetDirectory(string dirName) 
    {
        string directoryAddress = Application.persistentDataPath + $"/{dirName}";
        if (!Directory.Exists(directoryAddress)) Directory.CreateDirectory(directoryAddress);
        return directoryAddress;
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
