// DefaultAttackStrategy.cs
using UnityEngine;

public class DefaultAttackStrategy : IAttackStrategy
{
    public void ExecuteAttack(Tower tower, Enemy target)
    {
        if (target != null)
        {
            // ������� ���� �����
            target.TakeDamage(tower.damage);
            // ����� �������� ���������� �������, ���� � �.�.
            Debug.Log($"{tower.name} ������� {target.name}, ������ {tower.damage} �����.");
        }
    }
}
