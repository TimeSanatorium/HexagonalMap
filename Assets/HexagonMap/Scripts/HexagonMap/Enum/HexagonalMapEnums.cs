#region ��ͼ�������
/// <summary>
/// ��ͼ���ӵ�����
/// </summary>
public enum TileType
{
    Empty,
    Road,//��·
    Wall//ǽ��
}
/// <summary>
/// ��ͼ���Ӹ��µ�״̬
/// </summary>
public enum PathfindingState
{
    Unupdated,//��û�и���
    InBatch,//�ڸ�������
    Updated,//�������
    UDIS//������Ӳ��ɴ�
}
/// <summary>
/// ��ͼ���ӵĳ���
/// </summary>
public enum HexagonTopType
{
    FlatTop,    // ƽ��
    PointyTop   // �ⶥ
}
#endregion

#region ���ͼ���
public enum MapType
{
    Square,
    Hexagon
}
#endregion
