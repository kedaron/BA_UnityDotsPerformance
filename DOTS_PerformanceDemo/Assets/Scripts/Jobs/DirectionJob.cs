using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct DirectionJob : IJobParallelFor
{
    [ReadOnly]
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
