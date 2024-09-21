using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        enemy.agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemy.agent.speed = 3.5f;
        enemy.agent.acceleration = 4f;
    }

    public void UpdateState(EnemyAI enemy)
    {
        enemy.transform.LookAt(enemy.GetPlayer().transform.position);

        enemy.agent.acceleration = 4f;
        enemy.agent.destination = enemy.GetPlayer().position;

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.GetPlayer().position);
        if (distanceToPlayer <= enemy.GetAttackRange())
        {
            enemy.TransitionToState(new AttackState());
        }
        else if (distanceToPlayer > enemy.GetDetectionRange())
        {
            enemy.TransitionToState(new PatrolState());
        }
    }
}