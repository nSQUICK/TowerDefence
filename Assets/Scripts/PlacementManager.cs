using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlacementManager : MonoBehaviour
{
    [Header("Настройки установки башен")]
    [Tooltip("Префаб башни, выбранный для установки (задается через UI выбора)")]
    public GameObject selectedTowerPrefab;

    [Tooltip("Ссылка на NavMeshSurface игрового поля (например, Terrain)")]
    public NavMeshSurface navMeshSurface;

    [Tooltip("LayerMask, содержащий только слой, на котором находится игровой пол (например, 'Ground')")]
    public LayerMask groundLayer;

    [Header("Настройки превью башни")]
    [Tooltip("Цвет, указывающий, что место установки допустимо (с прозрачностью)")]
    public Color validColor = new Color(0, 1, 0, 0.5f);  // зеленый
    [Tooltip("Цвет, указывающий, что место установки недопустимо (с прозрачностью)")]
    public Color invalidColor = new Color(1, 0, 0, 0.5f);  // красный

    // Приватные переменные для работы превью
    private GameObject towerPreviewInstance;
    private Renderer[] previewRenderers;

    // Кэш основной камеры
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Если башня не выбрана или превью не создано, ничего не делаем
        if (selectedTowerPrefab == null || towerPreviewInstance == null)
            return;

        // Используем Raycast только по слою groundLayer, чтобы попадать только на игровой пол
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // Дополнительная проверка: убедимся, что Raycast ударился именно о объект с тегом "Ground"
            if (!hit.collider.CompareTag("Ground"))
            {
                // Если удар не по полю, не обновляем позицию превью
                return;
            }

            Vector3 placementPosition = hit.point;
            // Обновляем позицию превью
            towerPreviewInstance.transform.position = placementPosition;

            // Проверяем, можно ли установить башню в данной точке
            bool isValid = IsPlacementValid(placementPosition);
            SetPreviewColor(isValid ? validColor : invalidColor);

            // При клике (левая кнопка мыши) и если место установки допустимо – подтверждаем установку
            if (Input.GetMouseButtonDown(0) && isValid)
            {
                ConfirmPlacement(placementPosition);
            }
        }
    }

    /// <summary>
    /// Вызывается для установки выбранного префаба башни.
    /// Обычно этот метод вызывается из UI (например, через кнопку выбора башни).
    /// Создается превью выбранной башни.
    /// </summary>
    /// <param name="towerPrefab">Выбранный префаб башни</param>
    public void SetSelectedTower(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;

        // Если ранее уже было создано превью, удаляем его
        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
        }

        // Создаем экземпляр превью на основе выбранного префаба
        towerPreviewInstance = Instantiate(selectedTowerPrefab);

        // Помещаем превью на слой, который не участвует в Raycast (например, "Ignore Raycast")
        towerPreviewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Отключаем компоненты, влияющие на физику и боевую логику (чтобы превью не блокировало путь и не стреляло)
        DisablePreviewComponents(towerPreviewInstance);

        // Получаем рендереры для изменения цвета
        previewRenderers = towerPreviewInstance.GetComponentsInChildren<Renderer>();

        // Устанавливаем начальный цвет превью как допустимый
        SetPreviewColor(validColor);
    }

    /// <summary>
    /// Отключает нежелательные компоненты (коллайдеры, NavMeshObstacle, скрипт Tower) у объекта превью,
    /// чтобы он не влиял на игровой процесс.
    /// </summary>
    /// <param name="preview">Объект-превью</param>
    private void DisablePreviewComponents(GameObject preview)
    {
        // Отключаем все коллайдеры
        Collider[] colliders = preview.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Отключаем скрипт, отвечающий за боевую логику (если он есть)
        Tower towerComp = preview.GetComponent<Tower>();
        if (towerComp != null)
        {
            towerComp.enabled = false;
        }

        // Отключаем компонент NavMeshObstacle, чтобы он не влиял на NavMesh
        NavMeshObstacle obstacle = preview.GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }

    /// <summary>
    /// Устанавливает цвет для всех рендереров объекта превью.
    /// </summary>
    /// <param name="color">Желаемый цвет (с альфа-каналом)</param>
    private void SetPreviewColor(Color color)
    {
        if (previewRenderers != null)
        {
            foreach (Renderer rend in previewRenderers)
            {
                if (rend != null)
                {
                    // Создаем копию материала, чтобы не менять оригинальный материал префаба
                    Material previewMat = rend.material;
                    previewMat.color = color;
                }
            }
        }
    }

    /// <summary>
    /// Проверяет, можно ли установить башню в указанной точке.
    /// Проверки включают:
    /// 1. Отсутствие уже установленной башни в окрестности (по тегу "Tower").
    /// 2. Проверку NavMesh-проходимости (будет ли путь от точки спауна до базы).
    /// </summary>
    /// <param name="position">Позиция установки</param>
    /// <returns>true, если установка допустима; иначе false</returns>
    private bool IsPlacementValid(Vector3 position)
    {
        // 1. Проверяем, что рядом нет уже установленных башен.
        // Предполагается, что все установленные башни имеют тег "Tower".
        float checkRadius = 10.0f;

        // Получаем все коллайдеры в указанной области
        Collider[] nearbyColliders = Physics.OverlapSphere(position, checkRadius);
        foreach (Collider col in nearbyColliders)
        {
            Debug.Log("Найден объект: " + col.gameObject.name + " с тегом: " + col.gameObject.tag);
            // Если найден объект с тегом "Tower", установка недопустима
            if (col.gameObject.CompareTag("Tower"))
            {
                return false;
            }
        }

        // 2. Проверяем NavMesh-проходимость.
        // Создаем временный объект с NavMeshObstacle, чтобы смоделировать препятствие.
        GameObject tempObstacle = new GameObject("TempObstacle");
        NavMeshObstacle tempNavObstacle = tempObstacle.AddComponent<NavMeshObstacle>();
        tempNavObstacle.carving = true;

        // Если в выбранном префабе есть NavMeshObstacle, копируем его параметры
        NavMeshObstacle prefabObstacle = selectedTowerPrefab.GetComponent<NavMeshObstacle>();
        if (prefabObstacle != null)
        {
            tempNavObstacle.shape = prefabObstacle.shape;
            tempNavObstacle.size = prefabObstacle.size;
        }
        else
        {
            tempNavObstacle.shape = NavMeshObstacleShape.Box;
            tempNavObstacle.size = new Vector3(1, 1, 1);
        }
        tempObstacle.transform.position = position;

        // Обновляем NavMesh, чтобы временное препятствие было учтено
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        // Вычисляем путь от точки спауна до базы, используя NavMesh
        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(
            GameManager.Instance.enemySpawnPoint.position,
            GameManager.Instance.baseTransform.position,
            NavMesh.AllAreas,
            path
        );
        bool valid = pathFound && path.status == NavMeshPathStatus.PathComplete;

        Destroy(tempObstacle);

        // При необходимости обновляем NavMesh снова (чтобы вернуть исходное состояние)
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        return valid;
    }

    /// <summary>
    /// Подтверждает установку башни в указанной позиции.
    /// Создается реальный объект башни, вызывается метод GameManager для списания ресурсов и обновления UI,
    /// а объект-превью удаляется.
    /// </summary>
    /// <param name="position">Позиция установки башни</param>
    private void ConfirmPlacement(Vector3 position)
    {
        // Создаем реальный объект башни
        GameObject towerInstance = Instantiate(selectedTowerPrefab, position, Quaternion.identity);

        // Присваиваем установленной башне слой и тег, чтобы в будущем ее можно было обнаружить
        towerInstance.layer = LayerMask.NameToLayer("Tower");
        towerInstance.tag = "Tower";

        Tower towerComponent = towerInstance.GetComponent<Tower>();
        if (towerComponent != null)
        {
            GameManager.Instance.TowerPlaced(towerComponent);
        }
        else
        {
            Debug.LogWarning("Выбранный префаб не содержит компонент Tower!");
        }

        // Удаляем превью и сбрасываем выбор
        Destroy(towerPreviewInstance);
        towerPreviewInstance = null;
        selectedTowerPrefab = null;
    }
}
