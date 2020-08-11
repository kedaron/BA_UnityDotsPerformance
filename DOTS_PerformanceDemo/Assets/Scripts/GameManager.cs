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


    private List<JobsTestingSphere> jobsSphereList;

    public class JobsTestingSphere
    {
        public Transform transform;
        public float movementSpeed;
        public float3 direction;
    }

    public int spawnAmount;
    public bool useEcsAndJobs;
    public bool useJobs;
    public bool doPseudoCalculations;

    void Start()
    {
        entitiyManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // ECS + Jobs
        if (useEcsAndJobs)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                var spawnLocation = new float3(0f, UnityEngine.Random.Range(-5f, 5f), 0f);
                var movementSpeed = UnityEngine.Random.Range(1.0f, 6.0f);
                CreateEntity(spawnLocation, movementSpeed);
            }
        }
        // Jobs
        else if(useJobs)
        {
            jobsSphereList = new List<JobsTestingSphere>();
            for (int i = 0; i < spawnAmount; i++)
            {
                Transform sphereGameObject = Instantiate(jobsSpherePrefab, new Vector3(0f, UnityEngine.Random.Range(-5f, 5f), 0f), Quaternion.identity);
                jobsSphereList.Add(new JobsTestingSphere
                {
                    transform = sphereGameObject,
                    movementSpeed = UnityEngine.Random.Range(1.0f, 6.0f),
                    direction = new float3(RandomSign(), 0f, 0f)
                });
            }
        }
        // Default GameObjects with Scripts
        else
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                Instantiate(classicSpherePrefab, new Vector3(0f, UnityEngine.Random.Range(-5f, 5f), 0f), Quaternion.identity);
            }
        }
    }

    private void Update()
    {
        if (useJobs && !useEcsAndJobs)
        {
            ExecuteMovementJob();
            ExecuteDirectionJob();
        }
    }

    #region Jobs
    private void ExecuteMovementJob()
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(jobsSphereList.Count, Allocator.TempJob);
        NativeArray<float3> directionArray = new NativeArray<float3>(jobsSphereList.Count, Allocator.TempJob);
        NativeArray<float> movementSpeedArray = new NativeArray<float>(jobsSphereList.Count, Allocator.TempJob);
        for (int i = 0; i < jobsSphereList.Count; i++)
        {
            positionArray[i] = jobsSphereList[i].transform.position;
            directionArray[i] = jobsSphereList[i].direction;
            movementSpeedArray[i] = jobsSphereList[i].movementSpeed;
        }

        MovementJob job = new MovementJob
        {
            deltaTime = Time.deltaTime,
            doPseudoCalculations = doPseudoCalculations,
            position = positionArray,
            direction = directionArray,
            movementSpeed = movementSpeedArray
        };

        job.Schedule(jobsSphereList.Count, jobsSphereList.Count/10).Complete();

        for (int i = 0; i < jobsSphereList.Count; i++)
        {
            jobsSphereList[i].transform.position = positionArray[i];
        }

        positionArray.Dispose();
        directionArray.Dispose();
        movementSpeedArray.Dispose();
    }

    private void ExecuteDirectionJob()
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(jobsSphereList.Count, Allocator.TempJob);
        NativeArray<float3> directionArray = new NativeArray<float3>(jobsSphereList.Count, Allocator.TempJob);
        for (int i = 0; i < jobsSphereList.Count; i++)
        {
            positionArray[i] = jobsSphereList[i].transform.position;
            directionArray[i] = jobsSphereList[i].direction;
        }

        DirectionJob job = new DirectionJob
        {
            position = positionArray,
            direction = directionArray,
        };

        job.Schedule(jobsSphereList.Count, jobsSphereList.Count / 10).Complete();

        for (int i = 0; i < jobsSphereList.Count; i++)
        {
            jobsSphereList[i].direction = directionArray[i];
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
