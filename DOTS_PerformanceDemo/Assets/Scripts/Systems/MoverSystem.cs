using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoverSystem : SystemBase
{
    private bool doPseudoCalculations;
    protected override void OnCreate()
    {
        doPseudoCalculations = GameObject.FindObjectOfType<GameManager>().doPseudoCalculations;
    }
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        var doPseudoCalculations = this.doPseudoCalculations;
        Entities.ForEach((ref Translation position, ref MovementDirection direction, in MovementSpeed speed) =>
        {
            position.Value += direction.direction * speed.movementSpeed * dt;
            // Pseudo calculations
            if (doPseudoCalculations)
            {
                // TODO
            }
        }).ScheduleParallel();
    }
}
