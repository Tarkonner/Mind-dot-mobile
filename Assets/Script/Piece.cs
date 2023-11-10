using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector2[] posArray;
    public Dot[] dotsArray;
    public Vector2 pivotPoint;

    public GameObject testPivotPoint;

    private LineRenderer[] connections;

    private bool rotatable=true;

    public bool testRotate;

    int testTimer = 0;
    public void Start()
    {
        Debug.Log("Entered Start for piece");
        testPivotPoint.transform.localPosition = pivotPoint;
        for (int i = 0; i<dotsArray.Length; i++)
        {
            Debug.Log("In Loop");
            dotsArray[i].gameObject.transform.position.Set(posArray[i].x, posArray[i].y, 
                dotsArray[i].gameObject.transform.position.z);
        }
    }
    public void Update()
    {
        testTimer++;
        if (testTimer >= 1000 && testRotate)
        {
            Debug.Log("In Update");
            Rotate();
            testTimer = 0;
        }
    }
    public void LoadPiece()
    {

    }

    public void Rotate()
    {
        if (!rotatable)
            return;

        for (int i = 0; i<posArray.Length; i++)
        {
            //Rotation math for vector rotation around a point. (x,y)?(x?,y?)=(?+(x??)cos??(y??)sin?,?+(x??)sin?+(y??)cos?)
            //(x,y) is the point that is to be rotated. (?,?) is the pivot point of the rotation.
            //Since we want a 90 degree rotation, the formula effectively becomes: (x,y)?(??(y??),?+(x??))
            posArray[i] = new Vector2((pivotPoint.x - (posArray[i].y - pivotPoint.y)),
                (pivotPoint.y + (posArray[i].x - pivotPoint.y)));

            dotsArray[i].gameObject.transform.localPosition = posArray[i];
        }
    }

    public void Place(Vector2Int coordinates)
    {

    }

    public void Lift()
    {

    }
}
