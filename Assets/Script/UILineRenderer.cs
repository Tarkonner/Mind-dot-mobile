using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public List<Vector2> points;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

    }
    void DrawVerticiesForPoint(VertexHelper vh,Vector2 point)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

    }
}
