using System;

namespace Bedrot.Narrative.Presentation
{
    public interface INarrativeFlowState { void Enter(); void Exit(); }
    public abstract class NarrativeFlowState : INarrativeFlowState
    {
        private readonly Action _enter; private readonly Action _exit;
        protected NarrativeFlowState(Action enter = null, Action exit = null) { _enter = enter; _exit = exit; }
        public virtual void Enter() => _enter?.Invoke();
        public virtual void Exit() => _exit?.Invoke();
    }
    public sealed class MainMenuState : NarrativeFlowState { public MainMenuState(Action enter = null, Action exit = null) : base(enter, exit) { } }
    public sealed class LoadingNarrativeSceneState : NarrativeFlowState { public LoadingNarrativeSceneState(Action enter = null, Action exit = null) : base(enter, exit) { } }
    public sealed class PresentingDialogueState : NarrativeFlowState { public PresentingDialogueState(Action enter = null, Action exit = null) : base(enter, exit) { } }
    public sealed class WaitingForChoiceState : NarrativeFlowState { public WaitingForChoiceState(Action enter = null, Action exit = null) : base(enter, exit) { } }
    public sealed class ApplyingChoiceState : NarrativeFlowState { public ApplyingChoiceState(Action enter = null, Action exit = null) : base(enter, exit) { } }
    public sealed class TransitioningNarrativeSceneState : NarrativeFlowState { public TransitioningNarrativeSceneState(Action enter = null, Action exit = null) : base(enter, exit) { } }
    public sealed class EndingState : NarrativeFlowState { public EndingState(Action enter = null, Action exit = null) : base(enter, exit) { } }

    public sealed class NarrativeFlowStateMachine
    {
        private INarrativeFlowState _currentState;
        public INarrativeFlowState CurrentState => _currentState;
        public void ChangeState(INarrativeFlowState nextState)
        { if (nextState == null) throw new ArgumentNullException(nameof(nextState)); _currentState?.Exit(); _currentState = nextState; _currentState.Enter(); }
    }
}
