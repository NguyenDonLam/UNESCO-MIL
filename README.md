# Bedrot

Bedrot is a short narrative choice game built in Unity.

The game follows a branching structure inspired by Telltale-style narrative games and dating simulations. The player progresses through authored scenes, speaks with characters, and selects difficult responses that change relationship scores and determine which later subscenes become available.

The game is not based on free movement or exploration. Each scene is presented through a static or lightly animated background, one or more character sprite PNGs, a dialogue textbox, and a choice interface.

## Core Gameplay

```text
Main Menu
→ Scene 1
→ Apply choice effects
→ Update relationship scores
→ Evaluate next-scene conditions
→ Select an eligible subscene
→ Continue the story
```

Scene 1 introduces the characters and presents the first meaningful moral dilemma.

From Scene 2 onward, the game selects subscenes according to the player’s current relationship state and previous decisions.

## Relationship System

Each important character has an independent relationship score.

```text
Character A: 2
Character B: -1
Character C: 3
```

The complete relationship state can be represented as:

```text
(A, B, C) = (2, -1, 3)
```

Every meaningful answer must affect at least one character relationship score. A choice may affect several characters at once.

```text
Choice: Defend Character A publicly

Character A: +2
Character B: -1
Character C: 0
```

Relationship effects should not always be obvious before the player chooses. Choices should create conflict between loyalty, truth, trust, reputation, and wider consequences.

## Scene and Subscene Structure

The story is divided into major scenes:

```text
S1
S2
S3
...
Sn
```

A major scene may contain several conditional subscenes:

```text
S2
├── S2_A_TrustsPlayer
├── S2_BAndC_TrustPlayer
├── S2_AHostile
└── S2_Default
```

The selected subscene depends on the current game state.

```text
Relationship state: (A, B, C) = (1, 0, 0)
→ select an A-focused subscene

Relationship state: (A, B, C) = (0, 1, 1)
→ select a shared B-and-C subscene
```

Subscene conditions should normally use score ranges rather than one exact relationship vector.

```text
A >= 2
B <= 0
C >= 1
```

Each major scene must include a fallback subscene so progression cannot become blocked.

## Scene Selection

When a scene ends, the game:

1. records the selected choice;
2. applies relationship effects;
3. applies story flags and other consequences;
4. retrieves candidate subscenes for the next major scene;
5. evaluates each candidate against the current game state;
6. ranks eligible candidates by priority;
7. selects the highest-priority candidate;
8. presents that subscene.

Scene selection should be deterministic unless randomness is explicitly required.

## Choices

Choices should be difficult and plausible.

Avoid:

```text
A. Good answer
B. Neutral answer
C. Obviously evil answer
```

Prefer:

```text
A. Reveal what happened and betray a friend’s confidence
B. Protect the friend and allow a false interpretation to continue
C. Refuse to take a side and lose trust from both characters
```

Each meaningful choice must define:

- display text;
- relationship effects;
- optional story flags;
- optional immediate response;
- optional direct scene destination;
- optional long-term consequences.

## Presentation

Each scene uses a visual-novel-style composition:

```text
Background image
Character sprite layer
Optional secondary character sprite
Dialogue textbox
Speaker name
Choice panel
Minimal visual effects
```

Character sprites use transparent PNG files.

Animations should remain minimal:

- fade in and fade out;
- sprite position changes;
- small idle movement;
- expression swaps;
- brief screen shake;
- simple transition effects.

## Visual Style

The game uses a pixel-art visual style.

Recommended assets:

- PNG backgrounds;
- transparent PNG character sprites;
- PNG sprite sheets where animation is required;
- nearest-neighbour filtering;
- no compression for important pixel-art assets;
- consistent scale and dimensions.

## Initial Scope

The first playable version should include:

- a main menu;
- one complete opening scene;
- one difficult multiple-choice decision;
- relationship score updates;
- at least two possible Scene 2 subscenes;
- one fallback Scene 2 subscene;
- character sprite presentation;
- background presentation;
- dialogue textbox;
- basic scene transitions;
- local save and restart support.

The initial version does not require:

- free movement;
- combat;
- multiplayer;
- a remote backend;
- runtime generative AI;
- procedural dialogue;
- complex inventory systems;
- voice acting;
- full character animation;
- random scene generation.

## Technology

- Unity
- C#
- ScriptableObjects for narrative authoring
- PNG character sprites and backgrounds
- Local JSON save data

## Project Goal

The goal is to create a compact branching narrative game where difficult decisions reshape relationships and cause later scenes to change.

The project should prioritise:

- morally difficult choices;
- persistent relationship consequences;
- clear but non-obvious character reactions;
- conditional subscene selection;
- deterministic narrative flow;
- reusable scene authoring tools;
- clean separation between narrative logic and Unity presentation.
