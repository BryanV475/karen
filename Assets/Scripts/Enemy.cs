using UnityEngine;
using Panda;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform[] Enemies;
    private Transform Target;
    private float Velocity = 5f;
    [SerializeField] private int Energy = 5;
    private float minDistance = 5f;
    private float attackDistance = 2f;
    private Rigidbody rb;
    public Transform[] patrolPointObjects;
    private Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;
    public Transform safezone;
    private Vector3 restPosition;
    private bool isResting = false;
    public Animator animator;
    public float pushForce = 5f;
    public bool isAttacking = false;


    float delaySeconds = 1f;
    float startTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Extract patrol point positions from patrolPointObjects
        patrolPoints = new Vector3[patrolPointObjects.Length];
        for (int i = 0; i < patrolPointObjects.Length; i++)
        {
            patrolPoints[i] = patrolPointObjects[i].position;
        }

        restPosition = safezone.position;
    }
    private void Update()
    {
        if (Time.time >= startTime + delaySeconds)
        {
            isAttacking = false;
        }
    }

    [Task]
    void EnemyClose()
    {
        foreach (Transform enemy in Enemies)
        {
            if (Vector3.Distance(transform.position, enemy.position) <= minDistance)
            {
                Target = enemy;
                Task.current.Succeed();
                return;
            }
        }
        Task.current.Fail();
    }

    [Task]
    void GoEnemy()
    {
        if (Target != null && Energy >= 5)
        {
            Vector3 targetPosition = Target.position;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            Vector3 newVelocity = moveDirection * Velocity;

            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
            transform.LookAt(targetPosition);

            // Set animator parameters for movement
            float vertical = Mathf.Clamp(newVelocity.z / Velocity, -1f, 1f);
            float horizontal = Mathf.Clamp(newVelocity.x / Velocity, -1f, 1f);
            animator.SetFloat("vertical", vertical);
            animator.SetFloat("horizontal", horizontal);

            // If close enough, proceed to attack
            if (Vector3.Distance(transform.position, targetPosition) <= attackDistance)
            {
                isAttacking = true;
                Task.current.Succeed();
            }
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void Attack()
    {
        startTime = Time.time;

        if (Target != null && Energy >= 2 && isAttacking)
        {
            Vector3 targetPosition = Target.position;
            

            // Perform attack action here
            animator.SetTrigger("isPunching");

            Energy = 0;

            // Stop movement during attack
            rb.velocity = Vector3.zero;

            // Apply a force to push the target
            Rigidbody targetRb = Target.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                Vector3 pushDirection = targetPosition - transform.position;
                pushDirection.y = 0f; // Ensure no vertical push
                targetRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            }
            
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }


    [Task]
    void Rest()
    {
        if (Energy < 5 && !isResting && !isAttacking)
        {
            Vector3 direction = restPosition - transform.position;
            Vector3 moveDirection = direction.normalized;
            Vector3 newVelocity = moveDirection * Velocity;

            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
            transform.LookAt(restPosition);

            // Set animator parameters for resting
            // Set animator parameters for movement
            float vertical = Mathf.Clamp(newVelocity.z / Velocity, -1f, 1f);
            float horizontal = Mathf.Clamp(newVelocity.x / Velocity, -1f, 1f);
            animator.SetFloat("vertical", vertical);
            animator.SetFloat("horizontal", horizontal);

            if (Vector3.Distance(transform.position, restPosition) < 0.1f)
            {
                isResting = true;
                Energy += 1;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                // Set animator parameters for resting
                animator.SetFloat("vertical", 0f);
                animator.SetFloat("horizontal", 0f);
            }
        }
        else
        {
            isResting = false;
            rb.velocity = Vector3.zero; // Stop the enemy's movement
        }

        if (Energy == 5)
        {
            isResting = false;
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void Patrol()
    {
        if (isResting)
        {
            Task.current.Fail();
            return;
        }

        Vector3 targetPosition = patrolPoints[currentPatrolIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        Vector3 newVelocity = moveDirection * Velocity;

        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        transform.LookAt(targetPosition);

        // Set animator parameters for movement
        float vertical = Mathf.Clamp(newVelocity.z / Velocity, -1f, 1f);
        float horizontal = Mathf.Clamp(newVelocity.x / Velocity, -1f, 1f);
        animator.SetFloat("vertical", vertical);
        animator.SetFloat("horizontal", horizontal);

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        Task.current.Succeed();
    }

}
