// BasicEnemy.cs
using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        // ������������� �������������� �������������� ����� ��� �������� �������
        health = 50f;
        speed = 3.5f;
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    // ��� ������������� ����� �������������� � ������ ������.
}
