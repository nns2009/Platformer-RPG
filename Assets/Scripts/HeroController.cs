using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Damageable))]
public class HeroController : MonoBehaviour
{
    public const string ObjectName = "Hero";
    private const float PressThreshold = 0.5f;

    public float Speed;
    public float AttackRechargeTime;
    private float attackRechargeTimeLeft = 0;

    [Space]
    public float DashRechargeTime;
    public float DashDistance;
    public float DashDuration;
    private bool lastDashPressed = false;
    private float dashLastTimestamp = -100;
    private float dashTimeRemaining;
    private Vector3 dashDirection;

    [Space]
    public GameObject ShadowPrefab;
    public float ShadowCastTime;
    private GameObject shadow;
    private float shadowLastPressedTime = -100;
    private bool lastShadowPressed = false;
    private bool shadowPressUnused = false;

    [Space]
    public Transform body;
    public Transform weaponHolder;
    public Transform weaponTip;
    public GameObject projectilePrefab;

    [Space]
    public PlayerInput playerInput;
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;
    private HeroInput input;

    private Checkpoint lastCheckpoint;

    private Rigidbody rb;
    private Damageable damageable;

    void Awake()
    {
        input = new HeroInput();
    }
    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        damageable = GetComponent<Damageable>();
    }

    void Update()
    {
        // TODO: Fix to access real camera
        var hero2cam = transform.position - virtualCamera.transform.position;
        var hero2camAngle = Mathf.Atan2(hero2cam.x, hero2cam.z);
        //Debug.Log(mainCamera.transform.position + " : " + virtualCamera.transform.position + " : " + hero2camAngle);

        bool isDashing = Time.time - dashLastTimestamp <= DashDuration;
        var movInp = input.Land.Move.ReadValue<Vector2>();
        var movDirection =
            Quaternion.AngleAxis(hero2camAngle * Mathf.Rad2Deg, Vector3.up) *
            new Vector3(movInp.x, 0, movInp.y);

        Vector3 planeMoveVelocity;
        if (isDashing)
        {
            var dashSpeed = DashDistance / DashDuration;
            //var posDelta = dashDirection * dashSpeed * Time.deltaTime;
            //transform.Translate(posDelta, Space.World);
            planeMoveVelocity = dashDirection * dashSpeed;
        }
        else
        {
            var posDelta = movDirection * Speed * Time.deltaTime;
            //transform.Translate(posDelta.DropY(), Space.World);
            planeMoveVelocity = movDirection * Speed;
        }
        rb.velocity = new Vector3(planeMoveVelocity.x, rb.velocity.y, planeMoveVelocity.z);

        Vector2 dirInp;

        if (playerInput.currentControlScheme == input.GamepadScheme.name)
        {
            dirInp = input.Land.Dir.ReadValue<Vector2>();
            //Debug.Log("GP" + " - " + dirInp);
        }
        else
        {
            Vector2 mousePos = input.Mouse.MousePosition.ReadValue<Vector2>();
            dirInp = new Vector2(
                mousePos.x * 2 / Screen.width - 1,
                mousePos.y * 2 / Screen.height - 1);
            //Debug.Log("Keyboard" + " - " + mousePos + " - " + dirInp);
        }

        if (!movDirection.AlmostZero())
        {
            body.transform.rotation = Quaternion.LookRotation(movDirection);
        }

        if (dirInp.sqrMagnitude >= 0.02)
        {
            var front2dirAngle = Mathf.Atan2(dirInp.x, dirInp.y);
            weaponHolder.rotation = Quaternion.Euler(0, (hero2camAngle + front2dirAngle) * Mathf.Rad2Deg, 0);
        }

        float attackInp = input.Land.Attack.ReadValue<float>();
        if (attackInp > PressThreshold && attackRechargeTimeLeft <= 0)
        {
            var projectileObject = Instantiate(projectilePrefab, weaponTip.position, weaponTip.rotation);
            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.Set(weaponTip.forward);
            attackRechargeTimeLeft = AttackRechargeTime;
        }

        bool dashPressed = input.Land.Dash.ReadValue<float>() >= PressThreshold;
        if (!lastDashPressed && dashPressed && Time.time - dashLastTimestamp >= DashRechargeTime)
        {
            dashLastTimestamp = Time.time;
            dashDirection = weaponTip.forward.DropY().normalized;
        }
        lastDashPressed = dashPressed;

        bool shadowPressed = input.Land.Shadow.ReadValue<float>() >= PressThreshold;
        if (!lastShadowPressed && shadowPressed)
        {
            shadowPressUnused = true;
            shadowLastPressedTime = Time.time;
        }
        else if (shadowPressed && shadowPressUnused && Time.time - shadowLastPressedTime >= ShadowCastTime)
        {
            // Cast shadow
            shadowPressUnused = false;
            if (shadow != null)
            {
                Destroy(shadow);
                shadow = null;
            }
            shadow = Instantiate(ShadowPrefab, body.position, body.rotation);
        }
        else if (lastShadowPressed && !shadowPressed && shadowPressUnused)
        {
            // Teleport to shadow
            if (shadow != null)
            {
                transform.position = shadow.transform.position;
                transform.rotation = shadow.transform.rotation;
                // TODO: (maybe?) fix the rotation of the weapon ?

                Destroy(shadow);
                shadow = null;
            }
        }
        lastShadowPressed = shadowPressed;

        attackRechargeTimeLeft = Mathf.Max(0, attackRechargeTimeLeft - Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var checkpoint = other.GetComponentInParent<Checkpoint>();
        if (checkpoint != null)
        {
            if (lastCheckpoint != null)
            {
                lastCheckpoint.Deactivate();
            }
            lastCheckpoint = checkpoint;
            checkpoint.Activate();
        }
    }

    public void RestartAtCheckpoint()
    {
        if (lastCheckpoint == null)
        {
            Debug.LogWarning("Hero haven't activated any checkpoints yet");
            return;
        }
        transform.position = lastCheckpoint.RebirthPoint.position;
        damageable.Restore();

        var enemyGroups = FindObjectsOfType<EnemyGroup>();
        foreach (var group in enemyGroups)
        {
            group.CheckpointReset();
        }
    }
}
