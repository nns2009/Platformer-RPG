using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class HeroController : MonoBehaviour
{
    public const string ObjectName = "Hero";
    private const float PressThreshold = 0.5f;

    public float Speed;
    public float AttackRechargeTime;
    private float attackRechargeTimeLeft = 0;

    public float DashRechargeTime;
    public float DashDistance;
    public float DashDuration;
    private bool lastDashPressed = false;
    private float dashLastTimestamp = -100;
    private float dashTimeRemaining;
    private Vector3 dashDirection;

    public Transform body;
    public Transform weaponHolder;
    public Transform weaponTip;

    public GameObject projectilePrefab;

    private Rigidbody rb;

    private HeroInput input;
    public PlayerInput playerInput;
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;


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

        if (isDashing)
        {
            var dashSpeed = DashDistance / DashDuration;
            var posDelta = dashDirection * dashSpeed * Time.deltaTime;
            transform.Translate(posDelta, Space.World);
        }
        else
        {
            var posDelta = movDirection * Speed * Time.deltaTime;
            transform.Translate(posDelta.DropY(), Space.World);
        }

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

        attackRechargeTimeLeft = Mathf.Max(0, attackRechargeTimeLeft - Time.deltaTime);
    }
}
