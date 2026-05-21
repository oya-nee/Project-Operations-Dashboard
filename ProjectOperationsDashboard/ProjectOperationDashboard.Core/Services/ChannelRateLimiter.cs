namespace ProjectOperationsDashboard.Core.Services
{
    public class ChannelRateLimiter
    {
        private readonly int _maxPerMinute;
        private int _count;
        private DateTime _lastReset = DateTime.UtcNow;
        private readonly object _lock = new();

        public ChannelRateLimiter(int maxPerMinute) => _maxPerMinute = maxPerMinute;

        public bool AllowRequest()
        {
            lock (_lock)
            {
                if ((DateTime.UtcNow - _lastReset).TotalMinutes >= 1) //เวลาใหม่ - เก่า
                {
                    _count = 0;
                    _lastReset = DateTime.UtcNow;
                }

                if (_count >= _maxPerMinute) return false;

                _count++;
                return true;
            }
        }
    }
}
