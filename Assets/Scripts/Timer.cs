using UnityEngine;

namespace Scripts
{
    public class Timer
    {
        public const float Period = 3.0f;

        public float Clock { get; set; } = 0.0f;

        public void Tick()
        {
            Clock -= Time.deltaTime;
        }

        public void ResetTimer()
        {
            Clock = Period;
        }
    }
}