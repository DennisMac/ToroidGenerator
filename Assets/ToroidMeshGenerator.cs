using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ToroidMeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    LineRenderer lineRenderer;
    [SerializeField]
    Material material;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        CreateShape(new Vector3(0,0,1), 4f, 1f, new Vector3(2.5f,1,1), new Vector3(-1,3,0f));
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }
    /// <summary>
    /// Create a section of a toroid 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="R">Revolved radius</param>
    /// <param name="r">cross section of area revolved</param>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    private void CreateShape(Vector3 center, float R, float r, Vector3 start, Vector3 finish, Plane? _planeOfArc = null )
    {
        //If the three points do not define a plane, we need the plane identified (happens at 180 and 360)
        Plane planeOfArc = (Plane)((_planeOfArc == null) ? new Plane(center, start, finish):_planeOfArc);        

        Plane planeOfStartEndCap = new Plane(Vector3.Cross(start - center, planeOfArc.normal).normalized, start);
        Plane planeofFinishEndCap = new Plane(Vector3.Cross(finish - center, planeOfArc.normal).normalized, finish);

        R = (start - center).magnitude;
        transform.position = center;
        transform.rotation = Quaternion.LookRotation(planeOfArc.normal, (start-center));
        transform.Rotate(new Vector3(0, 0, 90f));
        int st = 15;                    //number of times we draw a ring
        int sl = 15;                    //number of subdivisions of the ring

        float phi = 0.0f;
        float dp = (2 * Mathf.PI) / sl; 
        float theta = 0.0f;

        float angleOfBend = 180f;
        angleOfBend = Vector3.Angle(finish - center, start - center);

        float dt = (angleOfBend * Mathf.PI/180f) / (st-1);

        List<Vector3> vertList = new List<Vector3>();
        for (int stack = 0; stack < st; stack++) //for stack = 0, st do
        {
            theta = dt * stack;
            for (int slice = 0; slice < sl; slice++)
            {
                phi = dp * slice;
                Vector3 v = new Vector3(Mathf.Cos(theta) * (R + Mathf.Cos(phi) * r), Mathf.Sin(theta) * (R + Mathf.Cos(phi) * r), Mathf.Sin(phi) * r);
                vertList.Add(v);
            }
        }

        List<int> triangleList = new List<int>();
        int i1 = 0;
        int i2 = 0;
        int i3 = 0;
        int i4 = 0;
        for (int stack = 0; stack < st-1; stack++) //for stack = 0, st do
        {
            for (int slice = 0; slice < sl-1; slice++)
            {
                if (stack == 0)
                {
                    i1 = st - 1 + (slice * sl);
                    i2 = (0) + (slice * sl);
                    i3 = st - 1 + ((slice + 1) * sl);
                    i4 = (0) + ((slice + 1) * sl);

                    triangleList.Add(i1);
                    triangleList.Add(i3);
                    triangleList.Add(i4);
                    triangleList.Add(i1);
                    triangleList.Add(i4);
                    triangleList.Add(i2);
                }

                i1 = stack + (slice * sl);
                i2 = (stack + 1) + (slice * sl);
                i3 = stack + ((slice + 1) * sl);
                i4 = (stack + 1) + ((slice + 1) * sl);
                triangleList.Add(i1);
                triangleList.Add(i3);
                triangleList.Add(i4);
                triangleList.Add(i1);
                triangleList.Add(i4);
                triangleList.Add(i2);

            }

            //i1 = i3;
            //i2 = i4;
            //i3 = stack;
            //i4 = stack + 1;
            //triangleList.Add(i1);
            //triangleList.Add(i3);
            //triangleList.Add(i4);
            //triangleList.Add(i1);
            //triangleList.Add(i4);
            //triangleList.Add(i2);

        }
        vertices = vertList.ToArray();
        triangles = triangleList.ToArray();
        lineRenderer.positionCount = vertList.Count;
        lineRenderer.SetPositions(vertList.ToArray());
    }
}
