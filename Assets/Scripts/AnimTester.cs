// using UnityEngine;
// using System.Collections;

// public class AnimTester : MonoBehaviour
// {
//     private Animator anim;
//     private bool isAttacking = false;
//     private Quaternion originalRotation;

//     void Start()
//     {
//         anim = GetComponent<Animator>();
//         // Отключаем root motion чтобы анимация не двигала персонажа
//         anim.applyRootMotion = false;
//     }

//     void Update()
//     {
//         // Блокируем движение во время атаки
//         if (isAttacking) return;

//         // 1. Движение: Ходьба (W) и Бег (W + Shift)
//         if (Input.GetKey(KeyCode.W))
//         {
//             float speed = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;
//             anim.SetFloat("Speed", speed);
//         }
//         else
//         {
//             anim.SetFloat("Speed", 0f);
//         }

//         // 2. Физическая атака (ЛКМ)
//         if (Input.GetKeyDown(KeyCode.Mouse0))
//         {
//             StartCoroutine(PlayAttackAnimation());
//         }

//         // 3. Магическая атака (ПКМ)
//         if (Input.GetKeyDown(KeyCode.Mouse1))
//         {
//             anim.SetTrigger("AttackMagic");
//         }

//         // 4. Получение урона (H)
//         if (Input.GetKeyDown(KeyCode.H))
//         {
//             anim.SetTrigger("TakeHit");
//         }

//         // 5. Смерть (K)
//         if (Input.GetKeyDown(KeyCode.K))
//         {
//             anim.SetBool("IsDead", true);
//         }
//     }

//     IEnumerator PlayAttackAnimation()
//     {
//         isAttacking = true;
//         originalRotation = transform.rotation;
        
//         // Сохраняем текущий поворот
//         anim.SetTrigger("AttackSword");
        
//         // Ждем окончания анимации
//         yield return new WaitForSeconds(0.5f); // Подбери время под свою анимацию
        
//         // Возвращаем исходный поворот
//         transform.rotation = originalRotation;
//         isAttacking = false;
//     }
// }

// using UnityEngine;

// [RequireComponent(typeof(CharacterController))]
// [RequireComponent(typeof(Animator))]
// public class PlayerTester : MonoBehaviour
// {
//     [Header("Настройки")]
//     public float speed = 5f;
//     public float rotationSpeed = 10f;
//     public float gravity = -9.81f;

//     private CharacterController controller;
//     private Animator animator;
//     private Vector3 velocity;
//     private bool isAttacking = false;
//     private Quaternion startRotation;

//     void Start()
//     {
//         controller = GetComponent<CharacterController>();
//         animator = GetComponent<Animator>();
        
//         // ОТКЛЮЧАЕМ ROOT MOTION ПРОГРАММНО (на всякий случай)
//         animator.applyRootMotion = false;
//     }

//     void Update()
//     {
//         // Если атакуем — блокируем движение и возвращаем поворот
//         if (isAttacking)
//         {
//             transform.rotation = startRotation;
//             return;
//         }

//         HandleMovement();
//         HandleAttack();
//         ApplyGravity();
//     }

//     void HandleMovement()
//     {
//         float h = Input.GetAxis("Horizontal"); // A/D
//         float v = Input.GetAxis("Vertical");   // W/S

//         Vector3 move = new Vector3(-h, 0, -v).normalized;

//         // Если есть ввод движения
//         if (move.magnitude >= 0.1f)
//         {
//             // Бег если зажат Shift
//             float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 1.5f : speed;

//             // Двигаем персонажа
//             controller.Move(move * currentSpeed * Time.deltaTime);

//             // Поворачиваем туда, куда идем
//             Quaternion targetRot = Quaternion.LookRotation(move);
//             transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

//             // Анимация: 0.5 = шаг, 1 = бег
//             animator.SetFloat("Speed", Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f);
//         }
//         else
//         {
//             // Стоим
//             animator.SetFloat("Speed", 0f);
//         }
//     }

//     void HandleAttack()
//     {
//         if (Input.GetMouseButtonDown(0)) // ЛКМ
//         {
//             isAttacking = true;
//             startRotation = transform.rotation; // Запоминаем поворот ДО атаки
//             animator.SetTrigger("AttackSword");
            
//             // Разблокируем атаку через 0.5 сек (подбери под длину анимации)
//             Invoke(nameof(StopAttack), 0.5f);
//         }
//     }

//     void StopAttack()
//     {
//         isAttacking = false;
//     }

//     void ApplyGravity()
//     {
//         if (controller.isGrounded && velocity.y < 0)
//         {
//             velocity.y = -2f; // Прижимаем к земле
//         }
//         else
//         {
//             velocity.y += gravity * Time.deltaTime;
//         }

//         controller.Move(velocity * Time.deltaTime);
//     }
// }


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