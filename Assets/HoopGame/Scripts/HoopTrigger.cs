using UnityEngine;

namespace HoopGame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class HoopTrigger : MonoBehaviour
    {
        [SerializeField] private string ballTag = "Ball";
        [SerializeField] private float baseScoreHalfWidth = 0.45f;
        [SerializeField] private float minScoreHalfWidth = 0.25f;
        [SerializeField] private float baseCleanHalfWidth = 0.3f;
        [SerializeField] private float minCleanHalfWidth = 0.18f;

        private bool enteredFromAbove;
        private Rigidbody2D ballBody;
        private BallRimTracker rimTracker;

        private void Reset()
        {
            var trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(ballTag))
            {
                return;
            }

            enteredFromAbove = other.transform.position.y > transform.position.y;
            ballBody = other.attachedRigidbody;
            rimTracker = other.GetComponent<BallRimTracker>();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(ballTag))
            {
                return;
            }

            var exitedBelow = other.transform.position.y < transform.position.y;
            var falling = ballBody == null || ballBody.linearVelocity.y <= 0f;

            if (enteredFromAbove && exitedBelow && falling)
            {
                var difficulty = GameManager.Instance != null ? GameManager.Instance.CurrentDifficulty : 0f;
                var scoreHalfWidth = Mathf.Lerp(baseScoreHalfWidth, minScoreHalfWidth, difficulty);
                var cleanHalfWidth = Mathf.Lerp(baseCleanHalfWidth, minCleanHalfWidth, difficulty);
                var localX = transform.InverseTransformPoint(other.transform.position).x;

                if (Mathf.Abs(localX) <= scoreHalfWidth)
                {
                    var cleanShot = rimTracker == null || !rimTracker.RimHit;
                    if (Mathf.Abs(localX) > cleanHalfWidth)
                    {
                        cleanShot = false;
                    }

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.RegisterScore(cleanShot);
                    }

                    if (HoopManager.Instance != null)
                    {
                        HoopManager.Instance.HandleScore();
                    }
                }
                else if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegisterMiss();
                }
            }

            if (rimTracker != null)
            {
                rimTracker.ResetRimHit();
            }

            enteredFromAbove = false;
            ballBody = null;
            rimTracker = null;
        }
    }
}
