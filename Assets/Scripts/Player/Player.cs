﻿using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float Health = 200;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimatorController animCtrl;
    private PlayerControls controls;
    private playerPickup pickup;
    private PlayerTrigger playerTrigger;
    private Collider2D playerCollider;

    private Vector2 moveInput;
    private Vector2 playerLookAround;
    public float lookSensitivity = 5f;

    private bool Hit = false;
    private float BaseDamage = 10f;
    public float CurrentDamage;

    private GameObject near;

    // --- Vehicle ---
    private bool isDriving = false;
    private VehicleController currentVehicle;
    private bool skipVehicleInputFrame = false;
    private Collider2D vehicleCollider;

    // --- Debug ---
    private Coroutine vehicleDebugRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animCtrl = GetComponent<PlayerAnimatorController>();
        pickup = GetComponent<playerPickup>();
        playerTrigger = GetComponentInChildren<PlayerTrigger>();
        playerCollider = GetComponent<Collider2D>();

        controls = new PlayerControls();

        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.LookAround.look.performed += ctx => playerLookAround = ctx.ReadValue<Vector2>();
        controls.LookAround.look.canceled += ctx => playerLookAround = Vector2.zero;
        controls.Combat.Attack.performed += ctx => OnAttack();
        controls.PickUp.pick.performed += ctx => PickUp();
        controls.PutDown.drop.performed += ctx => Drop();
        controls.SwitchWeapon.Scroll.performed += ctx =>
        {
            float scroll = ctx.ReadValue<float>();
            if (Mathf.Abs(scroll) > 0.01f)
                pickup.SwitchWeapon(scroll > 0 ? 1 : -1);
        };

        controls.EnterExitVehicle.EnterExitCar.performed += ctx => ToggleVehicle();

        CurrentDamage = BaseDamage;
    }

    void OnEnable()
    {
        EnableOnFootControls(true);
        controls.EnterExitVehicle.Enable();
        controls.VehicleController.Enable();
    }

    void OnDisable()
    {
        controls.Movement.Disable();
        controls.Combat.Disable();
        controls.LookAround.Disable();
        controls.PickUp.Disable();
        controls.PutDown.Disable();
        controls.SwitchWeapon.Disable();
        controls.EnterExitVehicle.Disable();
        controls.VehicleController.Disable();
    }

    void FixedUpdate()
    {
        if (!isDriving)
        {
            Move();
            ApplyLookRotation();
            Animate();
        }
        else if (currentVehicle != null)
        {
            // Lấy input xe
            if (skipVehicleInputFrame)
            {
                currentVehicle.SetMoveInput(Vector2.zero);
                skipVehicleInputFrame = false;
            }
            else
            {
                Vector2 vehicleInput = controls.VehicleController.VehicleMove.ReadValue<Vector2>();
                currentVehicle.SetMoveInput(vehicleInput);
            }

            // Player đi theo xe nhưng không tác động vật lý
            transform.position = currentVehicle.transform.position;
        }
    }

    private void Move()
    {
        Vector2 movement = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement);
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
            spriteRenderer.flipX = movement.x < 0;
        }
    }

    private void ApplyLookRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(playerLookAround);
        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = Mathf.LerpAngle(rb.rotation, angle, lookSensitivity * Time.fixedDeltaTime);
    }

    private void Animate()
    {
        bool isMoving = moveInput != Vector2.zero;
        animCtrl?.OnRunning(isMoving);
        if (spriteRenderer != null && moveInput.x != 0)
            spriteRenderer.flipX = moveInput.x < 0;
    }

    private void OnAttack()
    {
        if (isDriving) return;
        if (pickup.currentGun != null)
        {
            pickup.currentGun.Fire(pickup);
            return;
        }
        if (Hit || animCtrl == null) return;

        animCtrl.Attack();
        Hit = true;
        if (playerTrigger != null)
        {
            foreach (IDamageable target in playerTrigger.GetTargets().ToList())
                target.TakeDamage(pickup.CurrentDamage, transform);
        }
        StartCoroutine(ResetHit(0.5f));
    }

    private IEnumerator ResetHit(float delay)
    {
        yield return new WaitForSeconds(delay);
        animCtrl?.ResetAttack();
        Hit = false;
    }

    private void PickUp() => pickup.PickUp();
    private void Drop() => pickup.Drop();

    private void EnableOnFootControls(bool enable)
    {
        if (enable)
        {
            controls.Movement.Enable();
            controls.Combat.Enable();
            controls.LookAround.Enable();
            controls.PickUp.Enable();
            controls.PutDown.Enable();
            controls.SwitchWeapon.Enable();
        }
        else
        {
            controls.Movement.Disable();
            controls.Combat.Disable();
            controls.LookAround.Disable();
            controls.PickUp.Disable();
            controls.PutDown.Disable();
            controls.SwitchWeapon.Disable();
        }
    }

    private void ToggleVehicle()
    {
        if (!isDriving && currentVehicle != null)
        {
            // Enter vehicle
            isDriving = true;
            spriteRenderer.enabled = false;
            EnableOnFootControls(false);
            currentVehicle.EnableDriving(true);
            skipVehicleInputFrame = true;

            vehicleCollider = currentVehicle.GetComponent<Collider2D>();
            if (vehicleCollider != null && playerCollider != null)
                Physics2D.IgnoreCollision(playerCollider, vehicleCollider, true);

            if (vehicleDebugRoutine != null) StopCoroutine(vehicleDebugRoutine);
            vehicleDebugRoutine = StartCoroutine(VehicleDebugLoop());
        }
        else if (isDriving)
        {
            // Exit vehicle
            isDriving = false;
            spriteRenderer.enabled = true;
            EnableOnFootControls(true);

            if (currentVehicle != null)
            {
                currentVehicle.EnableDriving(false);
                transform.position = currentVehicle.transform.position + currentVehicle.transform.right * 1.5f;
                if (vehicleCollider != null && playerCollider != null)
                    Physics2D.IgnoreCollision(playerCollider, vehicleCollider, false);
                currentVehicle = null;
            }

            if (vehicleDebugRoutine != null)
            {
                StopCoroutine(vehicleDebugRoutine);
                vehicleDebugRoutine = null;
            }
        }
    }

    private IEnumerator VehicleDebugLoop()
    {
        Vector3 lastPos = transform.position;
        Vector3 lastRot = transform.rotation.eulerAngles;
        string[] lastKeysPressed = new string[0];
        float lastLogTime = Time.time;

        while (isDriving)
        {
            var pressedKeys = Keyboard.current.allKeys
                .Where(k => k.isPressed)
                .Select(k => k.displayName)
                .ToArray();

            bool posChanged = Vector3.Distance(transform.position, lastPos) > 0.01f;
            bool rotChanged = Vector3.Distance(transform.rotation.eulerAngles, lastRot) > 0.1f;
            bool hasInput = pressedKeys.Length > 0;

            bool keysChanged = !pressedKeys.SequenceEqual(lastKeysPressed);

            if (keysChanged || (hasInput && Time.time - lastLogTime >= 2f) || (!hasInput && (posChanged || rotChanged) && Time.time - lastLogTime >= 2f))
            {
                if (posChanged || rotChanged || hasInput)
                {
                    Debug.Log($"[VehicleDebug] Player Pos: {transform.position}, Rotation: {transform.rotation.eulerAngles}");
                    if (hasInput)
                        Debug.Log($"[VehicleDebug] Keys Pressed: {string.Join(", ", pressedKeys)}");
                    else
                        Debug.Log("[VehicleDebug] No keys pressed");
                }
                else
                {
                    Debug.Log("[VehicleDebug] Đứng yên");
                }

                lastKeysPressed = pressedKeys;
                lastPos = transform.position;
                lastRot = transform.rotation.eulerAngles;
                lastLogTime = Time.time;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Weapon"))
        {
            IWeapon weapon = col.GetComponent<IWeapon>();
            if (weapon != null && weapon.CanPickUp)
            {
                near = col.gameObject;
                pickup.SetNearWeapon(near);
            }
        }

        VehicleTrigger vt = col.GetComponent<VehicleTrigger>();
        if (vt != null)
            currentVehicle = vt.vehicle;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Weapon") && near == col.gameObject)
        {
            near = null;
            pickup.ClearNearWeapon(col.gameObject);
        }

        VehicleTrigger vt = col.GetComponent<VehicleTrigger>();
        if (vt != null && currentVehicle == vt.vehicle)
            currentVehicle = null;
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        Health -= damage;
        if (Health <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }
}
