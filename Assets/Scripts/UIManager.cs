// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ���������� Singleton
    public static UIManager Instance { get; private set; }

    [Header("UI ��������")]
    public Text resourcesText;
    public Text waveText;
    public Text baseHealthText;
    public Text messageText;
    // � UIManager.cs
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
    /// ���������� ����������� ��������.
    /// </summary>
    public void UpdateResources(int resources)
    {
        resourcesText.text = "�������: " + resources;
    }

    /// <summary>
    /// ���������� ������ �����.
    /// </summary>
    public void UpdateWave(int wave)
    {
        waveText.text = "�����: " + wave;
    }

    /// <summary>
    /// ���������� ��������� ����.
    /// </summary>
    public void UpdateBaseHealth(int health)
    {
        baseHealthText.text = "�������� ����: " + health;
    }

    /// <summary>
    /// ���������� ��������� ������.
    /// </summary>
    public void ShowMessage(string message)
    {
        messageText.text = message;
        // ����� �������� �������� ��� ������� ��������� ����� ��������� �����.
    }
}
