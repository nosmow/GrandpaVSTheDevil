using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemyAI : MonoBehaviour
{
    public Transform player;           // Referencia al jugador
    public float detectionRange = 15f; // Rango de detección del jugador
    public float attackRange = 5f;     // Rango de ataque
    public float flyingHeight = 10f;   // Altura a la que debe volar el enemigo
    public float verticalSpeed = 2f;   // Velocidad de ajuste vertical
    public float patrolSpeed = 3.5f;   // Velocidad de patrulla
    public float chaseSpeed = 5f;      // Velocidad de persecución

    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private EnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Patrol;
        agent.speed = patrolSpeed;

        // Desactiva la autoasignación de altura (porque volamos)
        agent.updatePosition = true;  // Permite que el agente navegue horizontalmente
        agent.updateRotation = true;  // Que el NavMesh controle la rotación
        agent.updateUpAxis = false;   // No controla la altura (volamos)
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
        }

        // Asegurar que el enemigo esté siempre a la altura correcta
        AdjustFlyingHeight();
    }

    void Patrol()
    {
        // Aquí iría la lógica de patrulla. Puedes usar puntos de patrulla o volar en una dirección.

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
            agent.speed = chaseSpeed;
        }
    }

    void ChasePlayer()
    {
        // Establecer la posición destino del jugador, pero solo actualizar en los ejes X-Z
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        agent.SetDestination(playerPos);

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Patrol;
            agent.speed = patrolSpeed;
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Atacando al jugador!");

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
        }
    }

    void AdjustFlyingHeight()
    {
        // Ajustar la altura (eje Y) para que el enemigo siempre esté volando a la altura correcta
        Vector3 position = transform.position;
        if (Mathf.Abs(position.y - flyingHeight) > 0.1f)
        {
            float newY = Mathf.Lerp(position.y, flyingHeight, verticalSpeed * Time.deltaTime);
            transform.position = new Vector3(position.x, newY, position.z);
        }
    }
}
