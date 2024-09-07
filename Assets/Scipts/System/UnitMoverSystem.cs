using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        unitMoverJob.ScheduleParallel();
    }

}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref LocalTransform localTransform
        , in UnitMover unitMover
        , ref PhysicsVelocity physicsVelocity
        )
    {
        float3 moveDirection = unitMover.TargetPosition - localTransform.Position;
        moveDirection.y = 0f;
        if (math.lengthsq(moveDirection) < unitMover.ReachedThreshold)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        moveDirection = math.normalize(moveDirection);
        localTransform.Rotation =
            math.slerp(localTransform.Rotation, 
                quaternion.LookRotation(moveDirection, math.up()),
                DeltaTime * unitMover.RotationSpeed);
        physicsVelocity.Linear = moveDirection * unitMover.MoveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}
