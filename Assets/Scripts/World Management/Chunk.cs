using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class contains all the data of a chunk.
    /// </summary>
    public class Chunk
    {
        public GameObject chunkGameObject;

        public void SetVisible(bool visible) 
        {
            chunkGameObject.SetActive(visible);
        }
    }

}