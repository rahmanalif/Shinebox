using System;
using UnityEngine;

namespace HoopGame
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Timer")]
        [SerializeField] private float startTime = 30f;
        [SerializeField] private float baseTimeReward = 2f;
        [SerializeField] private float minTimeReward = 0.5f;
        [SerializeField] private float timeRewardDecay = 1.2f;

        [Header("Scoring")]
        [SerializeField] private int scorePerShot = 1;
        [SerializeField] private int cleanShotBonus = 1;
        [SerializeField] private int onFireBonus = 1;
        [SerializeField] private int streakForFire = 3;

        [Header("Difficulty")]
        [SerializeField] private int scoreForMaxDifficulty = 20;

        public event Action<int> ScoreChanged;
        public event Action<int> ComboChanged;
        public event Action<bool> FireStateChanged;
        public event Action<float> TimerChanged;
        public event Action GameOver;

        public bool IsGameOver { get; private set; }
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public bool IsOnFire { get; private set; }
        public float TimeRemaining { get; private set; }
        public float CurrentDifficulty { get; private set; }

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
            TimeRemaining = startTime;
            TimerChanged?.Invoke(TimeRemaining);
        }

        private void Update()
        {
            if (IsGameOver)
            {
                return;
            }

            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                TimerChanged?.Invoke(TimeRemaining);
                EndGame();
                return;
            }

            TimerChanged?.Invoke(TimeRemaining);
        }

        public void RegisterScore(bool cleanShot)
        {
            if (IsGameOver)
            {
                return;
            }

            Combo += 1;
            ComboChanged?.Invoke(Combo);

            if (Combo >= streakForFire && !IsOnFire)
            {
                IsOnFire = true;
                FireStateChanged?.Invoke(true);
            }

            var points = scorePerShot;
            if (IsOnFire)
            {
                points += onFireBonus;
            }

            if (cleanShot)
            {
                points += cleanShotBonus;
            }

            Score += points;
            ScoreChanged?.Invoke(Score);

            CurrentDifficulty = Mathf.Clamp01(scoreForMaxDifficulty <= 0
                ? 0f
                : (float)Score / scoreForMaxDifficulty);

            var reward = Mathf.Max(minTimeReward, baseTimeReward - CurrentDifficulty * timeRewardDecay);
            TimeRemaining += reward;
            TimerChanged?.Invoke(TimeRemaining);
        }

        public void RegisterMiss()
        {
            if (IsGameOver)
            {
                return;
            }

            Combo = 0;
            if (IsOnFire)
            {
                IsOnFire = false;
                FireStateChanged?.Invoke(false);
            }

            ComboChanged?.Invoke(Combo);
        }

        private void EndGame()
        {
            if (IsGameOver)
            {
                return;
            }

            IsGameOver = true;
            GameOver?.Invoke();
        }
    }
}
