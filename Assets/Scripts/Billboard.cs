using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main; // Кэшируем основную камеру
    }

    private void LateUpdate()
    {
        // Способ 1: Повернуть объект лицом к камере, сохраняя вертикальную ориентацию
        transform.LookAt(transform.position + mainCam.transform.forward, mainCam.transform.up);

        // Или Способ 2: Полностью копировать ориентацию камеры
        // transform.rotation = mainCam.transform.rotation;
    }
}
