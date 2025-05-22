// Tower.cs
using UnityEngine;
using UnityEngine.AI;

public abstract class Tower : MonoBehaviour
{
    [Header("��������� �����")]
    public float damage = 10f;
    public float attackRange = 5f;
    public float attackSpeed = 1f; // ����� ���� � �������
    public int cost = 50;

    // ���������� ��� ����� ������� ����� �������
    protected float attackCooldown = 0f;

    protected virtual void Start()
    {
        // ��������� NavMeshObstacle, ���� ��� ��� ���, ��� ������������� ���������� NavMesh
        if (GetComponent<NavMeshObstacle>() == null)
        {
            NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
            // ��� ������������� ����� ��������� ����� � ������ �����������
        }
    }

    protected virtual void Update()
    {
        // ������ �������� �����
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            // ���� ���� ��� �����
            Enemy target = FindTarget();
            if (target != null)
            {
                Attack(target);
                attackCooldown = 1f / attackSpeed; // ����� �������� � ����������� �� �������� �����
            }
        }
    }

    /// <summary>
    /// ����� �����, ����������� � �����������.
    /// </summary>
    public abstract void Attack(Enemy target);

    /// <summary>
    /// ����� ��������� ����� (��������).
    /// </summary>
    public abstract void Upgrade();

    /// <summary>
    /// ����� ���� ��� �����.
    /// </summary>
    public abstract Enemy FindTarget();
}
