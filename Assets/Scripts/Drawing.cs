using UnityEngine;
using System.Collections;


 public class Drawing : MonoBehaviour
 { 
    static Texture2D _aaLineTex = null;
 
    static Texture2D _lineTex = null;
 
    static Texture2D aaLineTex
    {
        get
        {
            if (!_aaLineTex)
            {
                _aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
                _aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
                _aaLineTex.SetPixel(0, 1, Color.white);
                _aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
                _aaLineTex.Apply();
            }
            return _aaLineTex;
        }
    }
 
    static Texture2D lineTex
    {
        get
        {
            if (!_lineTex)
            {
                _lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
                _lineTex.SetPixel(0, 1, Color.white);
                _lineTex.Apply();
            }
            return _lineTex;
        }
        set
        {
            _lineTex = value;
        }
    }
     
     public static void DrawLine(Camera cam, Vector3 pointA, Vector3 pointB) { DrawLine(cam,pointA,pointB, GUI.contentColor, 1.0f); }
     public static void DrawLine(Camera cam, Vector3 pointA, Vector3 pointB, Color color) { DrawLine(cam,pointA,pointB, color, 1.0f); }
     public static void DrawLine(Camera cam, Vector3 pointA, Vector3 pointB, float width) { DrawLine(cam,pointA,pointB, GUI.contentColor, width); }
     public static void DrawLine(Camera cam, Vector3 pointA, Vector3 pointB, Color color, float width){
         Vector2 p1 = Vector2.zero;
         p1.x = cam.WorldToScreenPoint(pointA).x;
         p1.y = cam.pixelHeight - cam.WorldToScreenPoint(pointA).y;
         Vector2 p2 = Vector2.zero;
         p2.x = cam.WorldToScreenPoint(pointB).x;
         p2.y = cam.pixelHeight - cam.WorldToScreenPoint(pointB).y;
         DrawLine(p1,p2,color,width);
     }
     
     public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
     public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
     public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
     public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
     public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
     public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
     public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
     public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
     {
         pointA.x = (int)pointA.x;    pointA.y = (int)pointA.y;
         pointB.x = (int)pointB.x;    pointB.y = (int)pointB.y;

         Color savedColor = GUI.color;
         GUI.color = color;

         Matrix4x4 matrixBackup = GUI.matrix;

         float angle = Mathf.Atan2(pointB.y-pointA.y, pointB.x-pointA.x)*180f/Mathf.PI;
         float length = (pointA-pointB).magnitude;
         GUIUtility.RotateAroundPivot(angle, pointA);
         GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), lineTex);
         
         GUI.matrix = matrixBackup;
         GUI.color = savedColor;
     }
     
     public static void DrawCircle (Vector2 center, float radius, Color color, float lineWidth, Vector2 scale)
     {
         DrawPolygonOutline (center, radius, 40, color, lineWidth, 0f, scale);
     }
    
    public static void DrawPolygonOutline (Vector2 center, float radius, uint sides, Color color, float lineWidth, float startAngle, Vector2 scale)
    {
        DrawPolygonArc (center, radius, sides, sides, color, lineWidth, startAngle, scale);
    }

    public static void DrawPolygonArc (Vector2 center, float radius, uint sidesTotal, uint sidesToDraw, Color color, float lineWidth, float startAngle, Vector2 scale)
    {
        float angle = startAngle;
        float endAngle = startAngle + (Mathf.PI * 2) * (float) sidesToDraw / (float) sidesTotal;
        float increment = (Mathf.PI * 2) / sidesTotal;
        while (angle < endAngle)
        {
            Vector2 start = new Vector3(center.x + (Mathf.Cos (angle) * radius * scale.x), center.y + (Mathf.Sin (angle) * radius * scale.y));
            angle += increment;
            Vector2 end   = new Vector3(center.x + (Mathf.Cos (angle) * radius * scale.x), center.y + (Mathf.Sin (angle) * radius * scale.y));

            DrawLine (start, end, color, lineWidth);
        }
    }
    
    
     
     public static void DrawCircle (Camera cam, Vector3 center, float radius, Color color, float lineWidth, Vector2 scale)
     {
         DrawPolygonOutline (cam, center, radius, 40, color, lineWidth, 0f, scale);
     }
    
    public static void DrawPolygonOutline (Camera cam, Vector3 center, float radius, uint sides, Color color, float lineWidth, float startAngle, Vector2 scale)
    {
        DrawPolygonArc (cam, center, radius, sides, sides, color, lineWidth, startAngle, scale);
    }

    public static void DrawPolygonArc (Camera cam, Vector3 center, float radius, uint sidesTotal, uint sidesToDraw, Color color, float lineWidth, float startAngle, Vector2 scale)
    {
        float angle = startAngle;
        float endAngle = startAngle + (Mathf.PI * 2) * (float) sidesToDraw / (float) sidesTotal;
        float increment = (Mathf.PI * 2) / sidesTotal;
        while (angle < endAngle)
        {
            Vector3 start = new Vector3(center.x + (Mathf.Cos (angle) * radius * scale.x), center.y + (Mathf.Sin (angle) * radius * scale.y), center.z);
            angle += increment;
            Vector3 end   = new Vector3(center.x + (Mathf.Cos (angle) * radius * scale.x), center.y + (Mathf.Sin (angle) * radius * scale.y), center.z);

            DrawLine (cam, start, end, color, lineWidth);
        }
    }
 }
