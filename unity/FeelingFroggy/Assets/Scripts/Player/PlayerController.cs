using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public PlayerMovementController Movement { get; private set; }

    public PlayerCameraController Camera { get; private set; }

    public PlayerInput Input { get; private set; }

    [Header("Health Settings")]
    public HealthBarController healthBar;

    public float initialHealth = 100;
    public float maxHealth = 100;
    public float healthRegenAmount = 0.1f;
    public float healthRegenDelayAfterHeal = 0.05f;
    public float healthRegenDelayAfterDamage = 10;

    public float CurrentHealth { get; private set; } = 1; // needs to be greater than 0 to allow for initial heal

    public bool IsDead => CurrentHealth <= 0;

    public float LastDamageTime { get; private set; }

    public float LastRegenTime { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovementController>();
        Camera = GetComponent<PlayerCameraController>();
        Input = GetComponent<PlayerInput>();

        Instance = this;
    }

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
        SetHealth(initialHealth);
    }

    private void Update()
    {
        RegenHealth();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [UsedImplicitly]
    private void OnJump(InputValue jumpValue)
    {
        if (jumpValue.isPressed && GameController.Instance.IsInDialogue)
        {
            GameController.Instance.SkipDialogueLine();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EnemyHitbox"))
        {
            var enemyDamageController = other.GetComponentInParent<EnemyDamageController>();

            enemyDamageController?.HitPlayer();
        }
    }

    public void SetHealth(float health)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth = Math.Clamp(health, 0, maxHealth);

        healthBar.SetHealth(health);

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    public void Damage(float damage)
    {
        if (damage <= 0)
        {
            return;
        }

        LastDamageTime = Time.time;
        SetHealth(CurrentHealth - damage);
    }

    public void Heal(float health)
    {
        if (health <= 0)
        {
            return;
        }

        SetHealth(CurrentHealth + health);
    }

    private void Die()
    {
        LevelController.Instance.TriggerLost();
    }

    private void RegenHealth()
    {
        // Do not regen if the player has taken damage recently
        if (Time.time - LastDamageTime < healthRegenDelayAfterDamage)
        {
            return;
        }

        // Do not regen if the player has healed recently
        if (Time.time - LastRegenTime < healthRegenDelayAfterHeal)
        {
            return;
        }

        Heal(healthRegenAmount);
        LastRegenTime = Time.time;
    }
}