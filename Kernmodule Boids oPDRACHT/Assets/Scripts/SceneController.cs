using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private List<BoidController> boidPrefabs;

    public int spawnBoids = 100;
    public float boidSpeed = 10f;
    public float boidSteeringSpeed = 100f;
    public float boidNoClumpingArea = 10f;
    public float boidLocalArea = 10f;
    public float boidSimulationArea = 50f;

    private List<BoidController> _boids;

    private void Start()
    {
        _boids = new List<BoidController>();

        foreach (var prefab in boidPrefabs)
        {
            for (int i = 0; i < spawnBoids; i++)
            {
                SpawnBoid(prefab.gameObject, boidPrefabs.IndexOf(prefab));
            }
        }
    }

    private void Update()
    {
        foreach (BoidController boid in _boids)
        {
            boid.SimulateMovement(_boids, Time.deltaTime);

            var boidPos = boid.transform.position;

            boidPos = new Vector3(
                Mathf.Clamp(boidPos.x, -boidSimulationArea, boidSimulationArea),
                Mathf.Clamp(boidPos.y, -boidSimulationArea, boidSimulationArea),
                Mathf.Clamp(boidPos.z, -boidSimulationArea, boidSimulationArea)
            );

            boid.transform.position = boidPos;
        }
    }

    private void SpawnBoid(GameObject prefab, int swarmIndex)
    {
        var boidInstance = Instantiate(prefab);

        boidInstance.transform.Translate(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)));
        boidInstance.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        var boidController = boidInstance.GetComponent<BoidController>();
        boidController.swarmIndex = swarmIndex;
        boidController.speed = boidSpeed;
        boidController.steeringSpeed = boidSteeringSpeed;
        boidController.localAreaRadius = boidLocalArea;
        boidController.noClumpingRadius = boidNoClumpingArea;

        _boids.Add(boidController);
    }
}
