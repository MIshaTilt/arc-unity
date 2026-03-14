using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    private Vector3 moveDirection;
    
    [Header("Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isAttacking) return;

        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical"); // W/S

        // Определяем скорость
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Направление движения
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Двигаем персонажа
        if (moveDirection.magnitude >= 0.1f)
        {
            // Поворот в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                Time.deltaTime * rotationSpeed);

            // Движение
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            // Устанавливаем скорость для анимации
            anim.SetFloat("Speed", isRunning ? 1f : 0.5f);
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }

        // Гравитация
        controller.Move(Vector3.up * gravity * Time.deltaTime);

        // Атаки
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(PlayAttack());
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            anim.SetTrigger("AttackMagic");
        }
    }

    IEnumerator PlayAttack()
    {
        isAttacking = true;
        anim.SetTrigger("AttackSword");
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
}