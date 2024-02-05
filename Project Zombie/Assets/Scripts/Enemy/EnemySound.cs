using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnemySound : MonoBehaviour
{
    [SerializeField] private GameObject audioSourceGO;
    [SerializeField] private float volume = 1f;
    [Space]
    private bool playedSound = false;
    [HideInInspector] public bool isDead = false;
    [SerializeField] private float minTime = 3f;
    [SerializeField] private float maxTime = 5f;

    [SerializeField] private AudioClip groanSound;
    [HideInInspector] public AudioClip deathSound;
    private AudioSource ASgroan;
    [HideInInspector] public AudioSource ASdeath;


    void Start() {
        ASgroan = audioSourceGO.AddComponent<AudioSource>();
        ASdeath = audioSourceGO.AddComponent<AudioSource>();
        ASgroan.clip = groanSound;
        ASdeath.clip = deathSound;

        ASgroan.volume = volume;
        ASdeath.volume = volume;

        ASgroan.spatialBlend = 0.5f;
        ASdeath.spatialBlend = 0.5f;
    }

    void Update() {
        if (!playedSound && !isDead) {
            StartCoroutine(Groan());
        }
    }

    IEnumerator Groan() {
        ASgroan.PlayOneShot(groanSound);
        playedSound = true;

        float timeToWait = Random.Range(minTime, maxTime);
        yield return new WaitForSeconds(timeToWait);
        playedSound = false;
    }
}
