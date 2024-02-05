using UnityEngine;

public class AwakeParticleReset : MonoBehaviour, IPooledObject
{
    public new ParticleSystem particleSystem;
    public void OnObjectSpawn() {
        particleSystem.Play();
    }
}
