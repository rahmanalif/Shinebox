using UnityEngine;

namespace HoopGame
{
    public sealed class BallRimTracker : MonoBehaviour
    {
        [SerializeField] private string rimTag = "Rim";

        public bool RimHit { get; private set; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(rimTag))
            {
                RimHit = true;
            }
        }

        public void ResetRimHit()
        {
            RimHit = false;
        }
    }
}
