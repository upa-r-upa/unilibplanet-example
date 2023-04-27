using System;
using Libplanet.Action;
using Libplanet.Unity;
using Scripts.States;
using UnityEngine;


namespace Scripts.Actions
{
    [ActionType("normal_action")]
    public class NormalAction : ActionBase
    {
        private ActionPlainValue _plainValue;

        public NormalAction()
        {
        }

        public NormalAction(long count, long enterNonce)
        {
            _plainValue = new ActionPlainValue(count, enterNonce);
        }

        public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            if (plainValue is Bencodex.Types.Dictionary bdict)
            {
                _plainValue = new ActionPlainValue(bdict);
            }
            else
            {
                throw new ArgumentException(
                    $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
            }
        }

        public override IAccountStateDelta Execute(IActionContext context)
        {
            // Retrieves the previously stored state.
            IAccountStateDelta states = context.PreviousStates;
            CountState countState =
                states.GetState(context.Signer) is Bencodex.Types.Dictionary countStateEncoded
                    ? new CountState(countStateEncoded)
                    : new CountState(0L, 0L);

            // Mutates the loaded state, logs the result, and stores the resulting state.
            countState = countState.AddCount(_plainValue.Count);
            countState = countState.AddEnterCount(_plainValue.SpaceEnterCount);
            long nextCount = countState.Count;
            long nextEnterCount = countState.SpaceEnterCount;

            Debug.LogError($"nextCount: {nextCount}");
            Debug.LogError($"nextEnterCount: {nextEnterCount}");

            // Also update the scoreboard.
            ScoreBoardState scoreBoardState =
                states.GetState(ScoreBoardState.Address) is Bencodex.Types.Dictionary scoreBoardStateEncoded
                    ? new ScoreBoardState(scoreBoardStateEncoded)
                    : new ScoreBoardState();
            scoreBoardState = scoreBoardState.UpdateScoreBoard(context.Signer, nextCount, nextEnterCount);

            return states
                .SetState(ScoreBoardState.Address, scoreBoardState.Encode())
                .SetState(context.Signer, countState.Encode());
        }
    }
}
