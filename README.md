# Bedrot

Bedrot is a short first-person narrative choice game built in Unity.

The player spends the game lying in bed and scrolling through content on their phone. Different scenarios appear through social-media posts, messages, videos, headlines, and conversations. The player must choose how to respond based on the information currently available.

Each scenario presents several plausible choices. Decisions may change later dialogue, determine which information becomes available, and lead the story toward different outcomes.

## Core Concept

The game is centred around making decisions while bedrotting and doom-scrolling.

The player does not freely move around the environment. The bedroom, bed, hands, and phone form the main visual composition. Most interactions happen through the phone interface or through events visible from the bed.

The primary gameplay loop is:

```text
View content
→ review the available information
→ select a response or action
→ observe the consequence
→ continue to the next scenario
```

## Gameplay

During a scenario, the player may encounter:

- short-form videos;
- social-media posts;
- private messages;
- news headlines;
- images;
- comments;
- conversations with other characters.

The player selects one option from a set of choices. Choices should not be presented as simple correct or incorrect answers. Each option should appear reasonable based on what the player knows at that moment.

Later scenes may reveal additional information that changes the meaning of an earlier event or decision.

## Narrative Structure

The game uses a branching narrative structure.

A scenario may contain:

- an opening scene;
- one or more narrative beats;
- phone content;
- dialogue;
- a multiple-choice decision;
- a consequence;
- a destination leading to the next narrative node.

Previous choices may affect:

- later dialogue;
- which posts appear;
- how characters respond;
- which choices are available;
- the final outcome.

## Perspective

The game is presented from a first-person perspective.

The player generally remains unseen. The visible composition may include:

- the bedroom;
- the bed and blankets;
- the player’s hands;
- the phone;
- phone-screen content;
- environmental events occurring in front of the player.

The scene should feel as though the player is lying in bed and looking directly ahead while using their phone.

## Visual Style

The game uses a pixel-art visual style.

Visual elements are separated into layers so they can be animated and replaced independently:

```text
Bedroom background
Bed
Hands
Phone
Thumb
Phone-screen interface
Foreground effects
```

Sprites should use PNG format, consistent dimensions, transparent backgrounds where required, and nearest-neighbour filtering in Unity.

## Initial Scope

The first version should focus on:

- one bedroom environment;
- a first-person phone interface;
- a small set of authored scenarios;
- one meaningful multiple-choice decision per scenario;
- branching narrative outcomes;
- basic hand and scrolling animations;
- local save and restart support.

The initial version does not require:

- free movement;
- combat;
- multiplayer;
- a remote backend;
- live social-media integration;
- procedural dialogue;
- runtime generative AI;
- user-generated scenarios;
- complex progression systems.

## Technology

- Unity
- C#
- ScriptableObjects for narrative authoring
- PNG sprites and sprite sheets
- Local JSON save data

## Project Goal

The goal is to create a compact narrative experience in which the player repeatedly makes decisions from bed while consuming information through their phone.

The project should prioritise:

- believable choices;
- short and focused scenarios;
- clear consequences;
- consistent first-person presentation;
- reusable narrative and visual systems;
- clean separation between game logic and Unity presentation.
