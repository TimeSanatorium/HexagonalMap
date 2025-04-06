using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int width = 5; // ��ͼ����
    public int height = 5; // ��ͼ����
    public float radius = 1f; // �����ΰ뾶�������ĵ�����ľ��룩

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float hexWidth = 2f * radius;
        float hexHeight = Mathf.Sqrt(3f) * radius;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // ���������ε���������
                float x = col * hexWidth * 0.75f; // 0.75 ���д�
                float z = row * hexHeight;
                if (col % 2 == 1) z += hexHeight * 0.5f; // ż�������ư������

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
