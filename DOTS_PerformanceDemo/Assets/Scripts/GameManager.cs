using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class GameManager : MonoBehaviour
{
    private EntityManager entitiyManager;

    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private Transform jobsSpherePrefab;
    [SerializeField] private GameObject classicSpherePrefab;


    private List<TestingSphereJobs> sphereListJobs;

    public class TestingSphereJobs
    {
        public Transform transform;
        public float movementSpeed;
        public Vector3 direction;
    }

    public int spawnAmount;
    public bool useEcsAndJobs;
    public bool useJobs;
    public bool doPseudoCalculations;

    void Start()
    {
        entitiyManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (useEcsAndJobs)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                var spawnLocation = new float3(0f, UnityEngine.Random.Range(-5f, 5f), 0f);
                var movementSpeed = UnityEngine.Random.Range(1.0f, 6.0f);
                CreateEntity(spawnLocation, movementSpeed);
            }
        }
        else if(useJobs)
        {
            sphereListJobs = new List<TestingSphereJobs>();
            for (int i = 0; i < spawnAmount; i++)
            {
                Transform sphereGameObject = Instantiate(jobsSpherePrefab, new Vector3(0f, UnityEngine.Random.Range(-5f, 5f), 0f), Quaternion.identity);
                sphereListJobs.Add(new TestingSphereJobs
                {
                    transform = sphereGameObject,
                    movementSpeed = UnityEngine.Random.Range(1.0f, 6.0f),
                    direction = new Vector3(RandomSign(), 0f, 0f)
                });
            }
        }
        else
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                Instantiate(classicSpherePrefab, new Vector3(0f, UnityEngine.Random.Range(-5f, 5f), 0f), Quaternion.identity);
            }
        }
    }

    #region Jobs
    private void Update()
    {
        if (useJobs && !useEcsAndJobs)
        {
            ExecuteMovementJob();
            ExecuteDirectionJob();
        }
    }

    private void ExecuteMovementJob()
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(sphereListJobs.Count, Allocator.TempJob);
        NativeArray<float3> directionArray = new NativeArray<float3>(sphereListJobs.Count, Allocator.TempJob);
        NativeArray<float> movementSpeedArray = new NativeArray<float>(sphereListJobs.Count, Allocator.TempJob);
        for (int i = 0; i < sphereListJobs.Count; i++)
        {
            positionArray[i] = sphereListJobs[i].transform.position;
            directionArray[i] = sphereListJobs[i].direction;
            movementSpeedArray[i] = sphereListJobs[i].movementSpeed;
        }

        MovementJob job = new MovementJob
        {
            deltaTime = Time.deltaTime,
            doPseudoCalculations = doPseudoCalculations,
            position = positionArray,
            direction = directionArray,
            movementSpeed = movementSpeedArray
        };

        job.Schedule(sphereListJobs.Count, sphereListJobs.Count/10).Complete();

        for (int i = 0; i < sphereListJobs.Count; i++)
        {
            sphereListJobs[i].transform.position = positionArray[i];
        }

        positionArray.Dispose();
        directionArray.Dispose();
        movementSpeedArray.Dispose();
    }

    private void ExecuteDirectionJob()
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(sphereListJobs.Count, Allocator.TempJob);
        NativeArray<float3> directionArray = new NativeArray<float3>(sphereListJobs.Count, Allocator.TempJob);
        for (int i = 0; i < sphereListJobs.Count; i++)
        {
            positionArray[i] = sphereListJobs[i].transform.position;
            directionArray[i] = sphereListJobs[i].direction;
        }

        DirectionJob job = new DirectionJob
        {
            position = positionArray,
            direction = directionArray,
        };

        job.Schedule(sphereListJobs.Count, sphereListJobs.Count / 10).Complete();

        for (int i = 0; i < sphereListJobs.Count; i++)
        {
            sphereListJobs[i].direction = directionArray[i];
        }

        positionArray.Dispose();
        directionArray.Dispose();
    }

    #endregion

    #region ECS
    private void CreateEntity(float3 location, float speed)
    {
        EntityArchetype entityArchetype = entitiyManager.CreateArchetype(
            typeof(Translation),
            typeof(MovementSpeed),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(MovementDirection)
            );

        Entity entity = entitiyManager.CreateEntity(entityArchetype);

        entitiyManager.SetComponentData(entity, new Scale { Value = 0.5f });
        entitiyManager.SetComponentData(entity, new Translation { Value = location });
        entitiyManager.SetComponentData(entity, new MovementSpeed { movementSpeed = speed });
        entitiyManager.SetComponentData(entity, new MovementDirection { direction = new float3(RandomSign(), 0f, 0f) });
        entitiyManager.SetSharedComponentData(entity, new RenderMesh { mesh = mesh, material = material });
    }
    #endregion

    #region helper
    private int RandomSign()
    {
        return UnityEngine.Random.Range(0, 2) * 2 - 1;
    }
    #endregion
}

[BurstCompile]
public struct MovementJob : IJobParallelFor
{
    public NativeArray<float> movementSpeed;
    public NativeArray<float3> position;
    public NativeArray<float3> direction;
    public float deltaTime;
    public bool doPseudoCalculations;

    public void Execute(int index)
    {
        position[index] += direction[index] * movementSpeed[index] * deltaTime;

        // Pseudo calculations
        if (doPseudoCalculations)
        {
            // TODO
        }
    }
}

[BurstCompile]
public struct DirectionJob : IJobParallelFor
{
    public NativeArray<float3> position;
    public NativeArray<float3> direction;

    public void Execute(int index)
    {
        if ((direction[index].x == 1 && position[index].x > 10f)
        || (direction[index].x == -1 && position[index].x < -10f))
        {
            direction[index] = direction[index] * -1;
        }
    }
}
