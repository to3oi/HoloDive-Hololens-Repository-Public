using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(DefaultObserverEventHandler))]
public class BombDefusingDeviceEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _warningParticleSystem;
    void Awake()
    {
        var handler = GetComponent<DefaultObserverEventHandler>();
        handler.OnTargetFound.AddListener(WarningPlayEffect);
    }

    private bool isPlayedEffct = false;
    private void WarningPlayEffect()
    {
        if(isPlayedEffct){return;}

        isPlayedEffct = true;
        Instantiate(_warningParticleSystem, Camera.main.transform.position + Vector3.down * 0.5f ,quaternion.identity);
    }
}