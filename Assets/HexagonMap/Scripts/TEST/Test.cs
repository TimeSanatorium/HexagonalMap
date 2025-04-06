using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int width = 5; // 地图列数
    public int height = 5; // 地图行数
    public float radius = 1f; // 六边形半径（从中心到顶点的距离）

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float hexWidth = 2f * radius;
        float hexHeight = Mathf.Sqrt(3f) * radius;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // 计算六边形的世界坐标
                float x = col * hexWidth * 0.75f; // 0.75 让列错开
                float z = row * hexHeight;
                if (col % 2 == 1) z += hexHeight * 0.5f; // 偶数列下移半个格子

                DrawHexagon(new Vector3(x, 0, z), radius);
            }
        }
    }

    void DrawHexagon(Vector3 center, float radius)
    {
        Vector3[] hexCorners = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60 * i);
            hexCorners[i] = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }

        for (int i = 0; i < 6; i++)
        {
            Gizmos.DrawLine(hexCorners[i], hexCorners[(i + 1) % 6]);
        }
    }
}
