using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour
{
    [Header("��������� ����")]
    [Tooltip("������������ �������� ����")]
    public int maxHealth = 100;
    private int currentHealth;

    [Tooltip("������ �� UI-����� ��� ����������� �������� ����")]
    public Text baseHealthText;

    [Tooltip("������� ����� ������� ���� ����, ��������� ����")]
    public int damagePerEnemy = 10;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// ����� ���������� ��� ��������� ����� � ����.
    /// �������� ��������, ��� ���� ������ ����� Collider � Is Trigger.
    /// </summary>
    /// <param name="other">��������� ��������� �������</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("������������ � �����");
        Enemy enemy = other.GetComponent<Enemy>();
        // ������������, ��� ����� ����� ��� "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // ������� ���� ����
            TakeDamage(damagePerEnemy);

            // ������� ����� (��� ���������� � ���)
            enemy.Die();
        }
    }

    /// <summary>
    /// ������� ���� ���� � ��������� UI.
    /// ���� �������� ���� ������ �� ����, ���� �������������.
    /// </summary>
    /// <param name="damage">���������� �����</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// ��������� UI-����� �������� ����.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (baseHealthText != null)
        {
            baseHealthText.text = "Base Health: " + currentHealth;
        }
    }

    /// <summary>
    /// ����������, ����� �������� ���� ���������� �� 0.
    /// ����� ����� ����������� ��������� ����, ����� Game Over UI � �.�.
    /// </summary>
    private void GameOver()
    {
        Time.timeScale = 0f;
        Debug.Log("Game Over! Base destroyed.");
        // ��������, ���������� ����� ����, �������� Game Over ������, ��������� ����� Game Over � �.�.
        // GameManager.Instance.GameOver(); // ����� ����������� ��������� ����� � GameManager
    }
}
