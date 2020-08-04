using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float MaxHealth;
    public float Health;

    public void Start()
    {
        Health = MaxHealth;
    }

    public void ReceiveDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
