// GameManager.cs
using UnityEngine;
using System;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    // Реализация Singleton
    public static GameManager Instance { get; private set; }

    // Игровые параметры
    [Header("Игровые параметры")]
    public int playerResources = 100;
    public int baseHealth = 100;

    [Tooltip("Transform базы (цель для врагов)")]
    public Transform baseTransform;

    [Tooltip("Точка спауна врагов")]
    public Transform enemySpawnPoint;

    // События для оповещения об установке башен и спавне врагов (Observer-паттерн)
    public static event Action<Tower> OnTowerPlaced;
    public static event Action<Enemy> OnEnemySpawned;

    private void Awake()
    {
        // Реализация Singleton
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
    /// Вызывается при успешном установлении башни.
    /// Вычитает стоимость башни из ресурсов и оповещает подписчиков.
    /// </summary>
    /// <param name="tower">Установленная башня</param>
    public void TowerPlaced(Tower tower)
    {
        playerResources -= tower.cost;
        OnTowerPlaced?.Invoke(tower);
        UIManager.Instance.UpdateResources(playerResources);
    }

    /// <summary>
    /// Вызывается при спавне врага и оповещает подписчиков.
    /// </summary>
    /// <param name="enemy">Созданный враг</param>
    public void EnemySpawned(Enemy enemy)
    {
        OnEnemySpawned?.Invoke(enemy);
    }
}
