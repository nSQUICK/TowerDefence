// EnemyPool.cs
using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Header("��������� ���� ������")]
    public GameObject enemyPrefab;
    public int poolSize = 20;

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
            GameObject enemyObj = Instantiate(enemyPrefab);
            enemyObj.SetActive(false);
            poolQueue.Enqueue(enemyObj);
        }
    }

    /// <summary>
    /// �������� ����� �� ����.
    /// </summary>
    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        if (poolQueue.Count > 0)
        {
            GameObject enemyObj = poolQueue.Dequeue();
            enemyObj.SetActive(true);
            enemyObj.transform.position = position;
            enemyObj.transform.rotation = rotation;
            return enemyObj;
        }
        else
        {
            // ���� ��� ����, ������ ������ �����.
            return Instantiate(enemyPrefab, position, rotation);
        }
    }

    /// <summary>
    /// ���������� ����� � ���.
    /// </summary>
    public void ReturnEnemy(GameObject enemyObj)
    {
        enemyObj.SetActive(false);
        poolQueue.Enqueue(enemyObj);
    }
}
