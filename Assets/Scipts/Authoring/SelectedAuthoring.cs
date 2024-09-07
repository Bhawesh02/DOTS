using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class SelectedAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject m_selectedVisualGameObject;
    public class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected(
                GetEntity(authoring.m_selectedVisualGameObject, TransformUsageFlags.Dynamic)
                )
            );
            SetComponentEnabled<Selected>(entity, false);
        }
    }
}

public struct Selected : IComponentData, IEnableableComponent
{
    public Entity SelectedVisualEntity;

    public Selected(Entity selectedVisualEntity)
    {
        SelectedVisualEntity = selectedVisualEntity;
    }
}
