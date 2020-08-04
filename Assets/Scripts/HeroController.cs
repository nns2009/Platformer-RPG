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
    public float Speed;

    public Transform weaponHolder;
    public Transform body;

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
        
    }

    void Update()
    {
        // TODO: Fix to access real camera
        var hero2cam = transform.position - virtualCamera.transform.position;
        var hero2camAngle = Mathf.Atan2(hero2cam.x, hero2cam.z);
        //Debug.Log(mainCamera.transform.position + " : " + virtualCamera.transform.position + " : " + hero2camAngle);

        var movInp = input.Land.Move.ReadValue<Vector2>();
        var movDirection =
            Quaternion.AngleAxis(hero2camAngle * Mathf.Rad2Deg, Vector3.up) *
            new Vector3(movInp.x, 0, movInp.y);
        var posDelta = movDirection * Speed * Time.deltaTime;
        transform.Translate(posDelta.x, 0, posDelta.z, Space.World);

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

    }
}
