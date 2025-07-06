using System.Collections;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    protected ParticleSystem[] particles;
    protected WaitForSeconds waitForDuration;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        waitForDuration = new WaitForSeconds(particles[0].main.duration + 3f);
    }

    public virtual void PlayEffect(Vector3 playPos, int averageCnt = 0)
    {
        transform.position = playPos;

        foreach (var particle in particles)
            particle.Play();
        
        StartCoroutine(DestroyCoroutine()); 
    }

    protected IEnumerator DestroyCoroutine()
    {
        yield return waitForDuration;
        Destroy(gameObject);
    }
}
