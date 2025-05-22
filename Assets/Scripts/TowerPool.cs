// TowerPool.cs
using UnityEngine;
using System.Collections.Generic;

public class TowerPool : MonoBehaviour
{
    public static TowerPool Instance { get; private set; }

    [Header("Настройки пула башен")]
    public GameObject towerPrefab;
    public int poolSize = 10;

    private Queue<GameObject> poolQueue;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        poolQueue = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject towerObj = Instantiate(towerPrefab);
            towerObj.SetActive(false);
            poolQueue.Enqueue(towerObj);
        }
    }

    /// <summary>
    /// Получает башню из пула.
    /// </summary>
    public GameObject GetTower(Vector3 position, Quaternion rotation)
    {
        if (poolQueue.Count > 0)
        {
            GameObject towerObj = poolQueue.Dequeue();
            towerObj.SetActive(true);
            towerObj.transform.position = position;
            towerObj.transform.rotation = rotation;
            return towerObj;
        }
        else
        {
            return Instantiate(towerPrefab, position, rotation);
        }
    }

    /// <summary>
    /// Возвращает башню в пул.
    /// </summary>
    public void ReturnTower(GameObject towerObj)
    {
        towerObj.SetActive(false);
        poolQueue.Enqueue(towerObj);
    }
}
