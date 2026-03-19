using System;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private StudentController spawnPoint;
    
    void Awake()
    {
        gameObject.transform.position = spawnPoint.VisionPoint.transform.position;
        gameObject.transform.rotation = spawnPoint.VisionPoint.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
