using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    public event Action<Collider> OnBallNewSeat;
    public event Action<Collider> OnBallHitDanger;
    
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

    public void Throw(Vector3 direction, float speed, Transform parent)
    {
        _rb.isKinematic = false;
        _rb.linearVelocity = direction * speed;
        if (parent != null) transform.SetParent(parent);
    }

    public void Pick(Transform pickPosition, Transform parent)
    {
        Debug.Log("Ball picked");
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.SetParent(parent);
        transform.position = pickPosition.position;
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

}
