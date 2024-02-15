using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedData
{
    public class GridData 
    {
        public Dictionary<Vector2Int, DotData> dotDictionary = new Dictionary<Vector2Int, DotData>();
        public Vector2Int gridSize;
        public Vector2Int gridPosRef;
    }
}

