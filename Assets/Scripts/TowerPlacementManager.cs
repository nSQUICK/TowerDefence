using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("Настройки установки башен")]
    [Tooltip("Слой, содержащий игровой пол для установки башен (например, 'Ground')")]
    public LayerMask groundLayer;

    [Tooltip("Ссылка на компонент NavMeshSurface для обновления навмеш")]
    public NavMeshSurface navMeshSurface;

    // Выбранный префаб башни для установки (задается через UI)
    private GameObject selectedTowerPrefab;

    // Экземпляр превью выбранной башни
    private GameObject towerPreviewInstance;

    // Renderer'ы превью (для изменения цвета)
    private Renderer[] previewRenderers;

    [Header("Настройки цвета превью")]
    [Tooltip("Цвет, показывающий допустимое место установки (с прозрачностью)")]
    public Color validColor = new Color(0, 1, 0, 0.5f); // зеленый
    [Tooltip("Цвет, показывающий недопустимое место установки (с прозрачностью)")]
    public Color invalidColor = new Color(1, 0, 0, 0.5f); // красный

    // Кэш основной камеры
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Вызывается из TowerSelectionUI для выбора башни.
    /// Создается превью выбранной башни.
    /// </summary>
    /// <param name="towerPrefab">Префаб выбранной башни</param>
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

        // Помещаем превью на слой "Ignore Raycast", чтобы оно не мешало Raycast по игровому полю
        towerPreviewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Отключаем компоненты, влияющие на физику и боевую логику (превью должно быть чисто визуальным)
        DisablePreviewComponents(towerPreviewInstance);

        // Получаем рендереры для изменения цвета
        previewRenderers = towerPreviewInstance.GetComponentsInChildren<Renderer>();

        // Устанавливаем начальный цвет превью как допустимый
        SetPreviewColor(validColor);
    }

    /// <summary>
    /// Отключает компоненты (коллайдеры, NavMeshObstacle, скрипт Tower) у объекта-превью,
    /// чтобы он не влиял на навигацию и не выполнял боевую логику.
    /// </summary>
    /// <param name="preview">Объект-превью</param>
    private void DisablePreviewComponents(GameObject preview)
    {
        // Отключаем все коллайдеры
        Collider[] colliders = preview.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Отключаем скрипт Tower (если он есть)
        Tower towerComponent = preview.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.enabled = false;
        }

        // Отключаем NavMeshObstacle (если он есть)
        NavMeshObstacle obstacle = preview.GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }

    private void Update()
    {
        // Если башня не выбрана или превью не создано – ничего не делаем
        if (selectedTowerPrefab == null || towerPreviewInstance == null)
            return;

        // Проводим Raycast по игровому полю (groundLayer)
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // Дополнительная проверка: убеждаемся, что Raycast ударился по объекту с тегом "Ground"
            if (!hit.collider.CompareTag("Ground"))
            {
                return;
            }

            Vector3 placementPosition = hit.point;
            // Обновляем позицию превью
            towerPreviewInstance.transform.position = placementPosition;

            // Проверяем, можно ли установить башню в этой точке
            bool isValid = IsPlacementValid(placementPosition);
            SetPreviewColor(isValid ? validColor : invalidColor);

            // Если нажата левая кнопка мыши и установка допустима – подтверждаем установку
            if (Input.GetMouseButtonDown(0) && isValid)
            {
                ConfirmPlacement(placementPosition);
            }
        }
    }

    /// <summary>
    /// Устанавливает цвет для всех Renderer'ов объекта-превью.
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
    /// Выполняет две проверки:
    /// 1. Проверяет, нет ли уже установленных башен вблизи с помощью OverlapSphere по слою "Tower".
    /// 2. Проверяет, сохраняется ли проходимость пути для врагов (NavMesh).
    /// </summary>
    /// <param name="position">Позиция установки</param>
    /// <returns>true, если установка допустима; иначе false</returns>
    private bool IsPlacementValid(Vector3 position)
    {
        // 1. Проверка на наличие установленных башен
        float checkRadius = 0.5f;  // Подберите значение, соответствующее размерам башни

        // Получаем индекс слоя "Tower"
        int towerLayer = LayerMask.NameToLayer("Tower");
        if (towerLayer == -1)
        {
            Debug.LogWarning("Layer 'Tower' не найден! Создайте его в настройках проекта.");
            return false;
        }
        int towerLayerMask = 1 << towerLayer;
        Collider[] nearbyTowers = Physics.OverlapSphere(position, checkRadius, towerLayerMask);
        Debug.Log("Найдено " + nearbyTowers.Length + " объектов на слое Tower в радиусе " + checkRadius);
        if (nearbyTowers.Length > 0)
        {
            Debug.Log("Установка недопустима: рядом уже имеется башня.");
            return false;
        }

        // 2. Проверка NavMesh-проходимости
        // Создаем временный объект с NavMeshObstacle, чтобы смоделировать препятствие
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
        bool navMeshValid = pathFound && path.status == NavMeshPathStatus.PathComplete;

        Destroy(tempObstacle);

        // При необходимости обновляем NavMesh снова (чтобы вернуть исходное состояние)
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        return navMeshValid;
    }

    /// <summary>
    /// Подтверждает установку башни:
    /// – Создает реальный объект башни по выбранному префабу,
    /// – Вызывает метод TowerPlaced() в GameManager для списания ресурсов и уведомления,
    /// – Устанавливает тег и слой для обнаружения в будущем,
    /// – Удаляет объект-превью и сбрасывает выбор.
    /// </summary>
    /// <param name="position">Позиция установки башни</param>
    private void ConfirmPlacement(Vector3 position)
    {
        // Создаем реальный объект башни
        GameObject towerInstance = Instantiate(selectedTowerPrefab, position, Quaternion.identity);

        // Рекурсивно устанавливаем слой "Tower" и тег "Tower" для башни и всех её дочерних объектов
        SetLayerRecursively(towerInstance, LayerMask.NameToLayer("Tower"));
        SetTagRecursively(towerInstance, "Tower");

        // Вызываем метод GameManager для уведомления об установке башни и списания ресурсов
        Tower towerComponent = towerInstance.GetComponent<Tower>();
        if (towerComponent != null)
        {
            GameManager.Instance.TowerPlaced(towerComponent);
        }
        else
        {
            Debug.LogWarning("Выбранный префаб не содержит компонент Tower!");
        }

        // Удаляем объект-превью и сбрасываем выбор башни
        Destroy(towerPreviewInstance);
        towerPreviewInstance = null;
        selectedTowerPrefab = null;

        // Включаем UI выбора башен (например, через TowerSelectionUI)
        TowerSelectionUI[] allUI = Resources.FindObjectsOfTypeAll<TowerSelectionUI>();
        if (allUI.Length > 0)
        {
            TowerSelectionUI towerSelectionUI = allUI[0];
            towerSelectionUI.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Рекурсивно устанавливает заданный слой для объекта и всех его потомков.
    /// </summary>
    /// <param name="obj">Объект, для которого нужно установить слой</param>
    /// <param name="layer">Номер слоя</param>
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null)
            return;

        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    /// <summary>
    /// Рекурсивно устанавливает заданный тег для объекта и всех его потомков.
    /// </summary>
    /// <param name="obj">Объект, для которого нужно установить тег</param>
    /// <param name="tag">Тег</param>
    private void SetTagRecursively(GameObject obj, string tag)
    {
        if (obj == null)
            return;

        obj.tag = tag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }
}
