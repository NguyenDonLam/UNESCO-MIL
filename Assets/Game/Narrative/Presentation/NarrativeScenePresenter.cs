using System;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    public sealed class NarrativeScenePresenter : MonoBehaviour
    {
        [SerializeField] private DialoguePresenter dialoguePresenter;
        [SerializeField] private ChoicePresenter choicePresenter;
        [SerializeField] private CharacterSpritePresenter characterSpritePresenter;
        [SerializeField] private BackgroundPresenter backgroundPresenter;
        [SerializeField] private TransitionPresenter transitionPresenter;
        private NarrativeScene _scene; private int _beatIndex; private Action<ChoiceId> _selectChoice;
        private IGameEventPublisher _events; private readonly NarrativeFlowStateMachine _stateMachine = new();
        public NarrativeScene CurrentScene => _scene;
        public int CurrentBeatIndex => _beatIndex;

        public void Bind(Action<ChoiceId> selectChoice, IGameEventPublisher events) { _selectChoice = selectChoice; _events = events; }
        public void PresentScene(NarrativeScene scene)
        {
            _scene = scene ?? throw new ArgumentNullException(nameof(scene)); _beatIndex = 0;
            choicePresenter.HideChoices(); transitionPresenter?.SetVisible(false);
            _stateMachine.ChangeState(new LoadingNarrativeSceneState());
            if (_scene.Beats.Count == 0) { ShowChoicesOrEnd(); return; }
            PresentCurrentBeat();
        }
        public void AdvanceDialogue()
        {
            if (_scene == null || _stateMachine.CurrentState is WaitingForChoiceState || _stateMachine.CurrentState is EndingState) return;
            if (++_beatIndex < _scene.Beats.Count) PresentCurrentBeat(); else ShowChoicesOrEnd();
        }
        private void PresentCurrentBeat()
        {
            _stateMachine.ChangeState(new PresentingDialogueState());
            NarrativeBeat beat = _scene.Beats[_beatIndex]; dialoguePresenter.PresentBeat(beat);
            characterSpritePresenter?.PresentBeat(beat); backgroundPresenter?.PresentBeat(beat);
            _events?.Publish(new NarrativeBeatChangedEvent(_scene.Id, _beatIndex));
        }
        private void ShowChoicesOrEnd()
        {
            if (_scene.Choices.Count == 0) { _stateMachine.ChangeState(new EndingState()); return; }
            _stateMachine.ChangeState(new WaitingForChoiceState());
            choicePresenter.PresentChoices(_scene.Choices, SelectChoice);
        }
        private void SelectChoice(ChoiceId id)
        {
            _stateMachine.ChangeState(new ApplyingChoiceState()); choicePresenter.HideChoices();
            _stateMachine.ChangeState(new TransitioningNarrativeSceneState()); transitionPresenter?.SetVisible(true); _selectChoice?.Invoke(id);
        }
    }
}
