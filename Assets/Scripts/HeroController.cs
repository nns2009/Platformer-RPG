using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public float Speed;

    private HeroInput input;
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
        var posDelta = Quaternion.AngleAxis(hero2camAngle * Mathf.Rad2Deg, Vector3.up) *
            new Vector3(movInp.x, 0, movInp.y) * Speed * Time.deltaTime;
        transform.Translate(posDelta.x, 0, posDelta.z, Space.World);
    }
}
