using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector2[] gridPosArray;
    public Dot[] dotsArray;
    public Vector2 pivotPoint;

    public GameObject testPivotPoint;

    public float dotSpacing;

    private List<LineRenderer> connections;

    private bool rotatable = true;

    public bool testRotate;

    int testTimer = 0;
    public void Start()
    {
        LoadPiece();
        testPivotPoint.transform.localPosition = pivotPoint;
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
        //Goes through each dot and measures grid distance to each other dot.
        // Distance is used to differentiate adjacent and diagonal dot connections. 
        for (int i = 0; i < dotsArray.Length; i++)
        {
            //
            dotsArray[i].gameObject.transform.localPosition = gridPosArray[i];

            if (!dotsArray[i].IsConnected)
            {
                List<int> diagonalList = new List<int>();
                bool foundAdjacent = false;
                for (int j = 0; j < dotsArray.Length; j++)
                {
                    if (i == j) continue;

                    float val = Vector2.Distance(gridPosArray[i], gridPosArray[j]);
                    if (val > 1.3f) continue;

                    else if (val < 1.1f && !foundAdjacent)
                    {
                        diagonalList.Add(j);

                    }
                    else if (val == 1)
                    {
                        foundAdjacent = true;
                        CreateConnection(dotsArray[i].gameObject, gridPosArray[i], gridPosArray[j]);
                        dotsArray[i].IsConnected = true;
                        dotsArray[j].IsConnected = true;
                    }
                }
                if (!foundAdjacent)
                {
                    foreach (int j in diagonalList)
                    {
                        CreateConnection(dotsArray[i].gameObject, gridPosArray[i], gridPosArray[j]);
                        dotsArray[i].IsConnected = true;
                        dotsArray[j].IsConnected = true;
                    }
                }
            }

        }

    }
    public void CreateConnection(GameObject gameObject, Vector2 start, Vector2 end)
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        connections.Add(lineRenderer);
    }
    public void UpdateConnections()
    {

    }

    public void Rotate()
    {
        if (!rotatable)
            return;

        for (int i = 0; i < gridPosArray.Length; i++)
        {
            //Rotation math for vector rotation around a point. (x,y)->(x',y')=(a+(x-a)cos(t)-(y-?)sin(t),b+(x-a)sin(t)+(y-b)cos(t))
            //(x,y) is the point that is to be rotated. (a,b) is the pivot point of the rotation.
            //Since we want a 90 degree rotation, the formula effectively becomes: (x,y)?(??(y??),?+(x??))
            gridPosArray[i] = new Vector2((pivotPoint.x - (gridPosArray[i].y - pivotPoint.y)),
                (pivotPoint.y + (gridPosArray[i].x - pivotPoint.y)));

            dotsArray[i].gameObject.transform.localPosition = gridPosArray[i];
        }
    }

    public void Place(Vector2 coordinates)
    {

    }

    public void Lift()
    {
        //Consider making a mathematical circle around the center of the piece. Lifting within the circle simply uses the lift location.
        //Lifting outside the circle snaps into the circle's edge.
    }
}
