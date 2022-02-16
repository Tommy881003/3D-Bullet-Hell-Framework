using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BulletHell3D
{
    [CustomEditor(typeof(BHPattern))]
    public class BHPatternEditor : Editor
    {
        private enum DrawOption
        {
            None = 0,
            Dot = 1,
            TwoDimension = 2,
            ThreeDimension = 3
        }

        private enum TwoDimensionOption
        {
            Line = 0,
            Circle = 1,
            Polygon = 2
        }

        BHPattern inspecting = null;

        //Draw options.
        DrawOption drawOption;
        TwoDimensionOption twoDimensionOption;

        // General data of the drawing pattern. (For dot, 2D, and 3D)
        // Position.
        Vector3 drawOrigin = Vector3.zero;
        // Rotation. (In axis-angle form.)
        float theta = 90;
        float phi = 0;
        float angle = 0;
        Vector3 normalAxis 
        { 
            get 
            { 
                float thetaRad = theta * Mathf.Deg2Rad;
                float phiRad = phi * Mathf.Deg2Rad;
                return  new Vector3(Mathf.Cos(phiRad) * Mathf.Cos(thetaRad), Mathf.Sin(phiRad), Mathf.Cos(phiRad) * Mathf.Sin(thetaRad));
            } 
        }
        Quaternion lookRotation
        { 
            get 
            {
                Vector3 relativeUp;
                Vector3 relativeRight;

                BHHelper.LookRotationSolver(normalAxis, angle, out relativeUp, out relativeRight);

                Debug.DrawLine(drawOrigin, drawOrigin + relativeRight * 100, Color.red, 0.05f);
                Debug.DrawLine(drawOrigin, drawOrigin + relativeUp * 100, Color.green, 0.05f);
                Debug.DrawLine(drawOrigin, drawOrigin + normalAxis * 100, Color.blue, 0.05f);

                return Quaternion.LookRotation
                (
                    normalAxis,
                    relativeUp
                );
            } 
        }
        // Scale.
        Vector3 scale = Vector3.one;

        // View scaler.
        float viewScale = 1;

        // Data for 2D drawing pattern.
        float arc = 360;                // Arc of the circle.
        int bulletCountForCircle;       // Bullet count of the circle.
        int edgeCount;                  // Edge count of the polygon.
        int bulletCountForPolygonEdge;  // Bullet count of the edge of the polygon.
        Vector3 pointA;                 // Starting point of a line.
        Vector3 pointB;                 // End point of a line.
        int bulletCountForLine;         // Bullet count of the line.

        // Data for 3D drawing pattern.
        Mesh mesh;

        List<Vector3> previewPoints = new List<Vector3>();

        private void OnEnable() 
        {
            inspecting = (BHPattern)target;
            SceneView.duringSceneGui += OnSceneGUI;    
        }

        private void OnDisable()
        {
            inspecting = null;
            previewPoints.Clear();
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(10);
            serializedObject.Update();
            // Some notes:
            // The pattern itself is suggested to be constrained within a cube. (width = 2, centered in origin)
            // The pattern should be as big as possible without violating other constrains.

            // Support functions:
            // 1. Plot a single dot.
            // 2. Plot a 2D figure (line, polygon, circle, etc.) on a given plane.
            // 3. Plot a mesh. (For future me: this include the spherical fibonacci lattice.)

            //Draw options.
            viewScale = EditorGUILayout.Slider("檢視放大倍率：",viewScale,1,50);
            drawOption = (DrawOption)EditorGUILayout.EnumPopup("繪製樣式：", drawOption);
            if(drawOption == DrawOption.TwoDimension)
            {
                twoDimensionOption = (TwoDimensionOption)EditorGUILayout.EnumPopup("2D樣式：", twoDimensionOption);
                if(twoDimensionOption == TwoDimensionOption.Line)
                {
                    pointA = EditorGUILayout.Vector3Field("起點：", pointA);
                    pointB = EditorGUILayout.Vector3Field("終點：", pointB);
                }
                if(twoDimensionOption == TwoDimensionOption.Circle)
                {
                    arc = EditorGUILayout.Slider("圓周弧角：", arc, 0f, 360f);
                    bulletCountForCircle = EditorGUILayout.IntSlider("圓周子彈數：", bulletCountForCircle, 1, 100);
                }
                if(twoDimensionOption == TwoDimensionOption.Polygon)
                {
                    edgeCount = EditorGUILayout.IntSlider("多邊形邊數：", edgeCount, 3, 10);
                    bulletCountForPolygonEdge = EditorGUILayout.IntSlider("多邊形子彈數：", bulletCountForPolygonEdge, 1, 25);
                }
            }
            if(drawOption == DrawOption.ThreeDimension)
            {
                mesh = EditorGUILayout.ObjectField("模型：", mesh, typeof(Mesh), false) as Mesh;
            }

            // Transform of the pattern.
            EditorGUILayout.Space(10);
            drawOrigin = EditorGUILayout.Vector3Field("位置：", drawOrigin);
            theta = EditorGUILayout.Slider("方位角：", theta, 0f, 360f);
            phi = EditorGUILayout.Slider("極角：", phi, -90f, 90f);
            angle = EditorGUILayout.Slider("軸角：", angle, 0f, 360f);
            scale = EditorGUILayout.Vector3Field("縮放：", scale);

            // Buttons for operations.
            EditorGUILayout.Space(10);
            if(drawOption != DrawOption.None && inspecting.renderObject != null)
            {
                if(GUILayout.Button("生成彈幕"))
                {
                    ConstructPoints();
                    AddPattern();
                }
            }
            if(GUILayout.Button("清除彈幕"))
                inspecting.Clear();
            
            //Show bullet count.
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("子彈總數：" + inspecting.bullets.Count.ToString(), MessageType.Info);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        int sceneTick = 0;

        public void OnSceneGUI(SceneView scene)
        {
            if(sceneTick % 10 == 0)
                ConstructPoints();
            sceneTick = (sceneTick + 1) % 10000;
            DrawBound();
            DrawPreview(scene);
            DrawPattern(scene);
            SceneView.RepaintAll();
        }

        public void ConstructPoints()
        {
            previewPoints.Clear();
            Matrix4x4 TRS = Matrix4x4.TRS(drawOrigin, lookRotation, scale);
            if(drawOption == DrawOption.Dot)
                previewPoints.Add(Vector3.zero);
            if(drawOption == DrawOption.TwoDimension)
            {
                //To formalize, when drawing on a 2D plane we use (0,0,1) as the default normal vector.
                //That is, we draw on the xy plane. 
                if(twoDimensionOption == TwoDimensionOption.Line)
                {
                    for(int i = 0; i < bulletCountForLine; i++)
                        previewPoints.Add(Vector3.Lerp(pointA, pointB, i / (float)(bulletCountForLine - 1)));
                }
                if(twoDimensionOption == TwoDimensionOption.Polygon)
                {
                    float angle = 0;
                    float delta = 360f / edgeCount;
                    Vector3 start;
                    Vector3 end;

                    for (int i = 0; i < edgeCount; i++)
                    {
                        start = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                        end = new Vector3(Mathf.Cos((angle + delta) * Mathf.Deg2Rad), Mathf.Sin((angle + delta) * Mathf.Deg2Rad), 0);
                        for (int j = 0; j < bulletCountForPolygonEdge; j++)
                            previewPoints.Add(Vector3.Lerp(start, end, (float)j / bulletCountForPolygonEdge));
                        angle += delta;
                    }
                }
                if(twoDimensionOption == TwoDimensionOption.Circle)
                {
                    float angle = 0;
                    float delta = (arc != 360f) ? arc / (bulletCountForCircle - 1) : arc / bulletCountForCircle;

                    for (int i = 0; i < bulletCountForCircle; i++)
                    {
                        previewPoints.Add(new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0));
                        angle += delta;
                    }
                }
            }
            if(drawOption == DrawOption.ThreeDimension && mesh != null)
            {
                Dictionary<Vector3,bool> dict = new Dictionary<Vector3,bool>();
                for(int i = 0; i < mesh.vertices.Length; i ++)
                {
                    if(!dict.ContainsKey(mesh.vertices[i]))
                    {
                        dict.Add(mesh.vertices[i], false);
                        previewPoints.Add(mesh.vertices[i]);
                    }
                }
            }
            if(!(drawOption == DrawOption.TwoDimension && twoDimensionOption == TwoDimensionOption.Line))
            {
                for(int i = 0; i < previewPoints.Count; i++)
                    previewPoints[i] = TRS.MultiplyPoint3x4(previewPoints[i]);
            }   
        }

        public void AddPattern()
        {
            inspecting.AddBullets(previewPoints);
        }

        public void DrawPreview(SceneView scene)
        {
            Vector3 camPos = scene.camera.transform.position;
            Handles.color = new Color(0,1,1,0.1f);

            foreach(Vector3 pos in previewPoints)
            {
                Vector3 normal = camPos - pos; 
                Handles.DrawSolidDisc(pos * viewScale, normal, inspecting.renderObject.radius);
            }
        }

        public void DrawPattern(SceneView scene)
        {
            Vector3 camPos = scene.camera.transform.position;
            Handles.color = new Color(1,1,0,0.1f);

            foreach(Vector3 pos in inspecting.bullets)
            {
                Vector3 normal = camPos - pos; 
                Handles.DrawSolidDisc(pos * viewScale, normal, inspecting.renderObject.radius);
            }
        }

        public void DrawBound()
        {
            Handles.color = new Color(1,1,1,0.5f);
            float dashSize = 0.25f * viewScale;
            Handles.DrawDottedLine(new Vector3(-1,-1,-1) * viewScale, new Vector3(1,-1,-1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(-1,-1,-1) * viewScale, new Vector3(-1,1,-1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(-1,-1,-1) * viewScale, new Vector3(-1,-1,1) * viewScale, dashSize);
            
            Handles.DrawDottedLine(new Vector3(-1,1,1) * viewScale, new Vector3(-1,-1,1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(-1,1,1) * viewScale, new Vector3(-1,1,-1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(-1,1,1) * viewScale, new Vector3(1,1,1) * viewScale, dashSize);
            
            Handles.DrawDottedLine(new Vector3(1,-1,1) * viewScale, new Vector3(-1,-1,1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(1,-1,1) * viewScale, new Vector3(1,1,1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(1,-1,1) * viewScale, new Vector3(1,-1,-1) * viewScale, dashSize);
            
            Handles.DrawDottedLine(new Vector3(1,1,-1) * viewScale, new Vector3(-1,1,-1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(1,1,-1) * viewScale, new Vector3(1,-1,-1) * viewScale, dashSize);
            Handles.DrawDottedLine(new Vector3(1,1,-1) * viewScale, new Vector3(1,1,1) * viewScale, dashSize);
        }
    }
}