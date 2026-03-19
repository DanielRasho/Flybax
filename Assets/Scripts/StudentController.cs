using System;
using UnityEngine;

public class StudentController : MonoBehaviour
{
    [SerializeField] private bool isDestinyStudent;
    public bool IsDestinyStudent => isDestinyStudent;
    [SerializeField] private Transform visionPoint;
    public Transform VisionPoint => visionPoint;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
