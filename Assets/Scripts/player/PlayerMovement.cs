using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    
    public event Action OnPlayerHitDanger;
    public event Action OnPlayerOnGoal;
    
    [Header("General")]
    [SerializeField] private StudentController spawnPoint;
    [SerializeField] private BallController ball;
    [SerializeField] private FPSCamera povCamera;

    [Header("Ball Throwing")] 
    [SerializeField] private float baseThrowVelocity = 3f;
    [SerializeField] private float maxThrowVelocity = 6f;
    [SerializeField] private float minThrowVelocity = 1f;
    [SerializeField] private float speedIncreaseSteps = 0.5f;
    [SerializeField] private float speedTimeInterval = 0.5f;
    
    private TrajectoryDrawer _trajectoryDrawer;
    private float _speed;
    private Coroutine _speedRoutine;
    private bool _isBallThrown;

    void Start()
    {
        _trajectoryDrawer = GetComponent<TrajectoryDrawer>();
        SetNewSeat(spawnPoint);
        _speed = baseThrowVelocity;
        ball.OnBallNewSeat += ChangePOV;
    }

    private void OnDestroy()
    {
        ball.OnBallNewSeat -= ChangePOV;
    }

    void FixedUpdate()
    {
        if (!_isBallThrown)
        {
            _trajectoryDrawer.SimulateTrajectory(ball.transform.position, povCamera.transform.forward, _speed);
        }
    }

    public void Throw(InputAction.CallbackContext context)
    {
        if (_isBallThrown) return;
        
        if (!context.canceled) return;
        ball.Throw(povCamera.transform.forward, _speed, transform);
        _isBallThrown = true;
    }

    private void ChangePOV(Collider other)
    {
        StudentController student = other.GetComponent<StudentController>();
        if (student == null)
        {
            Debug.Log("Hit with element that is not of type StudentController, POV cannot be changed.");
            return;
        } 
        SetNewSeat(student);
        _isBallThrown = false;
        ball.Pick(povCamera.PickedItemPosition, povCamera.transform);
    }

    public void IncreaseSpeed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartSpeedRoutine(true);
        }

        if (context.canceled)
        {
            StopSpeedRoutine();
        }
    }

    public void DecreaseSpeed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartSpeedRoutine(false);
        }

        if (context.canceled)
        {
            StopSpeedRoutine();
        }
    }

    private void StartSpeedRoutine(bool increase)
    {
        // stop any existing routine (prevents overlap)
        StopSpeedRoutine();

        _speedRoutine = StartCoroutine(ChangeSpeedRoutine(increase));
    }

    private void StopSpeedRoutine()
    {
        if (_speedRoutine != null)
        {
            StopCoroutine(_speedRoutine);
            _speedRoutine = null;
        }
    }

    IEnumerator ChangeSpeedRoutine(bool increase)
    {
        while (true)
        {
            if (increase)
                _speed += speedIncreaseSteps;
            else
                _speed -= speedIncreaseSteps;

            _speed = Mathf.Clamp(_speed, minThrowVelocity, maxThrowVelocity);

            yield return new WaitForSeconds(speedTimeInterval);
        }
    }

    private void SetNewSeat(StudentController s)
    {
        transform.position = s.VisionPoint.position;
        transform.rotation = s.VisionPoint.rotation;
    }
}