using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    [SerializeField] private RectTransform m_unitSelectionVisual;
    [SerializeField] private Canvas m_unitSelectionCanvas; 
    
    private EntityManager m_entityManager;
    private EntityQuery m_entityQuery;
    private Vector2 m_selectionStartPosition;
    private Vector2 m_selectionEndPosition;
    private Rect m_selectedAreaRect;

    private void Start()
    {
        m_unitSelectionVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_unitSelectionVisual.gameObject.SetActive(true);
            m_selectionStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            m_selectionEndPosition = Input.mousePosition;
            UpdateSelectionVisual();
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_unitSelectionVisual.gameObject.SetActive(false);
            SelectUnits();
        }
        if (Input.GetMouseButton(1))
        {
            MoveSelectedUnit();
        }
    }

    #region SelectionVisual

    private void UpdateSelectionVisual()
    {
        m_selectedAreaRect = GetSelectionAreaRect();
        float scale = m_unitSelectionCanvas.transform.localScale.x;
        m_unitSelectionVisual.anchoredPosition = m_selectedAreaRect.position / scale;
        m_unitSelectionVisual.sizeDelta = m_selectedAreaRect.size / scale;
    }

    private Rect GetSelectionAreaRect()
    {
        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(m_selectionStartPosition.x, m_selectionEndPosition.x),
            Mathf.Min(m_selectionStartPosition.y, m_selectionEndPosition.y)
        );
        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(m_selectionStartPosition.x, m_selectionEndPosition.x),
            Mathf.Max(m_selectionStartPosition.y, m_selectionEndPosition.y)
        );
        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x-lowerLeftCorner.x,
            upperRightCorner.y-lowerLeftCorner.y
        );
    }

    #endregion
    
    private void SelectUnits(){
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(m_entityManager);
        NativeArray<Entity> entities = m_entityQuery.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in entities)
        {
            m_entityManager.SetComponentEnabled<Selected>(entity, false);
        }
        m_entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, UnitMover>().WithPresent<Selected>().Build(m_entityManager);
        entities = m_entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> unitLocalTransform = m_entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        for (int unitIndex = 0; unitIndex < unitLocalTransform.Length; unitIndex++)
        {
            if (m_selectedAreaRect.Contains(
                    Camera.main.WorldToScreenPoint(unitLocalTransform[unitIndex].Position)))
            {
                m_entityManager.SetComponentEnabled<Selected>(entities[unitIndex], true);
            }
        }
    }
    private void MoveSelectedUnit()
    {
        Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(m_entityManager);
        NativeArray<UnitMover> unitMovers = m_entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
        for (int unitMoverIndex = 0; unitMoverIndex < unitMovers.Length; unitMoverIndex++)
        {
            UnitMover unitMover = unitMovers[unitMoverIndex];
            unitMover.TargetPosition = mouseWorldPosition;
            unitMovers[unitMoverIndex] = unitMover;
        }

        m_entityQuery.CopyFromComponentDataArray(unitMovers);
    }
}
