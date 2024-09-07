using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private float m_reachedThreshold = 2f;
    
    public class Baker : Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMover
            {
                MoveSpeed = authoring.m_moveSpeed,
                RotationSpeed = authoring.m_rotationSpeed,
                ReachedThreshold = authoring.m_reachedThreshold,
                TargetPosition = authoring.transform.position,
            });
        }
    }
}

public struct UnitMover : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;
    public float3 TargetPosition;
    public float ReachedThreshold;
}
