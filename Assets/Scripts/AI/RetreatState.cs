using System.Collections;
using UnityEngine;

public class RetreatState : IEnemyState
{
    private Vector3 retreatDirection;
    private float retreatDistance = 5f;
    private float retreatSpeed = 5f;

    private bool retreatEnabled = true;
    private float waitingTime = 3f;
    private float currentTime = 0;

    public void EnterState(EnemyAI enemy)
    {
        // Calculate the back direction (away from the player)
        retreatDirection = (enemy.transform.position - enemy.GetPlayer().position).normalized;

        enemy.animator.ResetTrigger("IsHit");
        enemy.animator.ResetTrigger("IsAttack");
        enemy.animator.SetTrigger("IsNo");

        Debug.Log("Retrocediendo...");
        enemy.agent.isStopped = false;  // Resume movement
    }

    public void UpdateState(EnemyAI enemy)
    {
        enemy.transform.LookAt(enemy.GetPlayer().transform.position);

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        // Move the enemy in the retreat direction
        enemy.agent.acceleration = 4f;
        enemy.agent.Move(retreatDirection * retreatSpeed * Time.deltaTime);

        if (retreatEnabled)
        {
            currentTime = waitingTime;
            retreatEnabled = false;
        }

        if (!retreatEnabled && currentTime <= 0)
        {
            enemy.GetComponent<EnemyCombat>().ResetHitCounter();  // Reset the hit counter
            enemy.TransitionToState(new PatrolState()); // Switch to patrol
        }
    }
}
