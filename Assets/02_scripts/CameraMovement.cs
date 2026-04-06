using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Vector2 moveInput;
    public float speed = 10f;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        //Debug.Log("OnMove inserted..");
    }

    void Update()
    {
        Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }
}
