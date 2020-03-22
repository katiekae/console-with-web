namespace ConsoleWithWeb
{
    /// <summary>
    /// For communicating between the console project's main loop & the web server
    /// </summary>
    public class BridgeService
    {
        private double? _latestValue;
        private readonly object _latestValueLock = new object();
        
        public void SetLatestValue(in double d)
        {
            lock (_latestValueLock)
            {
                _latestValue = d;
            }
        }

        public double? LatestValue
        {
            get
            {
                lock (_latestValueLock)
                {
                    return _latestValue;
                }
            }
        }
    }
}