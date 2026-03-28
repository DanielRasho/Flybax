using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    public event Action<Collider> OnBallNewSeat;
    public event Action<Collider> OnBallHitDanger;

    public event Action OnBallReturned;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private Transform _lastPickPosition;
    private bool _hasBeenThrown = false;
    
    private Rigidbody _rb;
    // private bool _collidingWithCurrent = false;
    private int _dangerLayer = 0;
    private int _checkpointsLayer = 0;
    private bool _isSimulated;
    
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    
    public void Init(Vector3 velocity, bool isGhost) {
        _isSimulated = isGhost;
        _rb.AddForce(velocity, ForceMode.Impulse);
    }
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) Debug.Log("WHAT IS NULL?!"); 

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (_isSimulated)
        {
            return;
        }
        _dangerLayer = LayerMask.NameToLayer("Danger");
        _checkpointsLayer = LayerMask.NameToLayer("Checkpoints");
    }

    private void Update()
    {
        if (_isSimulated) return;

        if (_hasBeenThrown && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ReturnToLastHolder();
        }
    }

    public void Throw(Vector3 direction, float speed)
    {
        _rb.isKinematic = false;
        _rb.linearVelocity = direction * speed;
        _hasBeenThrown = true;
        transform.SetParent(null);
    }

    public void Pick(Transform pickPosition)
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        _lastPickPosition = pickPosition;
        _hasBeenThrown = false;

        transform.SetParent(pickPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isSimulated) return;
        if (other.gameObject.layer == _dangerLayer)
        {
            OnBallHitDanger?.Invoke(other);
        } else if (other.gameObject.layer == _checkpointsLayer)
        {
            OnBallNewSeat?.Invoke(other);
        }
    }

    private void ReturnToLastHolder()
    {
        // Debug.Log("ReturnToLastHolder llamado");

        // if (_lastPickPosition == null)
        // {
        //     Debug.LogWarning("No hay referencia guardada para devolver la pelota.");
        //     return;
        // }

        if (_lastPickPosition == null) return;

        Pick(_lastPickPosition);
        OnBallReturned?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isSimulated) return;

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

}
