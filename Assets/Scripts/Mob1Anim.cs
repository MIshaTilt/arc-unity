using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Mob1AnimatorTester : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    
    // Чтобы инвертировать управление, если моб ходит задом (как у героя было)
    [Header("Debug")]
    public bool invertMovement = false; 

    private CharacterController controller;
    private Animator animator;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        // ВАЖНО: Отключаем Root Motion, чтобы анимация не ломала позицию
        animator.applyRootMotion = false;
    }

    void Update()
    {
        // Если умер — блокируем всё
        if (isDead) return;

        HandleMovement();
        HandleInput();
    }

    // void HandleMovement()
    // {
    //     float h = Input.GetAxis("Horizontal"); // A/D
    //     float v = Input.GetAxis("Vertical");   // W/S

    //     // Если нужно инвертировать (если моб ходит задом)
    //     if (invertMovement)
    //     {
    //         h = -h;
    //         v = -v;
    //     }

    //     Vector3 move = new Vector3(h, 0, v).normalized;

    //     if (move.magnitude >= 0.1f)
    //     {
    //         // Движение
    //         controller.Move(move * moveSpeed * Time.deltaTime);
            
    //         // Поворот
    //         Quaternion targetRotation = Quaternion.LookRotation(move);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
    //         // Анимация
    //         animator.SetFloat("Speed", 1f);
    //     }
    //     else
    //     {
    //         animator.SetFloat("Speed", 0f);
    //     }
    // }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (invertMovement)
        {
            h = -h;
            v = -v;
        }

        Vector3 move = new Vector3(h, 0, v).normalized;

        if (move.magnitude >= 0.1f)
        {
            // Движение ТОЛЬКО по горизонтали (Y = 0)
            Vector3 moveDirection = new Vector3(move.x, 0, move.z);
            
            // Принудительно держим на земле
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            controller.Move(movement);
            
            // Поворот
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Анимация
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }



    void HandleInput()
    {
        // Атака (Пробел)
        if (Input.GetMouseButtonDown(1))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
                Invoke(nameof(StopAttack), 0.5f); // Подбери время под анимацию
            }
        }

        // Получение урона (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("TakeHit");
        }

        // Смерть (K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }

    void StopAttack()
    {
        isAttacking = false;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("DeathTrigger");
        // Скрипт продолжит работать, но Update заблокирован проверкой isDead
    }
}