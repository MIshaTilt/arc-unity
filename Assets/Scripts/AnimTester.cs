
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerTester : MonoBehaviour
{
    [Header("Настройки")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isAttacking = false;
    private bool isDead = false; 
    private Quaternion startRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    void Update()
    {
        // Если умер блокируем всё кроме анимации смерти
        if (isDead)
        {
            return;
        }

        if (isAttacking)
        {
            transform.rotation = startRotation;
            return;
        }

        HandleMovement();
        HandleInput();
        ApplyGravity();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 move = new Vector3(-h, 0, -v).normalized;

        if (move.magnitude >= 0.1f)
        {
            bool running = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = running ? speed * 1.5f : speed;
            
            controller.Move(move * currentSpeed * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            animator.SetFloat("Speed", running ? 1f : 0.5f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    void HandleInput()
    {
        // Атака (ЛКМ)
        if (Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }

        // Магия (ПКМ)
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("AttackMagic");
        }

        // Урон (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("TakeHit");
        }

        // СМЕРТЬ (K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        startRotation = transform.rotation;
        animator.SetTrigger("AttackSword");
        Invoke(nameof(StopAttack), 0.5f);
    }

    void StopAttack()
    {
        isAttacking = false;
    }

   
    void Die()
    {
        // if (isDead) return; 
        // isDead = true; 
        // animator.SetBool("IsDead", true); 
        if (isDead) return;
    
        isDead = true;
        animator.SetTrigger("DeathTrigger"); // Trigger автоматически сбрасывается
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += -9.81f * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}