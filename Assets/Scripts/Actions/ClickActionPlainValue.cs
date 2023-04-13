using Libplanet.Store;

namespace Scripts.Actions
{
    public class ClickActionPlainValue : DataModel
    {
        public long Count { get; private set; }

        public ClickActionPlainValue(long count)
            : base()
        {
            Count = count;
        }

        // Used for deserializing stored action.
        public ClickActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}