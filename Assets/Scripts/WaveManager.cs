using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // ���������� Singleton ��� �������� ������� � WaveManager
    public static WaveManager Instance { get; private set; }

    [Header("��������� ����")]
    [Tooltip("������ �����, ������� ����� ����������")]
    public GameObject enemyPrefab;

    [Tooltip("������� ���������� ������ � �����")]
    public int baseEnemyCount = 3;

    [Tooltip("����� ����� ������� ��������� ������ � �����")]
    public float timeBetweenEnemies = 0.5f;

    [Tooltip("����� �������� ����� ���������� �����, �� ��������� ������ ������� ��������� �����")]
    public float timeAfterWave = 2f;

    private int currentWave = 0;
    private bool waveInProgress = false;

    private void Awake()
    {
        // ���������� Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// ��������� �����, ������� ���������� �� ������� ������ ��� ������� �����.
    /// ����� ���������, ��� � ������ ������ ����� �� ���, � ��������� �������� ������ �����.
    /// </summary>
    public void StartWave()
    {
        if (!waveInProgress)
        {
            UIManager.Instance.ShowWaveStartButton(false);
            StartCoroutine(SpawnWave());
        }
        else
        {
            Debug.Log("����� ��� ��������!");
        }
    }


    /// <summary>
    /// ��������, ������� �������� �� ����� ������ � ������� �����.
    /// ����� ������ ���� ������ ��� ����������� ����� � �������������, ��� ����� ���������.
    /// </summary>
    private IEnumerator SpawnWave()
    {
        waveInProgress = true;
        currentWave++;
        UIManager.Instance.UpdateWave(currentWave);  // ��������� ����� ����� � UI

        // ��������� ���������� ������ ��� ������� �����
        int enemyCount = baseEnemyCount * currentWave;
        Debug.Log("����������� ����� " + currentWave + ". � ��� " + enemyCount + " ������.");

        // ������� ������ � �������� ��������� ����� �������
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenEnemies);
        }

        // ����� ���������� ������ ����� ��������� ��������� �����, ����� ����� ������������ � ��������� �����
        yield return new WaitForSeconds(timeAfterWave);

        waveInProgress = false;
        // � ���� ������ ����� ������������ ������ ������� ����� �����,
        // ��������, ����� UIManager: UIManager.Instance.ShowWaveStartButton();
        Debug.Log("����� ���������. ������� ������, ����� ��������� ��������� �����.");
        UIManager.Instance.ShowWaveStartButton(true);

    }

    /// <summary>
    /// ����� ��� ������ ������ �����.
    /// ���� �� ����������� ���� ��������, �������� Instantiate �� ��������� �� ����.
    /// </summary>
    private void SpawnEnemy()
    {
        // ������� ����� � ������� ����� ������, ������� �������� � GameManager
        GameObject enemyObj = Instantiate(enemyPrefab, GameManager.Instance.enemySpawnPoint.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        // ������ ��������� ������������� ����� � ����������� �� ������ �����:
        enemy.health += currentWave * 5;   // ����������� ��������
        enemy.speed += currentWave * 0.1f;   // ����������� ��������

        // ��������� GameManager � ������ ����� (���� ������������ Observer-�������)
        GameManager.Instance.EnemySpawned(enemy);
    }
}
