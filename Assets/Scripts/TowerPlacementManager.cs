using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("��������� ��������� �����")]
    [Tooltip("����, ���������� ������� ��� ��� ��������� ����� (��������, 'Ground')")]
    public LayerMask groundLayer;

    [Tooltip("������ �� ��������� NavMeshSurface ��� ���������� ������")]
    public NavMeshSurface navMeshSurface;

    // ��������� ������ ����� ��� ��������� (�������� ����� UI)
    private GameObject selectedTowerPrefab;

    // ��������� ������ ��������� �����
    private GameObject towerPreviewInstance;

    // Renderer'� ������ (��� ��������� �����)
    private Renderer[] previewRenderers;

    [Header("��������� ����� ������")]
    [Tooltip("����, ������������ ���������� ����� ��������� (� �������������)")]
    public Color validColor = new Color(0, 1, 0, 0.5f); // �������
    [Tooltip("����, ������������ ������������ ����� ��������� (� �������������)")]
    public Color invalidColor = new Color(1, 0, 0, 0.5f); // �������

    // ��� �������� ������
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// ���������� �� TowerSelectionUI ��� ������ �����.
    /// ��������� ������ ��������� �����.
    /// </summary>
    /// <param name="towerPrefab">������ ��������� �����</param>
    public void SetSelectedTower(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;

        // ���� ����� ��� ���� ������� ������, ������� ���
        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
        }

        // ������� ��������� ������ �� ������ ���������� �������
        towerPreviewInstance = Instantiate(selectedTowerPrefab);

        // �������� ������ �� ���� "Ignore Raycast", ����� ��� �� ������ Raycast �� �������� ����
        towerPreviewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");

        // ��������� ����������, �������� �� ������ � ������ ������ (������ ������ ���� ����� ����������)
        DisablePreviewComponents(towerPreviewInstance);

        // �������� ��������� ��� ��������� �����
        previewRenderers = towerPreviewInstance.GetComponentsInChildren<Renderer>();

        // ������������� ��������� ���� ������ ��� ����������
        SetPreviewColor(validColor);
    }

    /// <summary>
    /// ��������� ���������� (����������, NavMeshObstacle, ������ Tower) � �������-������,
    /// ����� �� �� ����� �� ��������� � �� �������� ������ ������.
    /// </summary>
    /// <param name="preview">������-������</param>
    private void DisablePreviewComponents(GameObject preview)
    {
        // ��������� ��� ����������
        Collider[] colliders = preview.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // ��������� ������ Tower (���� �� ����)
        Tower towerComponent = preview.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.enabled = false;
        }

        // ��������� NavMeshObstacle (���� �� ����)
        NavMeshObstacle obstacle = preview.GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }

    private void Update()
    {
        // ���� ����� �� ������� ��� ������ �� ������� � ������ �� ������
        if (selectedTowerPrefab == null || towerPreviewInstance == null)
            return;

        // �������� Raycast �� �������� ���� (groundLayer)
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // �������������� ��������: ����������, ��� Raycast �������� �� ������� � ����� "Ground"
            if (!hit.collider.CompareTag("Ground"))
            {
                return;
            }

            Vector3 placementPosition = hit.point;
            // ��������� ������� ������
            towerPreviewInstance.transform.position = placementPosition;

            // ���������, ����� �� ���������� ����� � ���� �����
            bool isValid = IsPlacementValid(placementPosition);
            SetPreviewColor(isValid ? validColor : invalidColor);

            // ���� ������ ����� ������ ���� � ��������� ��������� � ������������ ���������
            if (Input.GetMouseButtonDown(0) && isValid)
            {
                ConfirmPlacement(placementPosition);
            }
        }
    }

    /// <summary>
    /// ������������� ���� ��� ���� Renderer'�� �������-������.
    /// </summary>
    /// <param name="color">�������� ���� (� �����-�������)</param>
    private void SetPreviewColor(Color color)
    {
        if (previewRenderers != null)
        {
            foreach (Renderer rend in previewRenderers)
            {
                if (rend != null)
                {
                    // ������� ����� ���������, ����� �� ������ ������������ �������� �������
                    Material previewMat = rend.material;
                    previewMat.color = color;
                }
            }
        }
    }

    /// <summary>
    /// ���������, ����� �� ���������� ����� � ��������� �����.
    /// ��������� ��� ��������:
    /// 1. ���������, ��� �� ��� ������������� ����� ������ � ������� OverlapSphere �� ���� "Tower".
    /// 2. ���������, ����������� �� ������������ ���� ��� ������ (NavMesh).
    /// </summary>
    /// <param name="position">������� ���������</param>
    /// <returns>true, ���� ��������� ���������; ����� false</returns>
    private bool IsPlacementValid(Vector3 position)
    {
        // 1. �������� �� ������� ������������� �����
        float checkRadius = 0.5f;  // ��������� ��������, ��������������� �������� �����

        // �������� ������ ���� "Tower"
        int towerLayer = LayerMask.NameToLayer("Tower");
        if (towerLayer == -1)
        {
            Debug.LogWarning("Layer 'Tower' �� ������! �������� ��� � ���������� �������.");
            return false;
        }
        int towerLayerMask = 1 << towerLayer;
        Collider[] nearbyTowers = Physics.OverlapSphere(position, checkRadius, towerLayerMask);
        Debug.Log("������� " + nearbyTowers.Length + " �������� �� ���� Tower � ������� " + checkRadius);
        if (nearbyTowers.Length > 0)
        {
            Debug.Log("��������� �����������: ����� ��� ������� �����.");
            return false;
        }

        // 2. �������� NavMesh-������������
        // ������� ��������� ������ � NavMeshObstacle, ����� ������������� �����������
        GameObject tempObstacle = new GameObject("TempObstacle");
        NavMeshObstacle tempNavObstacle = tempObstacle.AddComponent<NavMeshObstacle>();
        tempNavObstacle.carving = true;

        // ���� � ��������� ������� ���� NavMeshObstacle, �������� ��� ���������
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

        // ��������� NavMesh, ����� ��������� ����������� ���� ������
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        // ��������� ���� �� ����� ������ �� ����, ��������� NavMesh
        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(
            GameManager.Instance.enemySpawnPoint.position,
            GameManager.Instance.baseTransform.position,
            NavMesh.AllAreas,
            path
        );
        bool navMeshValid = pathFound && path.status == NavMeshPathStatus.PathComplete;

        Destroy(tempObstacle);

        // ��� ������������� ��������� NavMesh ����� (����� ������� �������� ���������)
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        return navMeshValid;
    }

    /// <summary>
    /// ������������ ��������� �����:
    /// � ������� �������� ������ ����� �� ���������� �������,
    /// � �������� ����� TowerPlaced() � GameManager ��� �������� �������� � �����������,
    /// � ������������� ��� � ���� ��� ����������� � �������,
    /// � ������� ������-������ � ���������� �����.
    /// </summary>
    /// <param name="position">������� ��������� �����</param>
    private void ConfirmPlacement(Vector3 position)
    {
        // ������� �������� ������ �����
        GameObject towerInstance = Instantiate(selectedTowerPrefab, position, Quaternion.identity);

        // ���������� ������������� ���� "Tower" � ��� "Tower" ��� ����� � ���� � �������� ��������
        SetLayerRecursively(towerInstance, LayerMask.NameToLayer("Tower"));
        SetTagRecursively(towerInstance, "Tower");

        // �������� ����� GameManager ��� ����������� �� ��������� ����� � �������� ��������
        Tower towerComponent = towerInstance.GetComponent<Tower>();
        if (towerComponent != null)
        {
            GameManager.Instance.TowerPlaced(towerComponent);
        }
        else
        {
            Debug.LogWarning("��������� ������ �� �������� ��������� Tower!");
        }

        // ������� ������-������ � ���������� ����� �����
        Destroy(towerPreviewInstance);
        towerPreviewInstance = null;
        selectedTowerPrefab = null;

        // �������� UI ������ ����� (��������, ����� TowerSelectionUI)
        TowerSelectionUI[] allUI = Resources.FindObjectsOfTypeAll<TowerSelectionUI>();
        if (allUI.Length > 0)
        {
            TowerSelectionUI towerSelectionUI = allUI[0];
            towerSelectionUI.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ���������� ������������� �������� ���� ��� ������� � ���� ��� ��������.
    /// </summary>
    /// <param name="obj">������, ��� �������� ����� ���������� ����</param>
    /// <param name="layer">����� ����</param>
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
    /// ���������� ������������� �������� ��� ��� ������� � ���� ��� ��������.
    /// </summary>
    /// <param name="obj">������, ��� �������� ����� ���������� ���</param>
    /// <param name="tag">���</param>
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
