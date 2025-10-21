using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

[RequireComponent(typeof(VisualEffect))]
public class ParticlesFromBiplane : MonoBehaviour, IInstaller
{
    public float SpawnRate = 128f;

    public string SpawnRatePropertyName = "SpawnRate";

    public float ParticleLife = 2f;

    public string VelocityPropertyName = "Velocity";

    [Inject]
    private ParticleParent _particleParent;

    private BiplaneController _biplaneController;

    private Rigidbody _rigidbody;

    private VisualEffect _vfx;

    private Transform _followXf;

    private Transform _transform;

    public bool IsEnabled { get; } = true;

    public void InstallBindings()
    {
        Debug.Log("install bindings");
    }

    [ComponentConstructor]
    private void Construct(ParticleParent particleParent, BiplaneController biplaneController)
    {
        Debug.Log("construct", particleParent);
        _particleParent = particleParent;
        _biplaneController = biplaneController;
    }

    [UnityCallback]
    private void Start()
    {
        if (!_particleParent )
        {
            this.enabled = false;
            return;
        }

        _vfx = GetComponent<VisualEffect>();
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponentInParent<Rigidbody>();
        _followXf = _transform.parent;
        _transform.SetParent(_particleParent.transform, worldPositionStays: true);
        if (_vfx && _vfx.visualEffectAsset)
            _vfx.SetFloat(SpawnRatePropertyName, 0f);
        else
            this.enabled = false;
    }

    [UnityCallback]
    private void Update()
    {
        if (!_vfx || !_vfx.visualEffectAsset)
            return;

        if ((bool)_followXf)
        {
            _transform.SetPositionAndRotation(_followXf.position, _followXf.rotation);
        }
        else
        {
            ParticleLife -= Time.deltaTime;
            if (ParticleLife <= 0f)
            {
                UnityEngine.Object.Destroy(base.gameObject);
                return;
            }
        }

        if ((bool)_rigidbody)
        {
            _vfx.SetVector3(VelocityPropertyName, _rigidbody.velocity);
        }

        float num = 0f;
        if ((bool)_biplaneController && _biplaneController.enabled)
        {
            num = math.unlerp(-1.2f, 1f, _biplaneController.Input_Forward);
        }

        _vfx.SetFloat(SpawnRatePropertyName, num * SpawnRate);
    }
}
