using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main; // si usas Cinemachine también funciona, ya que asigna la MainCamera
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            transform.LookAt(transform.position + cam.transform.forward);
        }
    }
}
