using UnityEngine;

namespace HoopGame
{
    public sealed class HoopManager : MonoBehaviour
    {
        public static HoopManager Instance { get; private set; }

        [SerializeField] private GameObject hoopPrefab;
        [SerializeField] private Transform ball;

        [Header("Spawn Positions")]
        [SerializeField] private float rightSpawnX = 7.5f;
        [SerializeField] private float leftSpawnX = -7.5f;
        [SerializeField] private float baseMinY = -3f;
        [SerializeField] private float baseMaxY = 3f;
        [SerializeField] private float extraVariance = 1.25f;

        private GameObject currentHoop;
        private bool spawnRight = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            SpawnHoop(true);
        }

        public void HandleScore()
        {
            if (currentHoop != null)
            {
                Destroy(currentHoop);
            }

            spawnRight = !spawnRight;
            SpawnHoop(spawnRight);
        }

        private void SpawnHoop(bool onRight)
        {
            if (hoopPrefab == null)
            {
                return;
            }

            var difficulty = GameManager.Instance != null ? GameManager.Instance.CurrentDifficulty : 0f;
            var minY = baseMinY - extraVariance * difficulty;
            var maxY = baseMaxY + extraVariance * difficulty;
            var y = Random.Range(minY, maxY);
            var x = onRight ? rightSpawnX : leftSpawnX;

            currentHoop = Instantiate(hoopPrefab, new Vector3(x, y, 0f), Quaternion.identity);

            if (ball != null)
            {
                var rimTracker = ball.GetComponent<BallRimTracker>();
                if (rimTracker != null)
                {
                    rimTracker.ResetRimHit();
                }
            }
        }
    }
}
