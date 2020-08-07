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

    void Start()
    {
        player = GameObject.Find(HeroController.ObjectName).transform;
    }

    void Update()
    {
        var toPlayer = player.position - weaponTip.transform.position;
        var toPlayerDir = toPlayer.normalized;

        if (toPlayer.sqrMagnitude <= MaxChaseDistance * MaxChaseDistance &&
            MinChaseDistance * MinChaseDistance <= toPlayer.sqrMagnitude)
        {
            var posDelta = Speed * Time.deltaTime * toPlayerDir;
            transform.Translate(posDelta, Space.World);
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
