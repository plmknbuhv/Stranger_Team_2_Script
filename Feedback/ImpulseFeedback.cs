using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ImpulseFeedback : Feedback
{
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    
    public override void PlayFeedback()
    {
        _impulseSource.DefaultVelocity 
            = new Vector3(Random.Range(-.3f,.3f), Random.Range(-.3f,.3f), Random.Range(-.3f,.3f)) * 3;
        _impulseSource.GenerateImpulse(0.1f);
    }
}
