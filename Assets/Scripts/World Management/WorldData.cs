namespace Main.WorldManagement
{
    /// <summary>
    /// Holds data about the world at runtime.
    /// </summary>
    public class WorldData
    {
        public float minHeight;
        public float maxHeight;

        public WorldData() 
        {
            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
        }

       

    }
}
