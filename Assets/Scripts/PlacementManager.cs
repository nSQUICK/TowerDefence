using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlacementManager : MonoBehaviour
{
    [Header("��������� ��������� �����")]
    [Tooltip("������ �����, ��������� ��� ��������� (�������� ����� UI ������)")]
    public GameObject selectedTowerPrefab;

    [Tooltip("������ �� NavMeshSurface �������� ���� (��������, Terrain)")]
    public NavMeshSurface navMeshSurface;

    [Tooltip("LayerMask, ���������� ������ ����, �� ������� ��������� ������� ��� (��������, 'Ground')")]
    public LayerMask groundLayer;

    [Header("��������� ������ �����")]
    [Tooltip("����, �����������, ��� ����� ��������� ��������� (� �������������)")]
    public Color validColor = new Color(0, 1, 0, 0.5f);  // �������
    [Tooltip("����, �����������, ��� ����� ��������� ����������� (� �������������)")]
    public Color invalidColor = new Color(1, 0, 0, 0.5f);  // �������

    // ��������� ���������� ��� ������ ������
    private GameObject towerPreviewInstance;
    private Renderer[] previewRenderers;

    // ��� �������� ������
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // ���� ����� �� ������� ��� ������ �� �������, ������ �� ������
        if (selectedTowerPrefab == null || towerPreviewInstance == null)
            return;

        // ���������� Raycast ������ �� ���� groundLayer, ����� �������� ������ �� ������� ���
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // �������������� ��������: ��������, ��� Raycast �������� ������ � ������ � ����� "Ground"
            if (!hit.collider.CompareTag("Ground"))
            {
                // ���� ���� �� �� ����, �� ��������� ������� ������
                return;
            }

            Vector3 placementPosition = hit.point;
            // ��������� ������� ������
            towerPreviewInstance.transform.position = placementPosition;

            // ���������, ����� �� ���������� ����� � ������ �����
            bool isValid = IsPlacementValid(placementPosition);
            SetPreviewColor(isValid ? validColor : invalidColor);

            // ��� ����� (����� ������ ����) � ���� ����� ��������� ��������� � ������������ ���������
            if (Input.GetMouseButtonDown(0) && isValid)
            {
                ConfirmPlacement(placementPosition);
            }
        }
    }

    /// <summary>
    /// ���������� ��� ��������� ���������� ������� �����.
    /// ������ ���� ����� ���������� �� UI (��������, ����� ������ ������ �����).
    /// ��������� ������ ��������� �����.
    /// </summary>
    /// <param name="towerPrefab">��������� ������ �����</param>
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

        // �������� ������ �� ����, ������� �� ��������� � Raycast (��������, "Ignore Raycast")
        towerPreviewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");

        // ��������� ����������, �������� �� ������ � ������ ������ (����� ������ �� ����������� ���� � �� ��������)
        DisablePreviewComponents(towerPreviewInstance);

        // �������� ��������� ��� ��������� �����
        previewRenderers = towerPreviewInstance.GetComponentsInChildren<Renderer>();

        // ������������� ��������� ���� ������ ��� ����������
        SetPreviewColor(validColor);
    }

    /// <summary>
    /// ��������� ������������� ���������� (����������, NavMeshObstacle, ������ Tower) � ������� ������,
    /// ����� �� �� ����� �� ������� �������.
    /// </summary>
    /// <param name="preview">������-������</param>
    private void DisablePreviewComponents(GameObject preview)
    {
        // ��������� ��� ����������
        Collider[] colliders = preview.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // ��������� ������, ���������� �� ������ ������ (���� �� ����)
        Tower towerComp = preview.GetComponent<Tower>();
        if (towerComp != null)
        {
            towerComp.enabled = false;
        }

        // ��������� ��������� NavMeshObstacle, ����� �� �� ����� �� NavMesh
        NavMeshObstacle obstacle = preview.GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }

    /// <summary>
    /// ������������� ���� ��� ���� ���������� ������� ������.
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
    /// �������� ��������:
    /// 1. ���������� ��� ������������� ����� � ����������� (�� ���� "Tower").
    /// 2. �������� NavMesh-������������ (����� �� ���� �� ����� ������ �� ����).
    /// </summary>
    /// <param name="position">������� ���������</param>
    /// <returns>true, ���� ��������� ���������; ����� false</returns>
    private bool IsPlacementValid(Vector3 position)
    {
        // 1. ���������, ��� ����� ��� ��� ������������� �����.
        // ��������������, ��� ��� ������������� ����� ����� ��� "Tower".
        float checkRadius = 10.0f;

        // �������� ��� ���������� � ��������� �������
        Collider[] nearbyColliders = Physics.OverlapSphere(position, checkRadius);
        foreach (Collider col in nearbyColliders)
        {
            Debug.Log("������ ������: " + col.gameObject.name + " � �����: " + col.gameObject.tag);
            // ���� ������ ������ � ����� "Tower", ��������� �����������
            if (col.gameObject.CompareTag("Tower"))
            {
                return false;
            }
        }

        // 2. ��������� NavMesh-������������.
        // ������� ��������� ������ � NavMeshObstacle, ����� ������������� �����������.
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
        bool valid = pathFound && path.status == NavMeshPathStatus.PathComplete;

        Destroy(tempObstacle);

        // ��� ������������� ��������� NavMesh ����� (����� ������� �������� ���������)
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        return valid;
    }

    /// <summary>
    /// ������������ ��������� ����� � ��������� �������.
    /// ��������� �������� ������ �����, ���������� ����� GameManager ��� �������� �������� � ���������� UI,
    /// � ������-������ ���������.
    /// </summary>
    /// <param name="position">������� ��������� �����</param>
    private void ConfirmPlacement(Vector3 position)
    {
        // ������� �������� ������ �����
        GameObject towerInstance = Instantiate(selectedTowerPrefab, position, Quaternion.identity);

        // ����������� ������������� ����� ���� � ���, ����� � ������� �� ����� ���� ����������
        towerInstance.layer = LayerMask.NameToLayer("Tower");
        towerInstance.tag = "Tower";

        Tower towerComponent = towerInstance.GetComponent<Tower>();
        if (towerComponent != null)
        {
            GameManager.Instance.TowerPlaced(towerComponent);
        }
        else
        {
            Debug.LogWarning("��������� ������ �� �������� ��������� Tower!");
        }

        // ������� ������ � ���������� �����
        Destroy(towerPreviewInstance);
        towerPreviewInstance = null;
        selectedTowerPrefab = null;
    }
}
