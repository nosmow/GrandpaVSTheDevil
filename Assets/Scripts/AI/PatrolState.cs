using UnityEngine;

public class PatrolState : IEnemyState
{
    private int index = 0;

    public void EnterState(EnemyAI enemy)
    {
        MoveToNextPatrolPoint(enemy);
    }

    public void UpdateState(EnemyAI enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.GetPlayer().position) <= enemy.GetDetectionRange())
        {
            enemy.TransitionToState(new ChaseState());
        }
        else if (!enemy.agent.pathPending &&
                 enemy.agent.remainingDistance < 0.5f)
        {
            MoveToNextPatrolPoint(enemy);
        }
    }

    private void MoveToNextPatrolPoint(EnemyAI enemy)
    {
        if (enemy.GetPatrolPoints().Length == 0)
            return;

        index = Random.Range(0, enemy.GetPatrolPoints().Length);
        enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = enemy.GetPatrolPoints()[index].position;

        enemy.agent.acceleration = 0.5f;
    }

}