using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsAttachToUI : MonoBehaviour
{
    void Start() {
        // Attaching out health slider to the [PlayerStats]
        PlayerStats stats = DirectorZombieSpawner.Instance.playerTransform.GetComponent<PlayerStats>();

        stats.healthSlider = GetComponent<Slider>();
        stats.fillImg = stats.healthSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        
        stats.healthSlider.maxValue = stats.maxHealth;
        stats.healthSlider.value = stats.maxHealth;
    }
}
