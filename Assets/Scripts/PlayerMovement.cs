using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private DetectControls controls;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rollSpeed = 2f;
    [SerializeField] private float rollDuration = 0.7f;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private float currentHeight;
    private float currentCenter;
    private Animator animator;
    private Vector3 movInput;
    private bool isRolling;
    private Vector3 rollDirection;

    #region GET/SET
    
    public bool GetIsRolling()
    {
        return isRolling;
    }
    
    #endregion

    private void Start()
    {
        controls = GetComponent<DetectControls>();

        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        currentHeight = playerCollider.height;
        currentCenter = playerCollider.center.y;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        InputMovement();

        animator.SetFloat("MoveX", movInput.x);
        animator.SetFloat("MoveZ", movInput.z);

        if (!FindObjectOfType<PlayerCombat>().GetIsHit() && !FindObjectOfType<PlayerCombat>().isAttacking && controls.GetInputs().Move.Roll.triggered && !isRolling) // Evitar rodar si ya se está rodando
        {
            StartCoroutine(StartRoll());
        }
    }

    private void FixedUpdate()
    {
        if (!FindObjectOfType<PlayerCombat>().GetIsHit() && !FindObjectOfType<PlayerCombat>().isAttacking && !isRolling)
        {
            rb.MovePosition(rb.position + movInput * moveSpeed * Time.fixedDeltaTime);
        }
        else if (isRolling)
        {
            rb.MovePosition(rb.position + rollDirection * rollSpeed * Time.fixedDeltaTime);
        }
    }

    #region Movement

    private void InputMovement()
    {
        Vector2 input = controls.GetInputs().Move.Movement.ReadValue<Vector2>().normalized;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        movInput = (forward * input.y + right * input.x).normalized;
    }

    private IEnumerator StartRoll()
    {
        isRolling = true;      
        rollDirection = transform.forward;
        animator.SetTrigger("CanRoll");

        yield return new WaitForSeconds(0.78f); // Wait for the roll animation time
        isRolling = false;
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Ignore collision if colliding with the top of the enemy
            if (transform.position.y > collision.transform.position.y)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
        }
    }
}