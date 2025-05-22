// DefaultAttackStrategy.cs
using UnityEngine;

public class DefaultAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Tower tower, Enemy target)
    {
        if (target != null)
        {
            // Наносим урон врагу
            target.TakeDamage(tower.damage);
            // Можно добавить визуальные эффекты, звук и т.д.
            Debug.Log($"{tower.name} атакует {target.name}, нанося {tower.damage} урона.");
        }
    }
}
