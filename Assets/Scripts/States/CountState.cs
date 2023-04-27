using Libplanet.Store;

namespace Scripts.States
{
    public class CountState : DataModel
    {
        public long Count { get; private set; }
        public long SpaceEnterCount { get; private set; }

        // Used for creating a new state.
        public CountState(long count, long enterCount)
            : base()
        {
            Count = count;
            SpaceEnterCount = enterCount;
        }

        // Used for deserializing a stored state.
        // This must be declared as base constructor cannot be inherited.
        public CountState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        // Used for adding `count` to the current state.
        public CountState AddCount(long count)
        {
            return new CountState(Count + count, SpaceEnterCount);
        }

        public CountState AddEnterCount(long count)
        {
            return new CountState(Count, SpaceEnterCount + count);
        }
    }
}