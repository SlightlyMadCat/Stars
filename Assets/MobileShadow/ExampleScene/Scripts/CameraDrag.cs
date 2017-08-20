using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraDrag : MonoBehaviour
{
    private Transform _target;
    private Camera _camera;
    public BoxCollider MoveCollider;
    
    public Vector2 clampX = Vector2.zero;
    public Vector2 clampZ = Vector2.zero;

    Vector3 _startPos = Vector3.zero;
    Vector3 _posC = Vector3.zero;

    RaycastHit hit;
    private Ray ray;
    private Vector3 _velocity;
    private bool _isDrag;

    public void Awake()
    {
        _target = transform;
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        var tapPosition = new Vector3();

        tapPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            _isDrag = true;
            ray = _camera.ScreenPointToRay(tapPosition);
            if (MoveCollider.Raycast(ray, out hit, 200))
            {
                _startPos = hit.point;
                return;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isDrag = false;
        }
        if (Input.GetMouseButton(0) && _isDrag)
        {
            Vector3 _prevPosition = _target.position;
            ray = _camera.ScreenPointToRay(tapPosition);
            if (MoveCollider.Raycast(ray, out hit, 200))
            {
                _posC = _target.position + _startPos - hit.point;
                TargetPosFromClampPosC();
            }
            _velocity = _target.position - _prevPosition;
        }

        if (!_isDrag && _velocity.sqrMagnitude > 0.01f)
        {
            _target.position += _velocity;
            _posC = _target.position;
            TargetPosFromClampPosC();
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime);
        }
    }
    
    private void TargetPosFromClampPosC()
    {
        _posC.x = Mathf.Clamp(_posC.x, clampX.x, clampX.y);
        _posC.z = Mathf.Clamp(_posC.z, clampZ.x, clampZ.y);
        _target.position = _posC;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        if (EventSystem.current != null) EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}