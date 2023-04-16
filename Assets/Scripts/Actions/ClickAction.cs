using System;
using Libplanet.Action;
using Libplanet.Unity;
using Scripts.States;
using UnityEngine;


namespace Scripts.Actions
{
    [ActionType("click_action")]
    public class ClickAction : ActionBase
    {
        private ClickActionPlainValue _plainValue;

        public ClickAction()
        {
        }

        public ClickAction(long count)
        {
            _plainValue = new ClickActionPlainValue(count);
        }

        public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            if (plainValue is Bencodex.Types.Dictionary bdict)
            {
                _plainValue = new ClickActionPlainValue(bdict);
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
                    : new CountState(0L);

            // Mutates the loaded state, logs the result, and stores the resulting state.
            long prevCount = countState.Count;
            countState = countState.AddCount(_plainValue.Count);
            long nextCount = countState.Count;

            // Also update the scoreboard.
            ScoreBoardState scoreBoardState =
                states.GetState(ScoreBoardState.Address) is Bencodex.Types.Dictionary scoreBoardStateEncoded
                    ? new ScoreBoardState(scoreBoardStateEncoded)
                    : new ScoreBoardState();
            scoreBoardState = scoreBoardState.UpdateScoreBoard(context.Signer);

            return states
                .SetState(ScoreBoardState.Address, scoreBoardState.Encode())
                .SetState(context.Signer, countState.Encode());
        }
    }
}
