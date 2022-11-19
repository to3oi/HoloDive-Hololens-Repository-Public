using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DecalProjector))]
public class TimeChangeUI : MonoBehaviour
{
    [SerializeField, Header("未来のテクスチャ")] private Texture featureTexture;
    [SerializeField, Header("過去のテクスチャ")] private Texture pastTexture;

    private Material _decalMaterial;
    private DecalProjector _decalProjector;

    void Start()
    {
        _decalProjector = GetComponent<DecalProjector>();
        //MeshRenderer meshRenderer = GetComponent<MeshRenderer>(); 
        _decalMaterial = new Material(_decalProjector.material);
        _decalProjector.material = _decalMaterial;
        
        //set decal texture
        UpdateDecalTexture(TimeAxisManager.Instance.Axis);

        TimeAxisManager.Instance.TimeAxisObserver.Subscribe(axis => { UpdateDecalTexture(axis); }).AddTo(this);
    }

    void UpdateDecalTexture(TimeAxisManager.axis axis)
    {
        Debug.Log("Change Decal");
        switch (axis)
        {
            case TimeAxisManager.axis.future:
                //_decalMaterial.SetTexture("MainTexture", featureTexture);
                _decalMaterial.SetTexture("Base_Map", featureTexture);
                break;
            case TimeAxisManager.axis.past:
                //_decalMaterial.SetTexture("MainTexture", pastTexture);
                _decalMaterial.SetTexture("Base_Map", pastTexture);
                break;
        }
    }
}