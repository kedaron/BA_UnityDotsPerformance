using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

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
            for(int i = 0; i < 1000; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    int drop = i * j;
                }
            }
        }
    }
}
