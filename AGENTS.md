# Bedrot Project Instructions

## Project Definition

Bedrot is a Unity narrative choice game inspired by Telltale-style games and dating simulations.

The game consists of authored major scenes and conditional subscenes. The player reads dialogue, observes character sprites over a background, and selects choices. Every meaningful choice must affect at least one character relationship score.

From Scene 2 onward, subscene selection is based primarily on relationship scores, previous choices, and story flags.

Presentation consists of:

```text
Background
Character sprite PNGs
Dialogue textbox
Speaker name
Choice interface
Minimal animation
```

Do not implement free movement, combat, room-upgrade systems, or unnecessary world simulation unless explicitly requested.

---

# Core Runtime Flow

```text
Main Menu
→ Start Scene 1
→ Present dialogue
→ Present difficult choice
→ SelectChoiceCommand
→ Apply relationship and story effects
→ Complete current scene
→ Select next eligible subscene
→ Present next subscene
```

The next scene must be selected from game state. UI components must not directly decide narrative progression.

---

# Architectural Style

Use a feature-based modular monolith with:

- a pure C# domain layer;
- an application layer containing use cases;
- Unity presentation adapters;
- ScriptableObjects for authoring;
- JSON for save data.

Dependency direction:

```text
Unity Presentation
        ↓
Application
        ↓
Domain
        ↑
Infrastructure
```

The domain must not reference `MonoBehaviour`, `GameObject`, `ScriptableObject`, Unity scenes, Animator Controllers, UI components, or Unity APIs.

---

# Required Patterns

| Concern | Pattern |
|---|---|
| Creating scenarios | Factory |
| Loading scene definitions | Repository |
| Evaluating subscene conditions | Specification |
| Combining conditions | Composite Specification |
| Selecting among eligible scenes | Strategy |
| Player actions | Command |
| Handling player actions | Command Handler |
| Runtime presentation modes | State |
| Switching runtime modes | State Machine |
| Updating views | Observer / Events |
| Mapping state into UI | Presenter |
| Unity-specific implementations | Adapter |
| Saving and restoring state | Memento |
| Constructing dependencies | Composition Root |

Any class, interface, method, or function that primarily implements a recognised pattern must include that pattern name.

Required examples:

```text
ScenarioFactory
NarrativeSceneRepository
RelationshipScoreSpecification
AllSceneConditionsCompositeSpecification
HighestPriorityNarrativeSceneSelectionStrategy
SelectChoiceCommand
SelectChoiceCommandHandler
NarrativeFlowStateMachine
WaitingForChoiceState
NarrativeScenePresenter
UnityCharacterSpriteAdapter
GameSessionMemento
GameCompositionRoot
```

The scenario factory must be named `ScenarioFactory`.

---

# Authoritative Game State

Use one authoritative `GameSession`.

```csharp
public sealed class GameSession
{
    public NarrativeProgressState NarrativeProgress { get; }
    public RelationshipState Relationships { get; }
    public ChoiceHistoryState ChoiceHistory { get; }
    public StoryFlagState StoryFlags { get; }
}
```

Rules:

- `GameSession` is the single source of truth.
- Relationship scores must not be stored in UI components.
- Current scene IDs must not be inferred from active GameObjects.
- Save data must be created from `GameSession`.
- Scene transitions must not destroy persistent state.

---

# Relationship Model

Each character has an independent integer score.

```csharp
public sealed class RelationshipState
{
    private readonly Dictionary<CharacterId, int> _scores = new();

    public int GetScore(CharacterId characterId)
    {
        return _scores.GetValueOrDefault(characterId);
    }

    public void ChangeScore(CharacterId characterId, int amount)
    {
        _scores[characterId] = GetScore(characterId) + amount;
    }
}
```

Scores are not binary.

```text
(A, B, C) = (1, 0, 0)
(A, B, C) = (0, 1, 1)
(A, B, C) = (3, -2, 4)
```

Every meaningful choice must define at least one relationship effect.

```csharp
public sealed record RelationshipScoreChoiceEffect(
    CharacterId CharacterId,
    int Amount) : IChoiceEffect;
```

Do not place score mutations inside button click listeners.

---

# Narrative Scene Model

Use `NarrativeScene`, not `Scene`, for story content.

```csharp
public sealed record NarrativeScene(
    NarrativeSceneId Id,
    MajorSceneId MajorSceneId,
    IReadOnlyList<NarrativeBeat> Beats,
    IReadOnlyList<NarrativeChoice> Choices,
    IReadOnlyList<NarrativeSceneTransition> Transitions);
```

```csharp
public sealed record NarrativeBeat(
    CharacterId? SpeakerId,
    string Text,
    string? CharacterSpriteCue,
    string? BackgroundCue,
    string? AnimationCue,
    string? AudioCue);
```

```csharp
public sealed record NarrativeChoice(
    ChoiceId Id,
    string Text,
    IReadOnlyList<IChoiceEffect> Effects,
    NarrativeSceneId? DirectDestinationSceneId);
```

A major scene may contain several conditional subscenes.

```text
S2
├── S2_A_Trusting
├── S2_BC_Allied
├── S2_A_Hostile
└── S2_Default
```

Use score ranges and combined conditions. Do not create a subscene for every exact score vector.

---

# Scenario Factory

All runtime scenario construction must go through `ScenarioFactory`.

```csharp
public sealed class ScenarioFactory
{
    public NarrativeScene CreateScenario(
        NarrativeSceneAsset asset)
    {
        // Validate authoring data.
        // Convert it into pure runtime objects.
        // Return an immutable NarrativeScene.
    }
}
```

`ScenarioFactory` constructs scenarios. It does not select which scenario runs next.

---

# Conditional Subscene Selection

Use the Specification pattern.

```csharp
public interface INarrativeSceneSpecification
{
    bool IsSatisfiedBy(GameSession gameSession);
}
```

Required specifications may include:

```text
MinimumRelationshipScoreSpecification
MaximumRelationshipScoreSpecification
RelationshipScoreRangeSpecification
PreviousChoiceSelectedSpecification
StoryFlagSetSpecification
NarrativeSceneCompletedSpecification
AlwaysSatisfiedSpecification
```

Composite specifications:

```text
AllSceneConditionsCompositeSpecification
AnySceneConditionsCompositeSpecification
NotSceneConditionCompositeSpecification
```

Specifications read from `GameSession`. They must not query Unity views.

---

# Scene Selection Strategy

```csharp
public interface INarrativeSceneSelectionStrategy
{
    NarrativeSceneDefinition SelectScene(
        IReadOnlyList<NarrativeSceneDefinition> candidates,
        GameSession gameSession);
}
```

Initial implementation:

```text
HighestPriorityNarrativeSceneSelectionStrategy
```

Selection flow:

1. retrieve candidates for the next major scene;
2. retain candidates whose specifications pass;
3. order by priority descending;
4. use a deterministic tie-breaker;
5. select the winner;
6. fall back to an always-eligible default scene.

Every major scene after S1 must have a fallback subscene.

Do not use randomness unless explicitly requested.

---

# Choice Effects

```csharp
public interface IChoiceEffect
{
    void Apply(GameSession gameSession);
}
```

Required examples:

```text
RelationshipScoreChoiceEffect
SetStoryFlagChoiceEffect
RemoveStoryFlagChoiceEffect
RecordChoiceChoiceEffect
CompleteNarrativeSceneChoiceEffect
```

A choice may apply multiple effects.

```text
Choice: Tell Character A the truth

A: +2
B: -1
Set flag: truth_revealed
```

Effect classes must include `ChoiceEffect` in their names.

---

# Commands and Command Handlers

Required initial commands:

```text
StartNewGameCommand
SelectChoiceCommand
CompleteNarrativeSceneCommand
SelectNextNarrativeSceneCommand
SaveGameCommand
LoadGameCommand
RestartGameCommand
```

Required handlers:

```text
StartNewGameCommandHandler
SelectChoiceCommandHandler
CompleteNarrativeSceneCommandHandler
SelectNextNarrativeSceneCommandHandler
SaveGameCommandHandler
LoadGameCommandHandler
RestartGameCommandHandler
```

UI handlers may dispatch commands. They must not apply domain effects themselves.

---

# Runtime State Machine

Use:

```text
MainMenuState
LoadingNarrativeSceneState
PresentingDialogueState
WaitingForChoiceState
ApplyingChoiceState
TransitioningNarrativeSceneState
EndingState
```

```csharp
public interface INarrativeFlowState
{
    void Enter();
    void Exit();
}
```

```csharp
public sealed class NarrativeFlowStateMachine
{
    private INarrativeFlowState? _currentState;

    public void ChangeState(INarrativeFlowState nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }
}
```

The state machine controls current behaviour. It must not replace `GameSession`.

---

# Events

Use an injected, non-static event publisher.

Examples:

```text
NewGameStartedEvent
NarrativeSceneEnteredEvent
NarrativeBeatChangedEvent
ChoiceSelectedEvent
RelationshipScoreChangedEvent
NarrativeSceneCompletedEvent
NarrativeSceneSelectedEvent
GameEndedEvent
```

Commands request actions. Events report completed facts.

---

# Presentation Architecture

Use presenters:

```text
NarrativeScenePresenter
CharacterSpritePresenter
BackgroundPresenter
DialoguePresenter
ChoicePresenter
RelationshipDebugPresenter
```

Suggested hierarchy:

```text
NarrativeSceneRoot
├── BackgroundView
├── LeftCharacterView
├── RightCharacterView
├── DialogueView
│   ├── SpeakerNameText
│   └── DialogueText
├── ChoiceView
└── TransitionView
```

Presenters do not evaluate narrative conditions.

---

# Character Sprite System

Use transparent PNG sprites with semantic cues:

```text
characterA.neutral
characterA.concerned
characterA.angry
characterA.smiling
```

Narrative beats reference cues, not asset paths.

```csharp
public interface ICharacterSpriteAdapter
{
    void ShowSprite(string spriteCue);
    void HideSprite();
}
```

Unity-specific rendering belongs in `UnityCharacterSpriteAdapter`.

---

# ScriptableObject Authoring

Use ScriptableObjects for:

- narrative scenes;
- dialogue beats;
- choices;
- relationship effects;
- transition conditions;
- character sprite catalogues;
- background catalogues.

Required flow:

```text
NarrativeSceneAsset
→ ScenarioFactory
→ NarrativeScene
→ GameSession
```

Never use ScriptableObjects as runtime save state.

---

# Save Architecture

Use:

```text
GameSessionMemento
GameSessionMementoFactory
JsonSaveGameRepository
```

```csharp
[Serializable]
public sealed class GameSessionMemento
{
    public int Version;
    public string CurrentNarrativeSceneId;
    public Dictionary<string, int> RelationshipScores;
    public List<string> SelectedChoiceIds;
    public List<string> StoryFlags;
    public List<string> CompletedNarrativeSceneIds;
}
```

Save stable IDs and primitive values only.

---

# Main Menu to Demo Scene

Initial flow:

```text
MainMenu.unity
→ Start button
→ StartNewGameCommand
→ Game.unity
→ load S1
```

Prefer one `Game.unity` Unity scene for all narrative content.

Narrative scenes are data and presentation changes inside `Game.unity`.

Do not create one Unity scene file for every dialogue scene.

---

# Validation

Create `NarrativeSceneValidator`.

Validate:

- duplicate scene IDs;
- missing major-scene IDs;
- missing fallback subscenes;
- invalid destination IDs;
- choices without relationship effects;
- missing character IDs;
- missing sprite cues;
- missing background cues;
- invalid specifications;
- unreachable scenes;
- ambiguous equal-priority scenes.

Critical errors must block builds.

---

# Testing Requirements

Domain tests:

```text
Choice effects update relationship scores
Negative and positive scores are supported
Score-range specifications evaluate correctly
Composite specifications combine correctly
Highest-priority eligible subscene is selected
Fallback subscene is selected when required
Previous choices affect later selection
Save restoration recreates relationship state
```

Application tests:

```text
SelectChoiceCommandHandler applies every effect
SelectNextNarrativeSceneCommandHandler selects the expected scene
StartNewGameCommandHandler starts S1
SaveGameCommandHandler creates a GameSessionMemento
```

Play Mode tests should cover only Unity integration.

---

# Naming Rules

Any type implementing a pattern must include the pattern name.

Required examples:

```text
ScenarioFactory
NarrativeSceneRepository
HighestPriorityNarrativeSceneSelectionStrategy
MinimumRelationshipScoreSpecification
AllSceneConditionsCompositeSpecification
SelectChoiceCommandHandler
NarrativeFlowStateMachine
WaitingForChoiceState
NarrativeScenePresenter
UnityCharacterSpriteAdapter
GameSessionMementoFactory
GameCompositionRoot
```

Avoid vague names such as:

```text
GameManager
SceneManager
StoryHelper
ChoiceUtility
RelationshipProcessor
ScenarioService
```

---

# Initial Implementation Order

```text
1. CharacterId and NarrativeSceneId
2. RelationshipState
3. GameSession
4. NarrativeScene, NarrativeBeat, NarrativeChoice
5. IChoiceEffect and RelationshipScoreChoiceEffect
6. INarrativeSceneSpecification implementations
7. HighestPriorityNarrativeSceneSelectionStrategy
8. ScenarioFactory
9. ScriptableObject authoring assets
10. SelectChoiceCommandHandler
11. SelectNextNarrativeSceneCommandHandler
12. NarrativeFlowStateMachine
13. NarrativeScenePresenter
14. Main menu connection to S1
15. GameSessionMemento and JSON saving
```

---

# Scope Restrictions

Do not add unless explicitly requested:

- free movement;
- combat;
- room upgrades;
- dopamine reward systems;
- currencies;
- inventory;
- multiplayer;
- remote backend;
- runtime AI;
- procedural story generation;
- voice acting;
- complex animation systems.

The initial goal is a clean branching narrative demo driven by relationship scores and difficult choices.
