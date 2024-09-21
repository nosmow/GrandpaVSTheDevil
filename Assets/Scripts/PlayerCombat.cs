using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    private DetectControls controls;

    private Animator animator;

    [SerializeField] private int hp = 100;
    [SerializeField] private Slider sliderHp;

    [SerializeField] private float coldDown = 1f;

    [SerializeField] private Slider sliderCombo;
    [SerializeField] private float coldDownCombo = 5f;

    [SerializeField] private float durationCombo = 0.78f;
    [SerializeField] private float comboBoostForce = 10f;

    [SerializeField] private float timeAtterCombo = 2f;
    private float waitingTime = 0;
    private float waitingAfterCombo;


    public bool isAttacking = false;
    public string currentAttack = "";

    private float currentColdDownCombo = 0;
    private Rigidbody rb;

    private bool isMoveCombo = false;
    private bool isHit = false;

    #region GET/SET
    
    public bool GetIsHit()
    {
        return isHit;
    }
    
    #endregion

    private void Start()
    {
        controls = GetComponent<DetectControls>();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        sliderHp.maxValue = hp;
        sliderHp.value = hp;

        sliderCombo.maxValue = coldDownCombo;
        sliderCombo.value = coldDownCombo;
    }

    private void Update()
    {
        WaitingTime();
        WaitingAfterCombo();
        CurrentColdDownCombo();

        AttackMele();
        AttackCombo(); 
    }

    #region WaitingAttacks

    private void WaitingTime()
    {
        if (waitingTime > 0)
        {
            waitingTime -= Time.deltaTime;
        }
    }

    private void WaitingAfterCombo()
    {
        if (waitingAfterCombo > 0)
        {
            waitingAfterCombo -= Time.deltaTime;
        }
    }

    private void CurrentColdDownCombo()
    {
        if (currentColdDownCombo > 0)
        {
            currentColdDownCombo -= Time.deltaTime;
        }
    }

    #endregion

    #region Attack

    private void AttackMele()
    {
        if (waitingTime <= 0 && waitingAfterCombo <= 0)
        {

            if (!isAttacking && !isHit && !GetComponent<PlayerMovement>().GetIsRolling())
            {
                if (controls.GetInputs().Attack.Attack1.triggered)
                {
                    currentAttack = "BasicAttackPlayer";
                    ExecuteAnimationAttack(0f);
                    StartCoroutine(StartAttackMele(coldDown));
                }
                else if (controls.GetInputs().Attack.Attack2.triggered)
                {
                    currentAttack = "BasicAttackPlayer";
                    ExecuteAnimationAttack(0.5f);
                    StartCoroutine(StartAttackMele(coldDown));
                }
            }
        }
    }

    private void AttackCombo()
    {
        if (waitingTime <= 0 && waitingAfterCombo <= 0 && currentColdDownCombo <= 0)
        {

            if (!isAttacking && !isHit && !GetComponent<PlayerMovement>().GetIsRolling())
            {
                if (controls.GetInputs().Attack.AttackCombo.triggered)
                {
                    currentAttack = "ComboAttackPlayer";
                    ExecuteAnimationAttack(1f);
                    StartCoroutine(StartMoveAttackCombo(durationCombo));
                    StartCoroutine(StartWaitingAfterCombo(timeAtterCombo));
                    StartCoroutine(StartAttackCombo(coldDownCombo));
                }
            }
        }

        if (isMoveCombo)
        {
            rb.MovePosition(rb.position + transform.forward * comboBoostForce * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator StartAttackMele(float time)
    {
        isAttacking = true;
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    private IEnumerator StartWaitingAfterCombo(float time)
    {
        waitingAfterCombo = timeAtterCombo;
        isAttacking = true;
        yield return new WaitForSeconds(time);
        isAttacking = false;
        waitingAfterCombo = 0;
    }

    private IEnumerator StartMoveAttackCombo(float time)
    {
        isMoveCombo = true;
        yield return new WaitForSeconds(time);
        isMoveCombo = false;
    }

    private IEnumerator StartAttackCombo(float time)
    {
        currentColdDownCombo = coldDownCombo;
        sliderCombo.value = 0;
        yield return new WaitForSeconds(time);
        currentColdDownCombo = 0;
        sliderCombo.value = coldDownCombo;
    }

    private void ExecuteAnimationAttack(float typeAttack)
    { 
        animator.SetTrigger("IsAttacking");
        animator.SetFloat("TypeAttack", typeAttack);
        waitingTime = coldDown;
    }

    private IEnumerator StartHit(float time)
    {
        isHit = true;
        yield return new WaitForSeconds(time);
        isHit = false;
    }

    #endregion

    #region Damage

    private void AnimationDamage()
    {
        animator.SetTrigger("IsHit");
        animator.ResetTrigger("IsAttacking");
        animator.ResetTrigger("CanRoll");

        StartCoroutine(StartHit(0.91f));
    }

    private void ReceivedDamage()
    {
        if (!GetComponent<PlayerMovement>().GetIsRolling())
        {
            hp -= 10;
            sliderHp.value = hp;

            AnimationDamage();
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PunchEnemy"))
        {
            ReceivedDamage();
        }

        if (other.gameObject.CompareTag("PowerUp"))
        {
            hp += 40;

            if (hp > 100)
            {
                hp = 100;
            }

            sliderHp.value = hp;
        
            Destroy(other.gameObject);
        }
    }
}
