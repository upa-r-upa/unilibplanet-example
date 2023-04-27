using Libplanet.Store;

namespace Scripts.Actions
{
    public class ActionPlainValue : DataModel
    {
        public long Count { get; private set; }
        public long SpaceEnterCount { get; private set; }

        public ActionPlainValue(long count, long spaceEnterCount)
            : base()
        {
            Count = count;
            SpaceEnterCount = spaceEnterCount;
        }

        // Used for deserializing stored action.
        public ActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}