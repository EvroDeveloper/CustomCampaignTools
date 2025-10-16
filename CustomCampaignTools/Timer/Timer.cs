using UnityEngine;


namespace CustomCampaignTools.Timing
{
    public class Timer
    {
        private float startTime;

        private bool running = false;
        private float accumulatedTime = 0f;

        private int pauseCount = 0;

        public void StartTimer()
        {
            startTime = UnityEngine.Time.time;
            running = true;
            pauseCount = 0;
        }

        public void PauseTimer()
        {
            if (running)
            {
                running = false;
                accumulatedTime += Time.time - startTime;
            }
            pauseCount++;
        }

        public void ResumeTimer()
        {
            if (running) return;

            pauseCount--;
            if (pauseCount > 0) return;
            
            startTime = Time.time;
            running = true;
        }

        public float GetTimeSinceStart()
        {
            if (!running) return accumulatedTime;
            return accumulatedTime + (Time.time - startTime);
        }
    }
}