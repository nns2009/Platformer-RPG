using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public float Damage;
    public float Lifespan;

    private Vector3 direction;
    private float timeElapsed = 0;

    public void Set(Vector3 direction)
    {
        this.direction = direction;
        //this.lifespan = lifespan;
    }
    void Update()
    {
        timeElapsed += Time.deltaTime;
        transform.Translate(direction * Speed * Time.deltaTime, Space.World);

        if (timeElapsed > Lifespan)
        {
            Destroy(gameObject);
        }
    }
}
