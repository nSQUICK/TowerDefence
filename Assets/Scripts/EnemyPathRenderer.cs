// EnemyPathRenderer.cs
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathRenderer : MonoBehaviour
{
    // Ссылка на LineRenderer
    private LineRenderer lineRenderer;

    [Header("Настройки обновления пути")]
    [Tooltip("Интервал обновления пути в секундах")]
    public float updateInterval = 0.5f;
    private float timer;

    // Ссылки на точки спауна и базу, берутся из GameManager
    private Transform enemySpawnPoint;
    private Transform baseTransform;

    private void Start()
    {
        // Получаем LineRenderer с текущего объекта
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer не найден на объекте " + gameObject.name);
        }
        // Получаем точки спауна и базы из GameManager
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
    /// Вычисляет путь от точки спауна до базы с помощью NavMesh
    /// и обновляет LineRenderer для отображения линии.
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
            // Если путь не найден – очищаем линию
            lineRenderer.positionCount = 0;
        }
    }
}
