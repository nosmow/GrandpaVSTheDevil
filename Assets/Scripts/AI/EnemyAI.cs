using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool isPlayerInRange = false;
    public bool isDead = false;

    private IEnemyState currentState;
    private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    
    public NavMeshAgent agent;
    public Animator animator;

    #region GET/SET
    
    public Transform GetPlayer()
    {
        return player;
    }

    public Transform[] GetPatrolPoints()
    {
        return patrolPoints;
    }

    public float GetDetectionRange()
    {
        return detectionRange;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    #endregion


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        currentState = new PatrolState();
    }

    private bool DistaceToPlayerInRange()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        return distance <= detectionRange && !isPlayerInRange;
    }

    private bool DistanceToPlayerAfterRange()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        return distance > detectionRange && isPlayerInRange;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (DistaceToPlayerInRange())
        {
            isPlayerInRange = true;
            AudioManager.Instance.StartCombatSound();
        }
        else if (DistanceToPlayerAfterRange())
        {
            isPlayerInRange = false;
            AudioManager.Instance.StartBaseSound();
        }


        currentState.UpdateState(this);

    }

    public void Dead()
    {
        AudioManager.Instance.StartBaseSound();
    }

    public void TransitionToState(IEnemyState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }    
}