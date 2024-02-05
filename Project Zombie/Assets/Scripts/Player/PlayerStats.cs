using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : NetworkBehaviour
{
    private TextMeshProUGUI tallyField;

    private bool isLocked = false;

    [HideInInspector] public float health;
    public float maxHealth = 10f;
    public Slider healthSlider;
    [SerializeField] private Gradient healthGradient;
    public Image fillImg;

    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private float deathRotTime = 1f;
    private float currRotTime;

    private int _commonsKilledInMatch = 0;
    public int CommonsKilledInMatch {
        get { return _commonsKilledInMatch; }
        set { _commonsKilledInMatch = value; }
    }
    private int _fastsKilledInMatch = 0;
    public int FastsKilledInMatch {
        get { return _fastsKilledInMatch; }
        set { _fastsKilledInMatch = value; }
    }
    private int _brutesKilledInMatch = 0;
    public int BrutesKilledInMatch {
        get { return _brutesKilledInMatch; }
        set { _brutesKilledInMatch = value; }
    }


    void Start() {
        if (!IsOwner)  return;

        health = maxHealth;

        FindAndSpawn();
        SceneChanger.IsSingleplayer += DisableMultiplayer;

        DirectorZombieSpawner.Instance.playerTransform = transform;         // TODO: Change this for when adding multiplayer

        Destroy(GetComponent<OwnerNetworkAnimator>());
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            isLocked = !isLocked;

            if (isLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Debug.Log("Cursor Locked!");
            } else {
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("Cursor Unlocked!");
            }
        }


    }

    public void Damage(float amount) {
        health -= amount;
        healthSlider.value = health;

        fillImg.color = healthGradient.Evaluate(health / maxHealth);

        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        RecolorModel(Color.red);
        StartCoroutine(RotateIfDead(deathRotTime));

        PlayerMovement m_p = GetComponent<PlayerMovement>();
        m_p.animator.SetBool("isRunning", false);
        m_p.enabled = false;
    }

    IEnumerator RotateIfDead(float timeToRot) {
        Transform t_model = transform.Find("Model");

        for (float i = 0f; i < timeToRot; i += Time.deltaTime) {
            yield return null;
            t_model.localEulerAngles += (90f * Time.deltaTime / timeToRot) * Vector3.forward;
        }
    }

    void RecolorModel(Color color) {
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.materials[0].SetColor("_Color", color);
    }

    public void ZombieKilledCount(string zombieName) {
        if      (zombieName == "Common")  { CommonsKilledInMatch++; }
        else if (zombieName == "Fast")    { FastsKilledInMatch++; }
        else if (zombieName == "Brute")   { BrutesKilledInMatch++; }
        else                              { CommonsKilledInMatch++; }
    }

    void FindAndSpawn() {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        if (spawner) {
            transform.position = spawner.transform.position;
            spawner.SpawnProtocol();
        }
    }

    void DisableMultiplayer() {
        Destroy(GetComponent<OwnerNetworkAnimator>());
    }

    void OnDisable() {
        SceneChanger.IsSingleplayer -= DisableMultiplayer;
    }
}
