using System;
using UnityEngine;

public class StudentController : MonoBehaviour
{
    [SerializeField] private bool isDestinyStudent;
    public bool IsDestinyStudent => isDestinyStudent;
    [SerializeField] private Transform visionPoint;
    public Transform VisionPoint => visionPoint;

    private Collider studentCollider;
    
    void Start()
    {
        studentCollider = GetComponents<Collider>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAsCurrent()
    {
        studentCollider.enabled = false;
    }
}
