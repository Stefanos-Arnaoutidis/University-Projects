using UnityEngine;
using UnityEngine.AI;

public class EnemyCopAI : MonoBehaviour
{
    [Header("Στόχος")]
    public Transform player;

    [Header("Αποστάσεις & Ζημιά")]
    public float chaseDistance = 15f;
    public float attackDistance = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;

    [Header("Περιπολία (Patrol)")]
    public float patrolRadius = 10f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    [Header("Ηχητικά Εφέ (SFX)")]
    public AudioSource copAudio;
    public AudioClip sfxCopAlert;

    [Header("Οπτικό Εφέ (Visual Feedback)")]
    public GameObject alertIndicator; // Οπτική ένδειξη (θαυμαστικό) κατά τον εντοπισμό.

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;
    private Vector3 startingPosition;

    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        startingPosition = transform.position;

        if (copAudio == null) copAudio = GetComponent<AudioSource>();

        // Αρχική απόκρυψη της οπτικής ένδειξης.
        if (alertIndicator != null) alertIndicator.SetActive(false);

        PickNewPatrolPoint();
    }

    void Update()
    {
        // Ενημέρωση της παραμέτρου ταχύτητας στο Animator βάσει της κίνησης του NavMeshAgent.
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Εφαρμογή τεχνικής Billboard: Η οπτική ένδειξη στρέφεται διαρκώς προς την κύρια κάμερα.
        if (alertIndicator != null && alertIndicator.activeSelf && Camera.main != null)
        {
            alertIndicator.transform.rotation = Camera.main.transform.rotation;
        }

        // Διακοπή εκτέλεσης σε περίπτωση απουσίας αναφοράς στον παίκτη.
        if (player == null) return;

        // Υπολογισμός της απόστασης μεταξύ του πράκτορα και του παίκτη.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            // Κατάσταση Επίθεσης (Attack State).
            agent.speed = chaseSpeed;
            agent.SetDestination(transform.position);

            // Έλεγχος χρονικού ορίου (cooldown) μεταξύ των επιθέσεων.
            if (Time.time - lastAttackTime > attackCooldown)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            // Κατάσταση Καταδίωξης (Chase State).
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);

            if (!isChasing)
            {
                isChasing = true;

                // Ειδοποίηση του διαχειριστή μουσικής για την έναρξη της καταδίωξης.
                if (MusicManager.instance != null) MusicManager.instance.CopStartedChasing();

                // Αναπαραγωγή ηχητικού εφέ εντοπισμού.
                if (copAudio != null && sfxCopAlert != null)
                {
                    copAudio.PlayOneShot(sfxCopAlert);
                }

                // Ενεργοποίηση της οπτικής ένδειξης.
                if (alertIndicator != null) alertIndicator.SetActive(true);
            }
        }
        else
        {
            // Κατάσταση Περιπολίας (Patrol State).
            agent.speed = patrolSpeed;

            if (isChasing)
            {
                isChasing = false;

                // Ειδοποίηση του διαχειριστή μουσικής για τη λήξη της καταδίωξης.
                if (MusicManager.instance != null) MusicManager.instance.CopStoppedChasing();

                // Απενεργοποίηση της οπτικής ένδειξης κατά την απώλεια του στόχου.
                if (alertIndicator != null) alertIndicator.SetActive(false);
            }

            // Έλεγχος άφιξης στο τρέχον σημείο περιπολίας και επιλογή νέου προορισμού.
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                PickNewPatrolPoint();
            }
        }
    }

    void PickNewPatrolPoint()
    {
        // Επιλογή τυχαίου σημείου περιπολίας εντός της καθορισμένης ακτίνας.
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startingPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void ResetEnemyPosition()
    {
        // Επαναφορά του πράκτορα στην αρχική του θέση.
        if (agent != null)
        {
            agent.Warp(startingPosition);

            if (isChasing)
            {
                isChasing = false;
                if (MusicManager.instance != null) MusicManager.instance.CopStoppedChasing();

                // Απενεργοποίηση της οπτικής ένδειξης κατά την επαναφορά.
                if (alertIndicator != null) alertIndicator.SetActive(false);
            }

            PickNewPatrolPoint();
        }
    }
}