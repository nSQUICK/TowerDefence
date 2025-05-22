// EnemyPathRenderer.cs
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathRenderer : MonoBehaviour
{
    // ������ �� LineRenderer
    private LineRenderer lineRenderer;

    [Header("��������� ���������� ����")]
    [Tooltip("�������� ���������� ���� � ��������")]
    public float updateInterval = 0.5f;
    private float timer;

    // ������ �� ����� ������ � ����, ������� �� GameManager
    private Transform enemySpawnPoint;
    private Transform baseTransform;

    private void Start()
    {
        // �������� LineRenderer � �������� �������
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer �� ������ �� ������� " + gameObject.name);
        }
        // �������� ����� ������ � ���� �� GameManager
        enemySpawnPoint = GameManager.Instance.enemySpawnPoint;
        baseTransform = GameManager.Instance.baseTransform;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            UpdatePath();
            timer = 0f;
        }
    }

    /// <summary>
    /// ��������� ���� �� ����� ������ �� ���� � ������� NavMesh
    /// � ��������� LineRenderer ��� ����������� �����.
    /// </summary>
    private void UpdatePath()
    {
        if (enemySpawnPoint == null || baseTransform == null)
            return;

        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(enemySpawnPoint.position, baseTransform.position, NavMesh.AllAreas, path);
        if (pathFound && path.status == NavMeshPathStatus.PathComplete)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
        else
        {
            // ���� ���� �� ������ � ������� �����
            lineRenderer.positionCount = 0;
        }
    }
}
