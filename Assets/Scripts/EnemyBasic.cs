using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public float Speed;
    public float MinChaseDistance;
    public float AttackDistance;
    public float MaxChaseDistance;

    //public float Damage;
    public float AttackRechargeTime;

    private float attackRechargeTimeLeft = 0;

    public Transform weaponTip;
    public GameObject projectilePrefab;

    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.Find(HeroController.ObjectName).transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var toPlayer = player.position - weaponTip.transform.position;
        var toPlayerDir = toPlayer.normalized;

        if (Speed != 0 &&
            toPlayer.sqrMagnitude <= MaxChaseDistance * MaxChaseDistance &&
            MinChaseDistance * MinChaseDistance <= toPlayer.sqrMagnitude)
        {
            rb.velocity = toPlayerDir * Speed;
            //var posDelta = Speed * Time.deltaTime * toPlayerDir;
            //transform.Translate(posDelta, Space.World);
            transform.rotation = Quaternion.LookRotation(toPlayer.DropY());
        }

        attackRechargeTimeLeft = Mathf.Max(0, attackRechargeTimeLeft - Time.deltaTime);
        if (toPlayer.sqrMagnitude <= AttackDistance * AttackDistance && attackRechargeTimeLeft <= 0)
        {
            attackRechargeTimeLeft = AttackRechargeTime;

            var projectileObject = Instantiate(projectilePrefab, weaponTip.position, weaponTip.rotation);
            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.Set(toPlayerDir);
        }
    }
}
