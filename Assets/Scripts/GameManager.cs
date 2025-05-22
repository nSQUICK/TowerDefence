// GameManager.cs
using UnityEngine;
using System;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    // ���������� Singleton
    public static GameManager Instance { get; private set; }

    // ������� ���������
    [Header("������� ���������")]
    public int playerResources = 100;
    public int baseHealth = 100;

    [Tooltip("Transform ���� (���� ��� ������)")]
    public Transform baseTransform;

    [Tooltip("����� ������ ������")]
    public Transform enemySpawnPoint;

    // ������� ��� ���������� �� ��������� ����� � ������ ������ (Observer-�������)
    public static event Action<Tower> OnTowerPlaced;
    public static event Action<Enemy> OnEnemySpawned;

    private void Awake()
    {
        // ���������� Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���������� ��� �������� ������������ �����.
    /// �������� ��������� ����� �� �������� � ��������� �����������.
    /// </summary>
    /// <param name="tower">������������� �����</param>
    public void TowerPlaced(Tower tower)
    {
        playerResources -= tower.cost;
        OnTowerPlaced?.Invoke(tower);
        UIManager.Instance.UpdateResources(playerResources);
    }

    /// <summary>
    /// ���������� ��� ������ ����� � ��������� �����������.
    /// </summary>
    /// <param name="enemy">��������� ����</param>
    public void EnemySpawned(Enemy enemy)
    {
        OnEnemySpawned?.Invoke(enemy);
    }
}
