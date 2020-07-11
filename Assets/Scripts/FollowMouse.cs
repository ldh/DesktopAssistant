using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera camera;
    private Vector3 offset;

    private void Start()
    {
        camera = GameManager.Instance.Camera;
    }

    private void Update()
    {
        Collider2D hitCollider = GameManager.Instance.HitCollider;
        if (!hitCollider || hitCollider.transform != transform) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            offset = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f - transform.position;
        }
        
        if (Input.GetMouseButton(1))
        {
            transform.position = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f - offset;
        }
       
    }
}