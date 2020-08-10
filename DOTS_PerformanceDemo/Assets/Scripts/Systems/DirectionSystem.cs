using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MovementDirection direction, in Translation position) =>
        {
            if ((direction.direction.x == 1 && position.Value.x > 10f)
            || (direction.direction.x == -1 && position.Value.x < -10f))
            {
                direction.direction.x *= -1;
            }
        }).ScheduleParallel();
    }
}
