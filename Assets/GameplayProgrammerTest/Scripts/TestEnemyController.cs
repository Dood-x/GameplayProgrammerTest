using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemyController : MonoBehaviour
{

    [System.Serializable] 
   public enum EnemyState
    {
        Inactive,
        Searching,
        Chasing
    }

    public float sightRadius = 10.0f;
    public float stopTime = 2f;
    public float attackpower = 10.0f;

    private Transform playerTransform;
    private NavMeshAgent navAgent;

    private EnemyState enemyState = EnemyState.Searching;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerTransform = PlayerSingleton.instance.player.transform;
        GetComponent<Rigidbody>().sleepThreshold = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch(enemyState)
        {
            case EnemyState.Chasing:
                Track();
                break;
            case EnemyState.Searching:
                Search();
                break;
            case EnemyState.Inactive:
            default:
                break;
        }
    }

    void Search()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if(distance < sightRadius)
        {
            enemyState = EnemyState.Chasing;
        }
        else
        {
            enemyState = EnemyState.Searching;
        }
    }

    public void Rest()
    {
        StartCoroutine("StopAfterAttack");
    }

    void Track()
    {
        navAgent.SetDestination(playerTransform.position);
        Search();
    }

    IEnumerator StopAfterAttack()
    {
        enemyState = EnemyState.Inactive;
        yield return new WaitForSeconds(stopTime);
        enemyState = EnemyState.Searching;
    }
}
