using UnityEngine;

public class BuildEffectPlayer : EffectPlayer
{
    public override void PlayEffect(Vector3 playPos, int averageCnt = 0)
    {
        transform.position = playPos;

        particles[0].emission.SetBurst(0, new ParticleSystem.Burst(0,
            Random.Range(averageCnt-1, averageCnt+1), 1, 0.010f));
    
        particles[0].Play();
        StartCoroutine(DestroyCoroutine());
    }
}
