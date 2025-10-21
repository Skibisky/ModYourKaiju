using Sentient;
using Sentient.MeYouKaiju;
using UnityEngine;
using Zenject;

public class BiplaneController : MonoBehaviour
{
    [SerializeField]
    [InjectOptional]
    private Rigidbody _rigidbody;

    [InjectOptional]
    private Health _health;

    private Transform _transform;

    [Header("Config")]
    public float ForwardAccel = 5f;

    public float RollAccel = 5f;

    public float PitchAccel = 5f;

    public float YawAccel = 5f;

    public float TiltAmount = 25f;

    public float RotationSlerp = 0.1f;

    [Header("Inputs")]
    [Range(-1f, 1f)]
    public float Input_Forward;

    [Range(-1f, 1f)]
    public float Input_Roll;

    [Header("Input")]
    [Range(-1f, 1f)]
    public float Input_Pitch;

    [Range(-1f, 1f)]
    public float Input_Yaw;

    [UnityCallback]
    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    [UnityCallback]
    private void Update()
    {
        if ((bool)_rigidbody && (!_health || !_health.IsDead))
        {
            Vector3 movement = Vector3.forward * ForwardAccel;
            movement += (Input_Forward + 0.3f) * ForwardAccel * Vector3.forward;

            _rigidbody.AddRelativeForce(movement, ForceMode.Acceleration);
            Vector3 val4 = Input_Yaw * YawAccel * Vector3.up + Input_Pitch * PitchAccel * Vector3.right + Input_Roll * RollAccel * Vector3.forward;
            _rigidbody.AddRelativeTorque(val4, ForceMode.Acceleration);
        }
    }
}
