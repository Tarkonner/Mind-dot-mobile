using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedData
{
    public class PieceData : GridData
    {
        public int startRotationIndex = 0;
        public bool canRotate = true;
        public TwoKeyDictionary<Vector2Int> connectionsMade = new TwoKeyDictionary<Vector2Int>();
    }

}
