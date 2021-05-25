using System;

namespace GamesLibrary.Models
{
    public struct TimingInfo
    {
        public TimingInfo(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            ElapsedTime = elapsedTime;
            TotalTime = totalTime;
        }

        public TimeSpan ElapsedTime;
        public TimeSpan TotalTime;
    }
}
