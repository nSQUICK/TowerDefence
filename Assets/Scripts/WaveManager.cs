using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Реализация Singleton для удобного доступа к WaveManager
    public static WaveManager Instance { get; private set; }

    [Header("Настройки волн")]
    [Tooltip("Префаб врага, который будет спавниться")]
    public GameObject enemyPrefab;

    [Tooltip("Базовое количество врагов в волне")]
    public int baseEnemyCount = 3;

    [Tooltip("Время между спавном отдельных врагов в волне")]
    public float timeBetweenEnemies = 0.5f;

    [Tooltip("Время задержки после завершения волны, до появления кнопки запуска следующей волны")]
    public float timeAfterWave = 2f;

    private int currentWave = 0;
    private bool waveInProgress = false;

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Публичный метод, который вызывается по нажатию кнопки для запуска волны.
    /// Метод проверяет, что в данный момент волна не идёт, и запускает корутину спавна волны.
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
            Debug.Log("Волна уже запущена!");
        }
    }


    /// <summary>
    /// Корутина, которая отвечает за спавн врагов в текущей волне.
    /// После спавна всех врагов ждёт определённое время и сигнализирует, что волна завершена.
    /// </summary>
    private IEnumerator SpawnWave()
    {
        waveInProgress = true;
        currentWave++;
        UIManager.Instance.UpdateWave(currentWave);  // Обновляем номер волны в UI

        // Вычисляем количество врагов для текущей волны
        int enemyCount = baseEnemyCount * currentWave;
        Debug.Log("Запускается волна " + currentWave + ". В ней " + enemyCount + " врагов.");

        // Спавним врагов с заданной задержкой между спавном
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenEnemies);
        }

        // После завершения спавна можно подождать некоторое время, чтобы игрок подготовился к следующей волне
        yield return new WaitForSeconds(timeAfterWave);

        waveInProgress = false;
        // В этот момент можно активировать кнопку запуска новой волны,
        // например, через UIManager: UIManager.Instance.ShowWaveStartButton();
        Debug.Log("Волна завершена. Нажмите кнопку, чтобы запустить следующую волну.");
        UIManager.Instance.ShowWaveStartButton(true);

    }

    /// <summary>
    /// Метод для спавна одного врага.
    /// Если вы используете пулы объектов, замените Instantiate на получение из пула.
    /// </summary>
    private void SpawnEnemy()
    {
        // Спавним врага в позиции точки спауна, которая задается в GameManager
        GameObject enemyObj = Instantiate(enemyPrefab, GameManager.Instance.enemySpawnPoint.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        // Пример изменения характеристик врага в зависимости от номера волны:
        enemy.health += currentWave * 5;   // Увеличиваем здоровье
        enemy.speed += currentWave * 0.1f;   // Увеличиваем скорость

        // Оповещаем GameManager о спавне врага (если используется Observer-паттерн)
        GameManager.Instance.EnemySpawned(enemy);
    }
}
