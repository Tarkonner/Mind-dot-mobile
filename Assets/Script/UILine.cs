using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILine : Graphic
{
    public RectTransform startPoint;
    public RectTransform endPoint;

    public float lineWidth;

    private VertexHelper vh;
    public void Initialzie(RectTransform startPoint, RectTransform endPoint, float lineWidth, bool rotatebul = true)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.lineWidth = lineWidth;

        if(rotatebul)
            color = Color.black;
        else 
            color = Color.white;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        this.vh = vh;
        vh.Clear();

        AddQuad(vh, startPoint.anchoredPosition, endPoint.anchoredPosition);
    }
    private void AddQuad(VertexHelper vh, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 dir = (startPoint - endPoint).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        Vector2 v1 = (startPoint - perp * lineWidth);
        Vector2 v2 = (endPoint - perp * lineWidth);
        Vector2 v3 = (endPoint + perp * lineWidth);
        Vector2 v4 = (startPoint + perp * lineWidth);

        vh.AddUIVertexQuad(SetVbo(new[] { v1, v2, v3, v4 }, color));
    }
    public void UpdateLine()
    {
        SetVerticesDirty();
    }

    private UIVertex[] SetVbo(Vector2[] vertices, Color color)
    {
        UIVertex[] vbo = new UIVertex[4];
        for (int i = 0;i < vertices.Length;i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = vertices[i];
            vbo[i] = vert;
        }
        return vbo;
    }
}
