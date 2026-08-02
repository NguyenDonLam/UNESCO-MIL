# Project Architecture Instructions

## Project Type

This project is a short first-person Unity narrative choice game focused on Media and Information Literacy.

The player remains in bed and interacts primarily through a fictional social-media feed. Narrative choices affect later scenes, room upgrades or degradation, temporary gameplay modifiers, and eventual endings such as prison.

The architecture must remain clean, modular, testable, and appropriate for a small-to-medium Unity game. Do not treat the project like a distributed backend system.

---

# Core Architectural Direction

Use a feature-based modular monolith with a pure C# game core and Unity-specific adapters around it.

Use the following dependency direction:

```text
Unity Presentation
        ↓
Application / Use Cases
        ↓
Pure C# Domain
        ↑
Infrastructure Adapters
```

The central rule is:

> Narrative rules, progression rules, and game state must not depend on MonoBehaviour, scenes, prefabs, UI components, Animator Controllers, or other Unity APIs.

Unity should render and interact with game state. Unity objects must not become the authoritative source of game state.

---

# Required Architectural Patterns

Use:

- feature-first modular organisation;
- data-driven narrative;
- finite-state machines;
- command handlers for player actions;
- event-driven presentation updates;
- ports and adapters for persistence, content loading, animation, audio, and analytics;
- ScriptableObjects for editor authoring only;
- immutable runtime narrative definitions where practical;
- one authoritative `GameSession`;
- dependency injection through a manual composition root;
- pure C# domain and application tests.

Do not use:

- global mutable singletons;
- `FindObjectOfType`;
- static service locators;
- static event buses;
- scene objects directly mutating progression;
- story logic inside UI buttons;
- one giant `GameManager`;
- one giant Animator Controller;
- direct dependencies between unrelated features;
- microservices;
- full event sourcing;
- backend-style CQRS ceremony for trivial reads;
- a Unity scene for every narrative node.

---

# High-Level Modules

```text
Game
├── Bootstrap
├── Narrative
├── Progression
├── Room
├── Feed
├── Save
├── Audio
├── Presentation
└── Shared
```

## Narrative

Responsible for scenes, dialogue, choices, branching, conditions, effects, narrative progression, and scenario completion.

## Progression

Responsible for room tokens, unlocks, lightweight buffs and debuffs, ending conditions, and prison or other loss states.

## Room

Responsible for room tier, unlocked decorations, degraded room variants, visible room state, and mapping room state to presentation.

## Feed

Responsible for posts displayed on the phone, scrolling order, repeated narratives, missing-context content, alternative or corrective information, and feed behaviour produced by previous choices.

## Save

Responsible for serialization, save slots, autosaving, save versioning, and migrations.

## Audio

Responsible for music, sound effects, semantic audio cues, and the Unity audio implementation.

## Presentation

Responsible for UI, sprite rendering, camera, animation, transitions, player input, and visual effects.

---

# Recommended Folder Structure

```text
Assets/
├── Game/
│   ├── Bootstrap/
│   │   ├── GameBootstrapper.cs
│   │   ├── GameCompositionRoot.cs
│   │   └── SceneInstaller.cs
│   ├── Shared/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   ├── Narrative/
│   │   ├── Domain/
│   │   │   ├── StorySession.cs
│   │   │   ├── NarrativeNode.cs
│   │   │   ├── NarrativeChoice.cs
│   │   │   ├── NarrativeCondition.cs
│   │   │   ├── NarrativeEffect.cs
│   │   │   └── Events/
│   │   ├── Application/
│   │   │   ├── StartScenario/
│   │   │   ├── AdvanceDialogue/
│   │   │   ├── SelectChoice/
│   │   │   └── GetCurrentNarrative/
│   │   ├── Infrastructure/
│   │   │   └── ScriptableObjectNarrativeRepository.cs
│   │   ├── Authoring/
│   │   │   ├── NarrativeNodeAsset.cs
│   │   │   ├── ChoiceAsset.cs
│   │   │   └── ScenarioAsset.cs
│   │   └── Presentation/
│   │       ├── DialogueView.cs
│   │       ├── ChoicePanel.cs
│   │       └── NarrativePresenter.cs
│   ├── Progression/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   ├── Room/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Authoring/
│   │   └── Presentation/
│   ├── Feed/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Authoring/
│   │   └── Presentation/
│   ├── Save/
│   └── Audio/
├── Art/
├── Audio/
├── Scenes/
└── Settings/
```

Use assembly definitions for major layers where useful:

```text
Game.Narrative.Domain
Game.Narrative.Application
Game.Narrative.Infrastructure
Game.Narrative.Presentation
```

Dependency rules:

```text
Presentation → Application → Domain
Infrastructure → Application and Domain
Domain → nothing
```

The domain assembly must not reference Unity assemblies.

---

# Authoritative Runtime State

Use one authoritative `GameSession`.

```csharp
public sealed class GameSession
{
    public StorySession Story { get; }
    public PlayerProgression Progression { get; }
    public RoomState Room { get; }
    public FeedState Feed { get; }

    public GameSession(
        StorySession story,
        PlayerProgression progression,
        RoomState room,
        FeedState feed)
    {
        Story = story;
        Progression = progression;
        Room = room;
        Feed = feed;
    }
}
```

Rules:

- `GameSession` is the source of truth for current game state.
- MonoBehaviours must not own persistent narrative or progression state.
- Scene transitions must not destroy authoritative runtime state.
- Save data must be produced from `GameSession`.
- Presenters may cache view state, but not authoritative game state.

---

# Narrative Architecture

Represent the story as a directed graph.

```csharp
public sealed record NarrativeNode(
    string Id,
    IReadOnlyList<NarrativeBeat> Beats,
    IReadOnlyList<NarrativeChoice> Choices,
    string? AutomaticNextNodeId);
```

```csharp
public sealed record NarrativeChoice(
    string Id,
    string Text,
    string DestinationNodeId,
    IReadOnlyList<ChoiceCondition> Conditions,
    IReadOnlyList<ChoiceEffect> Effects);
```

```csharp
public sealed record NarrativeBeat(
    string SpeakerId,
    string Text,
    string? AnimationCue,
    string? CameraCue,
    string? AudioCue);
```

Each narrative node may contain dialogue or visual beats, optional phone-feed content, choices, entry conditions, effects, destination nodes, semantic animation cues, camera cues, audio cues, and educational tags.

Do not hardcode story progression into MonoBehaviours or UI components.

---

# ScriptableObject Rules

Use ScriptableObjects only for authoring and editor workflows.

```csharp
[CreateAssetMenu(menuName = "Game/Narrative/Node")]
public sealed class NarrativeNodeAsset : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private NarrativeBeatData[] beats;
    [SerializeField] private NarrativeChoiceData[] choices;

    public NarrativeNode ToDomain()
    {
        // Validate and convert to an immutable runtime definition.
    }
}
```

Required flow:

```text
ScriptableObject Authoring Asset
        ↓
Validation
        ↓
Pure Runtime Definition
        ↓
Game Session
```

Rules:

- Never mutate ScriptableObject authoring assets at runtime.
- Never use ScriptableObjects as save-game state.
- Convert assets into pure runtime objects during loading.
- Validate duplicate IDs, missing references, invalid cues, and unreachable nodes.

---

# Application Layer

Use one handler per meaningful player action.

Recommended commands:

```text
StartGameCommand
StartScenarioCommand
AdvanceDialogueCommand
SelectChoiceCommand
PurchaseRoomUpgradeCommand
ApplyRoomDegradationCommand
RestartGameCommand
SaveGameCommand
LoadGameCommand
```

Example:

```csharp
public sealed record SelectChoiceCommand(
    string NodeId,
    string ChoiceId);
```

```csharp
public sealed class SelectChoiceHandler
{
    private readonly GameSession _session;
    private readonly IGameEventPublisher _events;

    public SelectChoiceHandler(
        GameSession session,
        IGameEventPublisher events)
    {
        _session = session;
        _events = events;
    }

    public void Handle(SelectChoiceCommand command)
    {
        var result = _session.Story.SelectChoice(
            command.NodeId,
            command.ChoiceId,
            _session);

        foreach (var gameEvent in result.Events)
        {
            _events.Publish(gameEvent);
        }
    }
}
```

Use lightweight queries when presentation needs prepared read models:

```text
GetCurrentNarrativeView
GetAvailableChoices
GetRoomViewState
GetTokenBalance
GetCurrentFeed
```

Do not add handlers, repositories, or interfaces for trivial private helper logic.

---

# State Machines

Use two separate state machines.

## Game Flow State Machine

```text
Boot
→ MainMenu
→ Loading
→ Playing
→ Paused
→ Ending
→ GameOver
```

```csharp
public interface IGameState
{
    void Enter();
    void Exit();
}
```

## Narrative Presentation State Machine

```text
EnteringScene
→ ShowingDialogue
→ ShowingFeed
→ WaitingForChoice
→ ApplyingChoice
→ ShowingConsequence
→ Transitioning
```

Rules:

- Keep game flow separate from narrative presentation flow.
- Do not create one state for every animation clip.
- Do not combine scene loading, UI state, dialogue state, and animation state into one giant state machine.
- States coordinate behaviour; they do not own all game data.

---

# Event Architecture

Use an injected, non-static event publisher scoped to the current game runtime.

Recommended events:

```text
NarrativeNodeEntered
DialogueBeatStarted
ChoiceSelected
ChoiceEffectsApplied
TokensChanged
RoomUpgradeUnlocked
RoomStateChanged
FeedChanged
ModifierApplied
ModifierExpired
PrisonTriggered
GameEnded
```

Rules:

- Do not use a static event bus.
- Subscribers must be explicitly registered and disposed.
- Events represent completed facts.
- Commands request actions; events report what happened.
- Avoid long event chains that obscure control flow.

---

# Buff and Debuff Architecture

Use a lightweight modifier system. Do not create a large stat system.

```csharp
public interface IGameModifier
{
    string Id { get; }
    ModifierDuration Duration { get; }
}
```

Examples:

```csharp
public sealed record BetterWifiModifier() : IGameModifier
{
    public string Id => "better-wifi";
    public ModifierDuration Duration => ModifierDuration.Permanent;
}
```

```csharp
public sealed record NotificationSpamModifier(int RemainingScenes)
    : IGameModifier
{
    public string Id => "notification-spam";
    public ModifierDuration Duration => ModifierDuration.Temporary;
}
```

Use policies or strategies for behaviour:

```csharp
public interface IChoiceAvailabilityPolicy
{
    IReadOnlyList<NarrativeChoice> Filter(
        IReadOnlyList<NarrativeChoice> choices,
        GameSession session);
}
```

Examples:

- `BetterWifiChoicePolicy` exposes an additional source.
- `NotificationSpamChoicePolicy` inserts a distraction.
- `CrackedPhonePresentationPolicy` obscures metadata.
- `ComfortableBedTimingPolicy` increases decision time.

The domain decides what is available or allowed. Presentation decides how it appears.

---

# Progression and Room Tokens

Use one simple currency: room tokens.

Rules:

- Choices may award or remove tokens.
- Tokens unlock cosmetic room upgrades.
- Some upgrades may provide one small, explicit modifier.
- Negative choices may provide a larger immediate reward but create later consequences.
- Do not create multiple currencies unless explicitly required.

Possible room upgrades:

- pillow;
- blanket;
- lamp;
- plant;
- posters;
- desk;
- better phone;
- second screen;
- window view;
- decorative items.

The room is a visual representation of progression and consequences.

---

# Room State

Keep room state separate from narrative definitions.

```csharp
public sealed class RoomState
{
    private readonly HashSet<string> _unlockedItems = new();

    public RoomTier Tier { get; private set; }

    public IReadOnlyCollection<string> UnlockedItems =>
        _unlockedItems;

    public void Unlock(string itemId)
    {
        _unlockedItems.Add(itemId);
    }

    public void SetTier(RoomTier tier)
    {
        Tier = tier;
    }
}
```

Rules:

- Save item IDs, not GameObject references.
- Keep visual bindings in presentation.
- Do not let room decorations directly modify progression.
- Apply upgrades through use cases or domain methods.

---

# Phone and Doom-Scrolling Presentation

Separate the visual composition into layers:

```text
Room Background
Player Hands Sprite
Phone Frame Sprite
Phone Screen Render Surface
Feed UI
Choice Overlay
Foreground Effects
```

Use a dedicated phone presenter.

```csharp
public sealed class PhonePresenter : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;
    [SerializeField] private FeedView feedView;
    [SerializeField] private ChoicePanel choicePanel;

    public void PlayScroll()
    {
        handAnimator.Play("DoomScroll");
        feedView.ScrollToNextPost();
    }
}
```

Rules:

- The feed system decides which post appears.
- The presentation layer animates scrolling.
- Hand animation must not own feed state.
- Phone content and hand sprite animation must be independently controllable.
- Keep room background separate from hand and phone layers.

---

# Animation Architecture

Use Unity Animator only as a rendering mechanism.

Prefer small isolated controllers:

```text
HandsAnimator
PhoneAnimator
RoomAnimator
EffectsAnimator
```

Narrative data references semantic cues:

```json
{
  "animationCue": "player.scroll"
}
```

Use an adapter:

```csharp
public interface IAnimationCuePlayer
{
    void Play(string cueId);
}
```

Rules:

- Narrative code must not reference Animator parameter names.
- Animation cue IDs must be semantic and stable.
- Validate cue IDs before builds.
- Sprite sheets must use consistent frame sizes and nearest-neighbour filtering.
- Animation state must remain separate from persistent game state.

---

# Save Architecture

Save only pure serializable data.

```csharp
[Serializable]
public sealed class SaveGameData
{
    public int Version;
    public string CurrentNodeId;
    public List<string> SelectedChoiceIds;
    public int Tokens;
    public List<string> RoomItemIds;
    public List<string> ModifierIds;
    public bool IsInPrison;
}
```

Port:

```csharp
public interface ISaveGameStore
{
    void Save(SaveGameData data);
    SaveGameData? Load();
    bool Exists();
    void Delete();
}
```

Initial implementation:

```text
JsonFileSaveGameStore
```

Use `Application.persistentDataPath`.

Every save must contain a version number.

Rules:

- Never serialize MonoBehaviours or GameObjects.
- Never save ScriptableObject references as authoritative state.
- Save stable IDs and primitive values.
- Autosave after meaningful choices and upgrades.
- Keep save mapping separate from domain logic.

---

# Dependency Composition

Create dependencies once in a composition root.

Use manual dependency injection initially. Do not introduce a DI framework unless construction becomes materially difficult.

---

# Scene Structure

Recommended scenes:

```text
Bootstrap.unity
MainMenu.unity
Game.unity
```

Suggested `Game` hierarchy:

```text
GameRoot
├── RoomRoot
├── PhoneRoot
├── UIRoot
├── AudioRoot
├── TransitionRoot
└── DebugRoot
```

Rules:

- Narrative nodes are data, not Unity scenes.
- Do not create one scene per story beat.
- Use additive scenes only for genuinely independent locations or heavy content.
- Scene loading must not reset authoritative runtime state.

---

# Validation Requirements

Validate:

- duplicate node IDs;
- missing destination nodes;
- choices with no destination;
- invalid automatic-next references;
- unreachable nodes;
- missing speaker IDs;
- missing animation cues;
- missing audio cues;
- invalid room item IDs;
- invalid modifier IDs;
- invalid condition types;
- invalid effect payloads;
- circular paths without intended exits.

Expose validation through an editor menu, Play Mode entry checks, and build preprocessing.

Builds should fail on critical narrative validation errors.

---

# Testing Requirements

## Domain Tests

Test without Unity:

```text
Selecting a valid choice moves to the destination node
Unavailable choices are rejected
Choice effects modify tokens correctly
Room upgrades unlock correctly
Modifiers apply and expire correctly
Prison conditions end the game
Repeated choices cannot be selected twice
Save restoration reproduces session state
Invalid narrative references fail validation
```

## Application Tests

Use fake ports:

```text
SelectChoiceHandler publishes expected events
SaveGameHandler calls the save store
StartScenarioHandler loads the correct scenario
PurchaseRoomUpgradeHandler validates cost
LoadGameHandler restores state correctly
```

## Play Mode Tests

Use only for Unity integration:

- UI wiring;
- animations;
- scene transitions;
- sprite rendering;
- visual room bindings;
- input;
- audio cues.

Most game logic must remain testable in Edit Mode as pure C#.

---

# Pattern Usage

| Pattern | Use |
|---|---|
| State Machine | Game flow and narrative presentation phases |
| Command | Player actions |
| Observer / Events | Independent reactions to completed actions |
| Strategy / Policy | Choice filtering and modifier behaviour |
| Repository | Loading story definitions and save data |
| Factory | Creating new or restored sessions |
| Adapter | Unity animation, audio, persistence, and UI |
| Presenter | Mapping runtime state into Unity visuals |
| Composite | Combining narrative conditions |
| Specification | Reusable narrative and choice conditions |

Do not create abstractions merely to claim a pattern is used.

---

# Naming Rules

Use names that describe both domain intent and the architectural pattern being implemented.

## Pattern Names Must Be Explicit

Any class, interface, method, or function that exists primarily to implement a recognised pattern must include that pattern name in its identifier.

Examples:

```text
ScenarioFactory
GameSessionFactory
NarrativeNodeFactory
SaveGameRepository
StoryDefinitionRepository
BetterWifiChoicePolicy
ContextCheckSpecification
GameFlowStateMachine
NarrativePresentationStateMachine
GameEventPublisher
NarrativePresenter
UnityAnimationCueAdapter
```

Do not hide pattern roles behind vague names.

Bad:

```text
ScenarioBuilder
ScenarioCreator
ScenarioService
ScenarioProvider
ScenarioMaker
```

Good:

```text
ScenarioFactory
```

Bad:

```text
StoryStore
StoryDataSource
StoryProvider
```

Good:

```text
StoryRepository
```

Bad:

```text
ChoiceRule
ChoiceFilter
ChoiceChecker
```

Good:

```text
ChoiceAvailabilityPolicy
ChoiceConditionSpecification
```

Bad:

```text
GameFlow
FlowController
GameModeHandler
```

Good:

```text
GameFlowStateMachine
```

## Required Pattern Suffixes

Use these suffixes consistently:

| Pattern | Required naming |
|---|---|
| Factory | `*Factory` |
| Repository | `*Repository` |
| Adapter | `*Adapter` |
| Presenter | `*Presenter` |
| State machine | `*StateMachine` |
| State | `*State` |
| Command | `*Command` |
| Command handler | `*CommandHandler` |
| Query | `*Query` |
| Query handler | `*QueryHandler` |
| Policy | `*Policy` |
| Strategy | `*Strategy` |
| Specification | `*Specification` |
| Event | `*Event` |
| Event publisher | `*EventPublisher` |
| Event subscriber | `*EventSubscriber` |
| Mapper | `*Mapper` |
| Validator | `*Validator` |
| Composition root | `*CompositionRoot` |
| Facade | `*Facade` |
| Decorator | `*Decorator` |
| Composite | `*Composite` |

## Method and Function Naming

Methods and functions that directly express a pattern operation should also reflect that role when the pattern would otherwise be unclear.

Examples:

```csharp
Scenario CreateScenario(...)
GameSession CreateNewSession(...)
GameSession RestoreSession(...)
bool IsSatisfiedBy(GameSession session)
NarrativeChoice ApplyPolicy(...)
SaveGameData MapToSaveData(...)
void PublishEvent(IGameEvent gameEvent)
```

Do not force pattern words into every ordinary domain method.

Good domain methods:

```csharp
storySession.SelectChoice(...)
roomState.Unlock(...)
feedState.Advance(...)
```

Pattern names are required when the type or function exists because of the pattern. They are not required for normal domain behaviour.

## Domain Naming

Use precise domain names:

```text
SelectChoiceCommandHandler
StorySession
NarrativeNode
RoomUpgrade
FeedPost
PrisonTriggeredEvent
BetterWifiModifier
ScenarioFactory
```

Avoid vague names:

```text
Manager
Helper
Utility
Processor
Controller
System
Data
Thing
```

Use `Controller` only for input or orchestration at a presentation boundary.

Use `Manager` only when no more precise domain or pattern name exists.

---

# Coding Rules

- Prefer small cohesive classes.
- Prefer composition over inheritance.
- Keep domain methods intention-revealing.
- Avoid public mutable fields.
- Avoid service locators.
- Avoid hidden static state.
- Avoid reflection-based magic unless required by Unity.
- Fail fast on invalid IDs.
- Use explicit result objects for expected failures.
- Use exceptions for programming errors and invalid configuration.
- Keep UI code free of narrative rules.
- Keep domain code free of Unity APIs.
- Keep authoring data separate from runtime state.
- Keep presentation effects separate from persistent state changes.

---

# Recommended Initial Implementation Order

```text
1. GameSession
2. Narrative graph domain model
3. ScriptableObject narrative authoring assets
4. Narrative validation
5. SelectChoiceHandler
6. NarrativePresenter
7. Scoped GameEventBus
8. JSON save store
9. RoomState and RoomPresenter
10. FeedState and FeedPresenter
11. Game flow state machine
12. Animation cue adapter
13. Token and room upgrade flow
14. Modifier policies
15. Prison ending
```

Do not begin with analytics, a remote backend, runtime AI, or advanced editor tooling.

---

# Required Runtime Flow

```text
Player presses a choice button
        ↓
ChoicePanel emits a choice ID
        ↓
SelectChoiceCommand is dispatched
        ↓
SelectChoiceHandler executes
        ↓
StorySession validates and applies the choice
        ↓
Choice effects update progression, room, and feed
        ↓
Domain events are published
        ↓
Presenters update Unity objects
        ↓
Autosave stores pure session data
```

Any implementation that allows UI components or scene objects to directly modify progression should be rejected.

---

# Scope Control

The initial version is a local single-player game.

Do not add unless explicitly requested:

- multiplayer;
- live social-media integrations;
- web scraping;
- runtime generative AI;
- remote backend;
- cloud saves;
- procedural dialogue;
- complex inventory systems;
- multiple currencies;
- extensive RPG statistics;
- microservices;
- event sourcing.

The architecture must stay clean without becoming enterprise-heavy.
