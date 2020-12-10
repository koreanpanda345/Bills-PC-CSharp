using System;
using System.Timers;

namespace BillsPC.Web_Dashboard.Data
{
    public class BannerTimer
    {
        private Timer _timer;

        public void SetTimer(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Enabled = true;
        }

        public event Action OnElapsed;

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            OnElapsed?.Invoke();
            _timer.Dispose();
        }
    }
}