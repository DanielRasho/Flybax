using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryDrawer : MonoBehaviour {
    
    [Header("Trajectory Draw")]
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int maxPhysicsFrameIterations = 100;
    [SerializeField] private int frameSteps = 10;
    [SerializeField] private Transform obstaclesParent;

    private int currentFrame = 0;
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    
    private void Start() {
        CreatePhysicsScene();
    }
    
    public void SimulateTrajectory(Vector3 position, Vector3 direction, float speed) {
        var ghostObj = Instantiate(ballPrefab, position, Quaternion.identity);
        ghostObj.Throw(direction, speed, null);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        // Vector3 velocity = direction * speed;
        // ghostObj.Init(velocity, true);

        lineRenderer.positionCount = maxPhysicsFrameIterations;

        for (var i = 0; i < maxPhysicsFrameIterations; i++)
        {
            // currentFrame++;
            _physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, ghostObj.transform.position);
            // currentFrame = 0;
        }

        Destroy(ghostObj.gameObject);
    }

    private void CreatePhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in obstaclesParent) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    private void FixedUpdate() {
        foreach (var item in _spawnedObjects) {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }
}
