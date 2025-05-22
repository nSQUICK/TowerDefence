// Tower.cs
using UnityEngine;
using UnityEngine.AI;

public abstract class Tower : MonoBehaviour
{
    [Header("Параметры башни")]
    public float damage = 10f;
    public float attackRange = 5f;
    public float attackSpeed = 1f; // число атак в секунду
    public int cost = 50;

    // Переменная для учета времени между атаками
    protected float attackCooldown = 0f;

    protected virtual void Start()
    {
        // Добавляем NavMeshObstacle, если его ещё нет, для динамического обновления NavMesh
        if (GetComponent<NavMeshObstacle>() == null)
        {
            NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
            // При необходимости можно настроить форму и размер препятствия
        }
    }

    protected virtual void Update()
    {
        // Отсчет кулдауна атаки
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            // Ищем цель для атаки
            Enemy target = FindTarget();
            if (target != null)
            {
                Attack(target);
                attackCooldown = 1f / attackSpeed; // сброс кулдауна в зависимости от скорости атаки
            }
        }
    }

    /// <summary>
    /// Метод атаки, реализуется в наследниках.
    /// </summary>
    public abstract void Attack(Enemy target);

    /// <summary>
    /// Метод улучшения башни (апгрейда).
    /// </summary>
    public abstract void Upgrade();

    /// <summary>
    /// Поиск цели для атаки.
    /// </summary>
    public abstract Enemy FindTarget();
}
