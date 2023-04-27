using System.Collections.Immutable;
using Libplanet;
using Libplanet.Store;

namespace Scripts.States
{
    public class ScoreBoardState : DataModel
    {
        // Fields are ignored when encoding.
        // As ranking board itself is not tied to a specific account,
        // a hardcoded address to store the state is required.
        public static readonly Address Address = new Address(
            new byte[]
            {
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1,
            });

        // To enforce a state change.  As this class only tracks addresses,
        // changes in scores stored somewhere else do not necessarily affect the list
        // of participants below.
        public long Nonce { get; private set; }
        public long SpaceEnterNonce { get; private set; }

        // The list of addresses that have changed its count at some point.
        public ImmutableList<Address> Participants { get; private set; }

        public ScoreBoardState()
            : base()
        {
            Nonce = 0L;
            SpaceEnterNonce = 0L;
            Participants = ImmutableList<Address>.Empty;
        }

        public ScoreBoardState(long nonce, ImmutableList<Address> participants, long spaceEnterNonce)
        {
            Nonce = nonce;
            SpaceEnterNonce = spaceEnterNonce;
            Participants = participants;
        }

        // Used for deserializing a stored state.
        // This must be declared as base constructor cannot be inherited.
        public ScoreBoardState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        public ScoreBoardState UpdateScoreBoard(Address account, long nonce, long spaceEnterNonce)
        {
            return Participants.Contains(account)
                ? new ScoreBoardState(nonce, Participants, spaceEnterNonce)
                : new ScoreBoardState(nonce, Participants.Add(account), spaceEnterNonce);
        }
    }
}