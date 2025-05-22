// Enemy.cs
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Параметры врага")]
    public float health = 50f;
    public float speed = 3.5f;
    private FloatingHealthBar healthBar;
    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    protected virtual void Start()
    {
        // Настраиваем скорость агента
        agent.speed = speed;
        // Устанавливаем цель – базу игрока
        if (GameManager.Instance != null && GameManager.Instance.baseTransform != null)
        {
            agent.SetDestination(GameManager.Instance.baseTransform.position);
        }
        // Подписываемся на событие установки башни для пересчёта пути
        GameManager.OnTowerPlaced += HandleTowerPlaced;
    }

    protected virtual void OnDestroy()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        GameManager.OnTowerPlaced -= HandleTowerPlaced;
    }

    /// <summary>
    /// Обработчик события установки башни – пересчитывает путь до базы.
    /// </summary>
    protected virtual void HandleTowerPlaced(Tower tower)
    {
        RecalculatePath();
    }

    /// <summary>
    /// Пересчитывает путь до базы.
    /// </summary>
    public virtual void RecalculatePath()
    {
        if (GameManager.Instance != null && GameManager.Instance.baseTransform != null)
        {
            agent.SetDestination(GameManager.Instance.baseTransform.position);
        }
    }

    /// <summary>
    /// Метод получения урона.
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.UpdateHealtBar();
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Метод смерти врага. Здесь можно вернуть объект в пул.
    /// </summary>
    public virtual void Die()
    {
        // Если реализован пул врагов, возвращаем объект в пул
        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.ReturnEnemy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
