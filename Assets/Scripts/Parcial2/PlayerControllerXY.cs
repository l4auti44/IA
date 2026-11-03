using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerXY : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;

    private CharacterController controller;
    private InputAction moveAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Movimiento en XY, sin escalones ni desplazamientos mínimos.
        controller.stepOffset = 0f;
        controller.minMoveDistance = 0f;

        moveAction = new InputAction("Move", InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        Vector2 input = Vector2.ClampMagnitude(
            moveAction.ReadValue<Vector2>(), 1f
        );

        Vector3 movement = new Vector3(input.x, input.y, 0f);
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        moveAction.Dispose();
    }
}
