using System.Collections.Generic;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Blockchain.Renderers;
using Libplanet.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Scripts.Actions;
using Scripts.States;

namespace Scripts
{
    // Unity event handlers.
    public class BlockUpdatedEvent : UnityEvent<Block<PolymorphicAction<ActionBase>>>
    {
    }

    public class TotalCountUpdatedEvent : UnityEvent<CountState>
    {
    }

    public class Game : MonoBehaviour
    {
        // Connected to UI elements.
        public Text BlockHashText;
        public Text BlockIndexText;
        public Text AddressText;
        public Text TotalCountText;
        public Text TimerText;
        public Text ScoreBoardText;
        public Click Click;

        private BlockUpdatedEvent _blockUpdatedEvent;
        private TotalCountUpdatedEvent _totalCountUpdatedEvent;
        private IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> _renderers;
        private Agent _agent;
        private Timer _timer;

        // Unity MonoBehaviour Awake().
        public void Awake()
        {
            // General application settings.
            Screen.SetResolution(800, 600, FullScreenMode.Windowed);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);

            // Register listeners.
            _blockUpdatedEvent = new BlockUpdatedEvent();
            _blockUpdatedEvent.AddListener(UpdateBlockTexts);
            _totalCountUpdatedEvent = new TotalCountUpdatedEvent();
            _totalCountUpdatedEvent.AddListener(UpdateTotalCountText);

            // Renderers are called when certain conditions are met.
            // There are different types of renderers called under different conditions.
            // Some are called when a new block is added, some are called when an action is executed.
            _renderers = new List<IRenderer<PolymorphicAction<ActionBase>>>()
            {
                new AnonymousRenderer<PolymorphicAction<ActionBase>>()
                {
                    BlockRenderer = (oldTip, newTip) =>
                    {
                        // FIXME: For a genesis block, this renderer can get called
                        // while Libplanet's internal BlockChain object is not
                        // fully initialized.  This is a haphazard way to bypass
                        // NullReferenceException getting thrown.
                        if (newTip.Index > 0)
                        {
                            _agent.RunOnMainThread(() => _blockUpdatedEvent.Invoke(newTip));
                        }
                    }
                },
                new AnonymousActionRenderer<PolymorphicAction<ActionBase>>()
                {
                    ActionRenderer = (action, context, nextStates) =>
                    {
                        // Invoke the event handler only if the state is updated.
                        if (nextStates.GetState(context.Signer) is Bencodex.Types.Dictionary bdict)
                        {
                            _agent.RunOnMainThread(() => _totalCountUpdatedEvent.Invoke(new CountState(bdict)));
                        }
                    }
                }
            };

            // Initialize a Libplanet Unity Agent.
            _agent = Agent.AddComponentTo(gameObject, _renderers);

            // Initialize a Timer.
            _timer = new Timer();
        }

        // Unity MonoBehaviour Start().
        public void Start()
        {
            // Initialize texts.
            BlockHashText.text = "Block Hash: 0000";
            BlockIndexText.text = "Block Index: 0";
            ScoreBoardText.text = "Scoreboard";

            AddressText.text = $"My Address: {_agent.Address.ToHex().Substring(0, 4)}";
            Bencodex.Types.IValue initialState = _agent.GetState(_agent.Address);
            Debug.Log($"init state is null: {initialState is null}");
            if (initialState is Bencodex.Types.Dictionary bdict)
            {
                _totalCountUpdatedEvent.Invoke(new CountState(bdict));
            }
            else
            {
                _totalCountUpdatedEvent.Invoke(new CountState(0L));
            }

            _timer.ResetTimer();
            TimerText.text = $"Timer: {_timer.Clock:F1}";
        }

        // Unity MonoBehaviour FixedUpdate().
        public void FixedUpdate()
        {
            _timer.Tick();

            // If timer clock reaches zero, count the number of clicks so far
            // and create a transaction containing an action with the click count.
            // Afterwards, reset the timer and the count.
            if (_timer.Clock <= 0)
            {
                if (Click.Count > 0)
                {
                    // ActionBase to PolymorphicAction Casting is done automagically.
                    List<PolymorphicAction<ActionBase>> actions =
                        new List<PolymorphicAction<ActionBase>>()
                        {
                            new ClickAction(Click.Count)
                        };
                    _agent.MakeTransaction(actions);
                }

                Click.ResetCount();
                _timer.ResetTimer();
            }

            // Update timer text.
            TimerText.text = $"Timer: {_timer.Clock:F1}";
        }

        // Update block texts.
        private void UpdateBlockTexts(Block<PolymorphicAction<ActionBase>> tip)
        {
            BlockHashText.text = $"Block Hash: {tip.Hash.ToString().Substring(0, 4)}";
            BlockIndexText.text = $"Block Index: {tip.Index}";
        }

        // Update total count text.
        private void UpdateTotalCountText(CountState countState)
        {
            TotalCountText.text = $"Total Count: {countState.Count}";
        }
    }
}