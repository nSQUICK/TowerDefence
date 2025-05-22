using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main; // �������� �������� ������
    }

    private void LateUpdate()
    {
        // ������ 1: ��������� ������ ����� � ������, �������� ������������ ����������
        transform.LookAt(transform.position + mainCam.transform.forward, mainCam.transform.up);

        // ��� ������ 2: ��������� ���������� ���������� ������
        // transform.rotation = mainCam.transform.rotation;
    }
}
