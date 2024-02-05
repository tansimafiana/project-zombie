using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    private float maxHealth = 5f;
    private float health;
    private float speed;
    [SerializeField] private float damage = 4f;


    private EnemyHealthUI enemyHealthUI;
    private Transform zombieModel;
    private Rigidbody rb;

    bool isDead = false;        // For eliminating multi-death on the same target


    void Start() {
        if (transform.TryGetComponent(out EnemyHealthUI comp)) {
            enemyHealthUI = comp;
        } else {
            Debug.Log("[EnemyStats] 'enemyHealthUI' is NULL");
        }

        health = maxHealth;
        enemyHealthUI.SetSlider(maxHealth);

        zombieModel = transform.Find("Model").GetChild(0);

        rb = GetComponent<Rigidbody>();
    }

    public void Damage(float damage, out bool killingHit, float minPushbackForce = -4000f, float maxPushbackForce = 4000f, Vector3 pushbackDir = new Vector3()) {
        health -= damage;
        killingHit = false;
 
        if (health <= 0f && !isDead) {
            isDead = true;
            killingHit = true;

            pushbackDir = (pushbackDir == Vector3.zero) ? Vector3.forward : pushbackDir;
            GetComponent<EnemyDeath>().DeathProtocol(minPushbackForce, maxPushbackForce, pushbackDir);
        }

        enemyHealthUI.UpdateSlider(health, !killingHit);
    }


    void OnTriggerEnter(Collider col) {
        PlayerStats pl_stats = null;
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            // Damage the player
            try {
                pl_stats = col.gameObject.GetComponentInParent<PlayerStats>();
                pl_stats.Damage(damage);
            } catch {}              // TODO: FIX this messy way of checking for player
        }
    }

    public void UpdateStats(float delayAmount, float newHealth, float newSpeed, float newDamage) {
        StartCoroutine(SetValues(delayAmount, newHealth, newSpeed, newDamage));
    }

    IEnumerator SetValues(float delay, float newHealth, float newSpeed, float newDamage) {
        yield return new WaitForSeconds(delay);
        
        this.maxHealth = newHealth;
        this.health = newHealth;
        this.speed = newSpeed;
        this.damage = newDamage;

        enemyHealthUI.SetSlider(newHealth);
        //GetComponent<NavMeshAgent>().speed = newSpeed;
        GetComponent<EnemyNavmesh>().CurrSpeed = newSpeed;
    }
}
