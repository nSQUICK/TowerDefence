using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionUI : MonoBehaviour
{
    [Header("UI элементы")]
    [Tooltip("Префаб кнопки для выбора башни")]
    public GameObject towerButtonPrefab;

    [Tooltip("Контейнер (например, Panel) для кнопок. Контейнер должен иметь компонент Horizontal Layout Group и, при необходимости, Content Size Fitter.")]
    public Transform buttonContainer;

    [Header("Список доступных башен")]
    [Tooltip("Список префабов башен, доступных для установки")]
    public List<GameObject> availableTowerPrefabs;

    // Ссылка на менеджер установки башен
    private TowerPlacementManager towerPlacementManager;

    private void Start()
    {
        // Получаем ссылку на TowerPlacementManager из сцены
        towerPlacementManager = FindObjectOfType<TowerPlacementManager>();
        if (towerPlacementManager == null)
        {
            Debug.LogWarning("TowerPlacementManager не найден на сцене!");
            return;
        }

        // Очистим контейнер (если там уже что-то есть)
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Чтобы кнопки располагались справа налево, перебираем список в обратном порядке.
        for (int i = availableTowerPrefabs.Count - 1; i >= 0; i--)
        {
            GameObject towerPrefab = availableTowerPrefabs[i];
            GameObject buttonObj = Instantiate(towerButtonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogWarning("Префаб кнопки не содержит компонент Button!");
                continue;
            }

            // Получаем имя башни (можно использовать другое поле, если требуется)
            string towerName = towerPrefab.name;
            string statsInfo = "";

            // Пытаемся получить компонент Tower для извлечения характеристик
            Tower towerComponent = towerPrefab.GetComponent<Tower>();
            if (towerComponent != null)
            {
                // Форматируем строку с характеристиками башни
                statsInfo = $"Урон: {towerComponent.damage}\nДальность Атаки: {towerComponent.attackRange}\nЦена: {towerComponent.cost}";
            }
            else
            {
                statsInfo = "No stats available";
            }

            // Объединяем название и характеристики в один текст (можно отформатировать по своему вкусу)
            string buttonTextStr = $"{towerName}\n{statsInfo}";

            // Устанавливаем текст кнопки
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = buttonTextStr;
            }
            else
            {
                Debug.LogWarning("В кнопке не найден компонент Text!");
            }

            // Привязываем событие OnClick: при нажатии кнопки вызывается метод выбора башни
            button.onClick.AddListener(() => OnTowerSelected(towerPrefab));
        }
    }

    /// <summary>
    /// Метод, вызываемый при выборе башни.
    /// Передает выбранный префаб в TowerPlacementManager и скрывает UI-панель.
    /// </summary>
    /// <param name="selectedTowerPrefab">Выбранный префаб башни</param>
    public void OnTowerSelected(GameObject selectedTowerPrefab)
    {
        if (towerPlacementManager != null)
        {
            towerPlacementManager.SetSelectedTower(selectedTowerPrefab);
        }
        // Скрываем панель выбора башен после выбора
        gameObject.SetActive(false);
    }
}
