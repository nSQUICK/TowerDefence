// CannonTower.cs
using UnityEngine;

public class CannonTower : Tower
{
    // Поле для стратегии атаки
    private IAttackStrategy attackStrategy;

    private void Awake()
    {
        // Инициализируем стратегию атаки (можно заменить на другую реализацию)
        attackStrategy = new DefaultAttackStrategy();
    }

    public override void Attack(Enemy target)
    {
        if (target != null)
        {
            // Используем стратегию атаки для нанесения урона
            attackStrategy.ExecuteAttack(this, target);
        }
    }

    public override void Upgrade()
    {
        // Простой апгрейд: увеличение урона и скорости атаки
        damage += 5f;
        attackSpeed += 0.5f;
        cost += 25; // увеличение стоимости улучшения
        // Можно добавить дополнительные эффекты (визуальные, звуковые и т.д.)
    }

    public override Enemy FindTarget()
    {
        // Ищем ближайшего врага в пределах attackRange
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float minDistance = Mathf.Infinity;
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange && distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
}
