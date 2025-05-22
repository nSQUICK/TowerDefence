using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour
{
    [Header("Настройки базы")]
    [Tooltip("Максимальное здоровье базы")]
    public int maxHealth = 100;
    private int currentHealth;

    [Tooltip("Ссылка на UI-текст для отображения здоровья базы")]
    public Text baseHealthText;

    [Tooltip("Сколько урона наносит один враг, достигший базы")]
    public int damagePerEnemy = 10;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// Метод вызывается при попадании врага в базу.
    /// Обратите внимание, что база должна иметь Collider с Is Trigger.
    /// </summary>
    /// <param name="other">Коллайдер входящего объекта</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("СТОЛКНОВЕНИЕ С БАЗОЙ");
        Enemy enemy = other.GetComponent<Enemy>();
        // Предполагаем, что враги имеют тег "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // Наносим урон базе
            TakeDamage(damagePerEnemy);

            // Удаляем врага (или возвращаем в пул)
            enemy.Die();
        }
    }

    /// <summary>
    /// Наносит урон базе и обновляет UI.
    /// Если здоровье базы падает до нуля, игра заканчивается.
    /// </summary>
    /// <param name="damage">Количество урона</param>
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
    /// Обновляет UI-текст здоровья базы.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (baseHealthText != null)
        {
            baseHealthText.text = "Base Health: " + currentHealth;
        }
    }

    /// <summary>
    /// Вызывается, когда здоровье базы опускается до 0.
    /// Здесь можно реализовать остановку игры, вывод Game Over UI и т.д.
    /// </summary>
    private void GameOver()
    {
        Time.timeScale = 0f;
        Debug.Log("Game Over! Base destroyed.");
        // Например, остановите спавн волн, покажите Game Over панель, загрузите сцену Game Over и т.д.
        // GameManager.Instance.GameOver(); // Можно реализовать отдельный метод в GameManager
    }
}
