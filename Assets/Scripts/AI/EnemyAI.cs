using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
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

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void TransitionToState(IEnemyState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }    
}