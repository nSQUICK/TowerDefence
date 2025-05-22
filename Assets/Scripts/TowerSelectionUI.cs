using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionUI : MonoBehaviour
{
    [Header("UI ��������")]
    [Tooltip("������ ������ ��� ������ �����")]
    public GameObject towerButtonPrefab;

    [Tooltip("��������� (��������, Panel) ��� ������. ��������� ������ ����� ��������� Horizontal Layout Group �, ��� �������������, Content Size Fitter.")]
    public Transform buttonContainer;

    [Header("������ ��������� �����")]
    [Tooltip("������ �������� �����, ��������� ��� ���������")]
    public List<GameObject> availableTowerPrefabs;

    // ������ �� �������� ��������� �����
    private TowerPlacementManager towerPlacementManager;

    private void Start()
    {
        // �������� ������ �� TowerPlacementManager �� �����
        towerPlacementManager = FindObjectOfType<TowerPlacementManager>();
        if (towerPlacementManager == null)
        {
            Debug.LogWarning("TowerPlacementManager �� ������ �� �����!");
            return;
        }

        // ������� ��������� (���� ��� ��� ���-�� ����)
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // ����� ������ ������������� ������ ������, ���������� ������ � �������� �������.
        for (int i = availableTowerPrefabs.Count - 1; i >= 0; i--)
        {
            GameObject towerPrefab = availableTowerPrefabs[i];
            GameObject buttonObj = Instantiate(towerButtonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogWarning("������ ������ �� �������� ��������� Button!");
                continue;
            }

            // �������� ��� ����� (����� ������������ ������ ����, ���� ���������)
            string towerName = towerPrefab.name;
            string statsInfo = "";

            // �������� �������� ��������� Tower ��� ���������� �������������
            Tower towerComponent = towerPrefab.GetComponent<Tower>();
            if (towerComponent != null)
            {
                // ����������� ������ � ���������������� �����
                statsInfo = $"����: {towerComponent.damage}\n��������� �����: {towerComponent.attackRange}\n����: {towerComponent.cost}";
            }
            else
            {
                statsInfo = "No stats available";
            }

            // ���������� �������� � �������������� � ���� ����� (����� ��������������� �� ������ �����)
            string buttonTextStr = $"{towerName}\n{statsInfo}";

            // ������������� ����� ������
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = buttonTextStr;
            }
            else
            {
                Debug.LogWarning("� ������ �� ������ ��������� Text!");
            }

            // ����������� ������� OnClick: ��� ������� ������ ���������� ����� ������ �����
            button.onClick.AddListener(() => OnTowerSelected(towerPrefab));
        }
    }

    /// <summary>
    /// �����, ���������� ��� ������ �����.
    /// �������� ��������� ������ � TowerPlacementManager � �������� UI-������.
    /// </summary>
    /// <param name="selectedTowerPrefab">��������� ������ �����</param>
    public void OnTowerSelected(GameObject selectedTowerPrefab)
    {
        if (towerPlacementManager != null)
        {
            towerPlacementManager.SetSelectedTower(selectedTowerPrefab);
        }
        // �������� ������ ������ ����� ����� ������
        gameObject.SetActive(false);
    }
}
