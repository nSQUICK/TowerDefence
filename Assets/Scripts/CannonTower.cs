// CannonTower.cs
using UnityEngine;

public class CannonTower : Tower
{
    // ���� ��� ��������� �����
    private IAttackStrategy attackStrategy;

    private void Awake()
    {
        // �������������� ��������� ����� (����� �������� �� ������ ����������)
        attackStrategy = new DefaultAttackStrategy();
    }

    public override void Attack(Enemy target)
    {
        if (target != null)
        {
            // ���������� ��������� ����� ��� ��������� �����
            attackStrategy.ExecuteAttack(this, target);
        }
    }

    public override void Upgrade()
    {
        // ������� �������: ���������� ����� � �������� �����
        damage += 5f;
        attackSpeed += 0.5f;
        cost += 25; // ���������� ��������� ���������
        // ����� �������� �������������� ������� (����������, �������� � �.�.)
    }

    public override Enemy FindTarget()
    {
        // ���� ���������� ����� � �������� attackRange
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
