using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Slider slider;
    private Enemy Enemy;
    private float maxHealth;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogWarning("Не найден Slider среди дочерних объектов " + gameObject.name);
        }
        Enemy = GetComponentInParent<Enemy>();
        maxHealth = Enemy.health;
    }

    public void UpdateHealtBar()
    {
        slider.value = Enemy.health / maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
