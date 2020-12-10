using System;
using System.Timers;

namespace BillsPC.Web_Dashboard.Data
{
    public enum ToastLevel
    {
        Info, Success, Warning, Error
    }
    public class ToastService : IDisposable
    {
        public event Action<string, ToastLevel> OnShow;
        public event Action OnHide;
        private Timer Countdown;

        public void ShowToast(string message, ToastLevel level)
        {
            OnShow?.Invoke(message, level);
            StartCountdown();
        }

        private void StartCountdown()
        {
            SetCountdown();

            if (Countdown.Enabled)
            {
                Countdown.Stop();
                Countdown.Start();
            }
            else
            {
                Countdown.Start();
            }
        }

        private void SetCountdown()
        {
            if (Countdown is null)
            {
                Countdown = new Timer(10000);
                Countdown.Elapsed += HideToast;
                Countdown.AutoReset = false;
            }
        }

        private void HideToast(object sender, ElapsedEventArgs e)
        {
            OnHide?.Invoke();
        }

        public void Dispose() => Countdown?.Dispose();
    }
}