using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class RotorScript : MonoBehaviour
{
    private Camera targetCamera;
    private bool dragging = false;
    private Vector3 dragPosition;

    private void OnEnable()
    {
        dragging = false;
        targetCamera = GetComponent<Camera>();
    }
    private void OnDisable()
    {
        targetCamera = null;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pointerEvent = new PointerEventData(EventSystem.current);
            pointerEvent.position = Input.mousePosition;
            var result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEvent, result);
            if (!dragging && (result.Count == 0))
            {
                dragging = true;
                dragPosition = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            targetCamera.transform.localRotation = Quaternion.identity;
        }
        if (dragging)
        {
            var dst = Screen.height * 0.5f / Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var ofs = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, -dst);
            var v0 = (Input.mousePosition - ofs).normalized;
            var v1 = (dragPosition - ofs).normalized;
            var axis = Vector3.Cross(v0, v1);
            var angle = Mathf.Acos(Vector3.Dot(v0, v1)) * Mathf.Rad2Deg;
            targetCamera.transform.rotation = targetCamera.transform.rotation * Quaternion.AngleAxis(angle, axis);
            dragPosition = Input.mousePosition;
        }
    }
}
