// BasicEnemy.cs
using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        // Устанавливаем индивидуальные характеристики врага для базового примера
        health = 50f;
        speed = 3.5f;
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    // При необходимости можно переопределить и другие методы.
}
