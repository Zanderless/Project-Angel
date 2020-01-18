using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : WorldCharacter
{

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Movement Properties")]
    public float walkSpeed;
    public float runSpeedMultiplier;
    private float moveSpeed;

    private const float GRAVITY = 20f;

    private Vector3 movementDir; 
    private CharacterController Controller => GetComponent<CharacterController>();

    private void Awake()
    {
    }

    private void Update()
    {

        if (playerCamera.gameObject.activeSelf == BattleManager.Instance.InBattle)
            playerCamera.gameObject.SetActive(!BattleManager.Instance.InBattle);

        if (BattleManager.Instance.InBattle || PauseMenu.Instance.IsPaused)
            return;

        Movement();
    }

    private void Movement()
    {

        moveSpeed = walkSpeed;

        float h = Input.GetAxis("Horizontal") * moveSpeed;
        float v = Input.GetAxis("Vertical") * moveSpeed;

        movementDir = new Vector3(h, movementDir.y, v);

        movementDir = transform.TransformDirection(movementDir);

        movementDir.y -= GRAVITY * Time.deltaTime;

        Controller.Move(movementDir * Time.deltaTime);

    }

}
