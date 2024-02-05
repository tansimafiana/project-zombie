using System.Collections;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    private PlayerStats p_stats;
    private TrapSO trapType;
    private bool activated = false;

    public void Init(PlayerStats playerWhoDeployed, TrapSO trapType) {
        this.p_stats = playerWhoDeployed;
        this.trapType = trapType;
    }

    void OnTriggerEnter(Collider col) {
        if (col.transform.parent != null) {
            EnemyNavmesh mov = col.GetComponentInParent<EnemyNavmesh>();
            if (mov && !activated) {
                activated = true;

                bool killingHit = false;
                mov.GetComponent<EnemyStats>().Damage(trapType.damage, out killingHit);
                if (killingHit)  { GetComponent<PlayerStats>().ZombieKilledCount(mov.name);  return; }

                mov.StopAllCoroutines();
                StartCoroutine(mov.Stun(trapType.stunPercentSpeed, trapType.stunTime));
            }
        }
    }

    IEnumerator Stun(EnemyNavmesh mov) {
        bool killingHit = false;
        mov.GetComponent<EnemyStats>().Damage(trapType.damage, out killingHit);
        if (killingHit)  { p_stats.ZombieKilledCount(mov.name);  yield break; }
        
        mov.CurrSpeed *= trapType.stunPercentSpeed;
        yield return new WaitForSeconds(trapType.stunTime);
        if (mov != null)  { mov.CurrSpeed = mov.moveSpeed; }

        Destroy(gameObject);
    }
}
