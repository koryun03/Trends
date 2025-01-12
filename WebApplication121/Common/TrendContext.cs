using WebApplication121.Models;

namespace WebApplication121.Common
{
    public class TrendContext
    {
        private static readonly Lazy<TrendContext> _instance = new(() => new TrendContext());

        private readonly List<TrendRow> _trend4Store;
        private readonly List<TrendRow> _trend24Store;
        private readonly List<TrendRow> _trend48Store;
        private readonly List<TrendRow> _trend7Store;

        private TrendContext()
        {
            _trend4Store = new List<TrendRow>();
            _trend24Store = new List<TrendRow>();
            _trend48Store = new List<TrendRow>();
            _trend7Store = new List<TrendRow>();
        }

        public static TrendContext Instance => _instance.Value;

        private List<TrendRow> GetStore(string trendType) =>
            trendType switch
            {
                "trend4" => _trend4Store,
                "trend24" => _trend24Store,
                "trend48" => _trend48Store,
                "trend7" => _trend7Store,
                _ => throw new ArgumentException("Invalid trend type", nameof(trendType))
            };

        public void AddRange(string trendType, List<TrendRow> value)
        {
            var store = GetStore(trendType);
            lock (store)
            {
                store.AddRange(value);
            }
        }


        public List<TrendRow> Get(string trendType)
        {
            var store = GetStore(trendType);
            lock (store)
            {
                return new List<TrendRow>(store);
            }
        }

        public void Remove(string trendType)
        {
            var store = GetStore(trendType);
            lock (store)
            {
                store.Clear(); // Clear all items
            }
        }
    }
}
