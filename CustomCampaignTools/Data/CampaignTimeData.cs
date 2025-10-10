using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCampaignTools.Data
{
    public class CampaignTimeData
    {
        private int _totalSecondsElapsed;
        public Dictionary<string, TimeDataPair> individualLevelTimes = [];

        public int TotalSecondsElapsed 
        {
            get 
            {
                if (_totalSecondsElapsed == 0)
                    SumTimes();

                return _totalSecondsElapsed; 
            }
        }

        void SumTimes()
        {
            _totalSecondsElapsed = 0;
            foreach(TimeDataPair levelTime in individualLevelTimes.Values)
            {
                _totalSecondsElapsed = levelTime.totalTimeElapsed;
            }
        }

    }

    public class TimeDataPair
    {
        public int totalTimeElapsed = 0;
        public int bestCompletedTime = 0;

        public void RecordCompletedTime(int seconds)
        {
            if(seconds < bestCompletedTime || bestCompletedTime == 0)
                bestCompletedTime = seconds;
        }
    }
}
