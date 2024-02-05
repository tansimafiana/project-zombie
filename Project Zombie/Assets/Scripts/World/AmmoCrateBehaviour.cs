using UnityEngine;

public class AmmoCrateBehaviour : MonoBehaviour
{
    public AmmoSpawner parent;

    void OnTriggerEnter(Collider other) {
        Shoot player = other.GetComponentInParent<Shoot>();
        if (player) {
            GunSO equipped = player.equippedWeapon;
            player.UpdateAmmo(player.clipAmount, equipped.ammoAmount - player.clipAmount);
            parent.CountdownSpawnCrate();
            gameObject.SetActive(false);
        }
    }
}
