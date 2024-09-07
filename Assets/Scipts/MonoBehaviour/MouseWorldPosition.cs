using System;
using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    private Plane m_plane = new Plane(Vector3.up, Vector3.zero);
    
    public static MouseWorldPosition Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public Vector3 GetPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return m_plane.Raycast(mouseCameraRay, out float distance) 
            ? mouseCameraRay.GetPoint(distance) 
            : Vector3.zero;
    }
}
