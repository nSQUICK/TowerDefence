// Enemy.cs
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [Header("��������� �����")]
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
        // ����������� �������� ������
        agent.speed = speed;
        // ������������� ���� � ���� ������
        if (GameManager.Instance != null && GameManager.Instance.baseTransform != null)
        {
            agent.SetDestination(GameManager.Instance.baseTransform.position);
        }
        // ������������� �� ������� ��������� ����� ��� ��������� ����
        GameManager.OnTowerPlaced += HandleTowerPlaced;
    }

    protected virtual void OnDestroy()
    {
        // ������������ �� �������, ����� �������� ������ ������
        GameManager.OnTowerPlaced -= HandleTowerPlaced;
    }

    /// <summary>
    /// ���������� ������� ��������� ����� � ������������� ���� �� ����.
    /// </summary>
    protected virtual void HandleTowerPlaced(Tower tower)
    {
        RecalculatePath();
    }

    /// <summary>
    /// ������������� ���� �� ����.
    /// </summary>
    public virtual void RecalculatePath()
    {
        if (GameManager.Instance != null && GameManager.Instance.baseTransform != null)
        {
            agent.SetDestination(GameManager.Instance.baseTransform.position);
        }
    }

    /// <summary>
    /// ����� ��������� �����.
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
    /// ����� ������ �����. ����� ����� ������� ������ � ���.
    /// </summary>
    public virtual void Die()
    {
        // ���� ���������� ��� ������, ���������� ������ � ���
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
