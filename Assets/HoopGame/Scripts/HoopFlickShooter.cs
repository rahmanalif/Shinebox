using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HoopGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class HoopFlickShooter : MonoBehaviour
    {
        [SerializeField] private float tapImpulse = 6.5f;
        [SerializeField] private float startHorizontalSpeed = 3.5f;
        [SerializeField] private float minHorizontalSpeed = 1.5f;
        [SerializeField] private float horizontalBounce = 0.95f;
        [SerializeField] private float verticalBounce = 0.95f;
        [SerializeField] private float minBounceSpeed = 1.2f;
        [SerializeField] private bool ignoreTapsOverUI = true;

        private Rigidbody2D body;
        private Camera mainCamera;
        private float lastHorizontalSign = 1f;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            var velocity = body.linearVelocity;
            if (Mathf.Abs(velocity.x) < minHorizontalSpeed)
            {
                velocity.x = Mathf.Sign(lastHorizontalSign) * startHorizontalSpeed;
                body.linearVelocity = velocity;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return;
            }

            if (!IsTapDown())
            {
                return;
            }

            body.AddForce(Vector2.up * tapImpulse, ForceMode2D.Impulse);
        }

        private void FixedUpdate()
        {
            if (mainCamera == null)
            {
                return;
            }

            var viewport = mainCamera.WorldToViewportPoint(transform.position);
            if (viewport.x < 0f && body.linearVelocity.x < 0f)
            {
                BounceAtViewportEdge(0f, viewport);
            }
            else if (viewport.x > 1f && body.linearVelocity.x > 0f)
            {
                BounceAtViewportEdge(1f, viewport);
            }

            if (viewport.y < 0f && body.linearVelocity.y < 0f)
            {
                BounceAtViewportVerticalEdge(0f, viewport);
            }
            else if (viewport.y > 1f && body.linearVelocity.y > 0f)
            {
                BounceAtViewportVerticalEdge(1f, viewport);
            }

            MaintainHorizontalMomentum();
        }

        private void BounceAtViewportEdge(float targetX, Vector3 viewport)
        {
            var velocity = body.linearVelocity;
            velocity.x = -velocity.x * horizontalBounce;
            lastHorizontalSign = Mathf.Sign(velocity.x == 0f ? -lastHorizontalSign : velocity.x);
            if (Mathf.Abs(velocity.x) < minBounceSpeed)
            {
                velocity.x = Mathf.Sign(velocity.x == 0f ? 1f : velocity.x) * minBounceSpeed;
            }

            body.linearVelocity = velocity;
            viewport.x = targetX;
            var world = mainCamera.ViewportToWorldPoint(viewport);
            transform.position = new Vector3(world.x, transform.position.y, transform.position.z);
        }

        private void MaintainHorizontalMomentum()
        {
            var velocity = body.linearVelocity;
            if (Mathf.Abs(velocity.x) >= minHorizontalSpeed)
            {
                lastHorizontalSign = Mathf.Sign(velocity.x);
                return;
            }

            velocity.x = Mathf.Sign(lastHorizontalSign) * minHorizontalSpeed;
            body.linearVelocity = velocity;
        }

        private void BounceAtViewportVerticalEdge(float targetY, Vector3 viewport)
        {
            var velocity = body.linearVelocity;
            velocity.y = -velocity.y * verticalBounce;
            if (Mathf.Abs(velocity.y) < minBounceSpeed)
            {
                velocity.y = Mathf.Sign(velocity.y == 0f ? 1f : velocity.y) * minBounceSpeed;
            }

            body.linearVelocity = velocity;
            viewport.y = targetY;
            var world = mainCamera.ViewportToWorldPoint(viewport);
            transform.position = new Vector3(transform.position.x, world.y, transform.position.z);
        }

        private bool IsTapDown()
        {
            if (ignoreTapsOverUI && EventSystem.current != null)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return false;
                }
            }

#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null)
            {
                return Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            }

            return Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
#else
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).phase == TouchPhase.Began;
            }

            return Input.GetMouseButtonDown(0);
#endif
        }
    }
}
