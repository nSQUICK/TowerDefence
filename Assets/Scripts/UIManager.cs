// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Реализация Singleton
    public static UIManager Instance { get; private set; }

    [Header("UI Элементы")]
    public Text resourcesText;
    public Text waveText;
    public Text baseHealthText;
    public Text messageText;
    // В UIManager.cs
    public GameObject waveStartButton;

    public void ShowWaveStartButton(bool show)
    {
        if (waveStartButton != null)
        {
            waveStartButton.SetActive(show);
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Обновление отображения ресурсов.
    /// </summary>
    public void UpdateResources(int resources)
    {
        resourcesText.text = "Ресурсы: " + resources;
    }

    /// <summary>
    /// Обновление номера волны.
    /// </summary>
    public void UpdateWave(int wave)
    {
        waveText.text = "Волна: " + wave;
    }

    /// <summary>
    /// Обновление состояния базы.
    /// </summary>
    public void UpdateBaseHealth(int health)
    {
        baseHealthText.text = "Здоровье базы: " + health;
    }

    /// <summary>
    /// Отображает сообщение игроку.
    /// </summary>
    public void ShowMessage(string message)
    {
        messageText.text = message;
        // Можно добавить корутину для очистки сообщения через некоторое время.
    }
}
