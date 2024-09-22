using UnityEngine;

public class AttackState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        enemy.agent.acceleration = 4f;
        enemy.agent.isStopped = false;  // Permitir que el enemigo se mueva un poco si es necesario
        // Iniciar animación de ataque o acciones de ataque
        Debug.Log("Iniciando ataque al jugador");
        enemy.GetComponent<EnemyCombat>().SoundPunch();
    }

    public void UpdateState(EnemyAI enemy)
    {  
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.GetPlayer().position);

        // Si el jugador está fuera del rango de ataque pero dentro del rango de detección, perseguirlo
        if (distanceToPlayer > enemy.GetAttackRange() && distanceToPlayer <= enemy.GetDetectionRange())
        {
            enemy.TransitionToState(new ChaseState());
        }
        // Si el jugador se aleja fuera del rango de detección, volver a patrullar
        else if (distanceToPlayer > enemy.GetDetectionRange())
        {
            enemy.TransitionToState(new PatrolState());
        }
        else
        {
            Vector3 targetPosition = enemy.GetPlayer().transform.position;
            targetPosition.y = enemy.transform.position.y; // Mantener la altura del enemigo
            enemy.transform.LookAt(targetPosition);

            enemy.animator.SetTrigger("IsAttack");
            
            //var enemyCombat = enemy.GetComponent<EnemyCombat>();
            //enemyCombat.audioSource.PlayOneShot(enemyCombat.GetClipPunch(), 0.5f);
        }

    }
}
