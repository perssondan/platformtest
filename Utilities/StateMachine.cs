using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace uwpPlatformer.Utilities
{
    public class StateMachine<TTrigger, TState>
        where TState : Enum
        where TTrigger : Enum
    {
        private Action<TTrigger, TState, TState> _onStateChanged;
        private Dictionary<TState, Dictionary<TTrigger, TState>> _states = new Dictionary<TState, Dictionary<TTrigger, TState>>();
        private TState _state;

        public StateMachine(TState initialState)
        {
            _state = initialState;
        }

        /// <summary>
        /// Current state
        /// </summary>
        public TState State => _state;

        /// <summary>
        /// Event triggered when the state changes.
        /// </summary>
        public Action<TTrigger, TState, TState> OnStateChanged { get => _onStateChanged; set => _onStateChanged = value; }

        public void FireTrigger(TTrigger trigger)
        {
            if (!_states.TryGetValue(State, out var transitions))
            {
                throw new InvalidOperationException($"State {State} not configured!");
            }

            if (!transitions.TryGetValue(trigger, out var newState))
            {
                throw new InvalidOperationException($"Trigger {trigger} not allowed in state {State}!");
            }

            TryUpdateState(trigger, State, newState);
        }

        public StateMachine<TTrigger, TState> ConfigureState(TState fromState, TTrigger trigger, TState toState)
        {
            if (!_states.ContainsKey(fromState))
            {
                _states[fromState] = new Dictionary<TTrigger, TState>();
            }

            if (_states[fromState].ContainsKey(trigger))
            {
                throw new InvalidOperationException($"Trigger {trigger} already configured for state {fromState}!");
            }

            _states[fromState].Add(trigger, toState);

            return this;
        }

        private void TryUpdateState(TTrigger trigger, TState currentState, TState newState)
        {
            if (State.Equals(newState))
            {
                return;
            }

            _state = newState;

            Debug.WriteLine($"State changed from {currentState} to {newState} by {trigger}");

            OnStateChanged?.Invoke(trigger, currentState, newState);
        }
    }
}
