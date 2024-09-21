using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private int hp = 100;
    [SerializeField] private Slider sliderHp;
    [SerializeField] private int maxHitsBeforeRetreat = 2;
    [SerializeField] private GameObject prefabPowerUp;

    private EnemyAI enemyAI;
    private int hitCounter = 0;

    private Transform body;

    private bool isDied = false;

    private void Start()
    {
        sliderHp.maxValue = hp;
        sliderHp.value = hp;
    
        enemyAI = GetComponent<EnemyAI>();

        body = GetComponentInChildren<Enemy>().transform;
    }

    private void Update()
    {
        if (hp <= 0 && !isDied)
        {
            StartCoroutine(Died());
        }

        if (isDied)
        {
            if (body.position.y > 0)
            {
                body.position = new Vector3(body.position.x, -Time.fixedDeltaTime, body.position.z);
            }
        }
    }

    private void ReceiveDamage(int damage)
    {
        hp -= damage;
        sliderHp.value = hp;
        enemyAI.animator.SetTrigger("IsHit");
    }

    // Method to record player's strokes
    public void TakeHit(int damage)
    {
        if (hp > 0)
        {
            hitCounter++;

            ReceiveDamage(damage);

            if (hitCounter >= maxHitsBeforeRetreat)
            {
                enemyAI.TransitionToState(new RetreatState()); // Back off after two hits
            }
        }
    }

    public void ResetHitCounter()
    {
        hitCounter = 0;
    }

    private IEnumerator Died()
    {
        isDied = true;
     
        enemyAI.animator.ResetTrigger("IsHit");
        enemyAI.animator.ResetTrigger("IsAttack");
        enemyAI.animator.SetTrigger("Died");

        enemyAI.enabled = false;
        sliderHp.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        Instantiate(prefabPowerUp, new Vector3(body.position.x, 0.8f, body.position.z), Quaternion.identity);
        Destroy(gameObject);
    }
}
