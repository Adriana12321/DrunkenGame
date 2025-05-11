using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    public sliderAdjustment slider;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        slider.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)){
            TakeDamage(1);
        }

        if(Input.GetKeyDown(KeyCode.H)){
            HealUp(1);
        }
    }

    void TakeDamage(int damage){
        currentHealth -= damage;

        slider.setHealth(currentHealth);
    }

    void HealUp(int heal){
        currentHealth += heal;

        slider.setHealth(currentHealth);
    }
}
