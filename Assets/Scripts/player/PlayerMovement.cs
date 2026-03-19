using System;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private StudentController spawnPoint;
    
    void Start()
    {
        transform.position = spawnPoint.VisionPoint.position;
        transform.rotation = spawnPoint.VisionPoint.rotation;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
