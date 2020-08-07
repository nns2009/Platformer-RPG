using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public float MaxHealth;
    public float Health;
    public bool AutoDestroy = true;

    [SerializeField]
    UnityEvent died;

    public void Start()
    {
        Restore();
    }

    public void ReceiveDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            died.Invoke();
            if (AutoDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Restore()
    {
        Health = MaxHealth;
    }
}
