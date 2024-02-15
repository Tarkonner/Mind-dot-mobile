using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedData
{
    public class CellData
    {
        //Grid
        public Vector2Int gridCoordinates;  //Position
        public bool turnedOff;              //Active cell
        public DotData holding;             //What Dot it holds

        public bool partOfPiece;
    }

}
