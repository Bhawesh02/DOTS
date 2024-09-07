using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
        {
            SystemAPI.SetComponentEnabled<MaterialMeshInfo>(selected.ValueRO.SelectedVisualEntity, true);
        }
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            SystemAPI.SetComponentEnabled<MaterialMeshInfo>(selected.ValueRO.SelectedVisualEntity, false);
        }
    }
}
