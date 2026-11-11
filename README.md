<img width="1909" height="1071" alt="image" src="https://github.com/user-attachments/assets/59abccb0-1bba-4b69-a4fd-07afbf391f9b" />

# MindStakes

[![CI](https://github.com/6ajmon/MindStakes/actions/workflows/ci.yml/badge.svg)](https://github.com/6ajmon/MindStakes/actions/workflows/ci.yml)
[![Godot export](https://github.com/6ajmon/MindStakes/actions/workflows/godot-export.yml/badge.svg)](https://github.com/6ajmon/MindStakes/actions/workflows/godot-export.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![Godot 4.6.1 .NET](https://img.shields.io/badge/Godot-4.6.1%20.NET-478cbf?logo=godotengine&logoColor=white)](https://godotengine.org)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)

MindStakes is a tool for quiz show hosts, made in Godot.

# Features:
- track participants' scores
- get random categories
- get random, non-repeating questions
- track answer time
- use questions with audio and sound
- choose from three game modes
- use question pools

#  Game modes
## Betting
- The current player selects a category. They can either keep it or reroll it.
- Every player bets points, up to 20.
- The question and answers are revealed. Players have limited time to pick their answer.
- The correct answer is revealed, and players either lose or gain the amount of points they bet.
## Fraud
- The current player selects a category. They can either keep it or reroll it.
- Question and answers are revealed.
- The current player states whether they know the answer.
- If they say no, they lose only 10 points.
- If they say yes, other players can challenge them.
- If no one challenges them, the current player gets 20 points, whether they know the answer or not.
- If someone decides to challenge, the current player has to answer the question.
- If they answer correctly, they gain 20 points, and the challenging player loses 10.
- Otherwise, they lose 20 points, and the challenging player gets 10.
## Sabotage
- The current player selects a category. They can either keep it or reroll it.
- Question and answers are revealed.
- The current player can either answer the question or give it to another player.
- If they answer, they gain or lose 20 points based on their answer.
- If they give the question to another player, the selected player gains or loses 20 points based on their answer.
- If the selected player answers correctly, the current player loses 10 points. If the answer is incorrect, the current player gains 10 points.

# Question Creator
- Create new questions.
- Edit existing questions.
- Save questions after creating or updating them.
- Modify existing question entries whenever needed.

# Question data
Question sets are **not** part of this repository. Only a two-question sample pool (`Resources/Questions/Pool1/1.tres` and `2.tres`) is tracked; every other file under `Resources/Questions/Pool*` is ignored by Git.

The question folder is resolved at startup, in this order:

1. `--questions-dir=<path>` user command line argument (passed after `--`).
2. `MINDSTAKES_QUESTIONS_DIR` environment variable.
3. `mindstakes/questions_dir` project setting (Project Settings → Mindstakes → Questions Dir).
4. `<folder of the executable>/questions`, in exported builds, when the folder exists.
5. `user://questions`, when it exists.
6. `res://Resources/Questions` (fallback).

Options 1–3 accept any folder, including one outside the project. To keep answers out of the repository *and* out of exported builds, point them at a folder outside the project:

```powershell
$env:MINDSTAKES_QUESTIONS_DIR = "C:\MindStakes-data\questions"
godot --path "C:\Github\MindStakes"
```

Start Godot (or the exported game) from that shell. The Question Creator writes new questions to the same folder, so nothing lands in `Resources/Questions` while it is set.

If the folder stays on `res://`, the files present on disk are packed into the exported game. Either keep the data outside the project or add `Resources/Questions/*` to the export preset's *exclude filter*.

A client-side game has to know the correct answer in order to reveal it, so anyone with full access to the machine running the game can recover the data. Keeping it out of the repository and out of the exported `.pck` is what prevents accidental disclosure.

### This project is still a work in progress.

What I want to add:
- custom category creator
- import and export for questions
- dual-language support
- custom game modes creator

# Download

Every tagged release ships prebuilt Windows and Linux builds. Each push and pull request also produces downloadable build artifacts in the [Godot export](https://github.com/6ajmon/MindStakes/actions/workflows/godot-export.yml) workflow.

# Building from source

Requirements:

- [Godot 4.6.1 **.NET**](https://godotengine.org/download) (the standard build cannot run C#),
- [.NET 8 SDK](https://dotnet.microsoft.com/download),
- Git LFS, because images and audio are stored with LFS.

```powershell
git lfs install
git clone https://github.com/6ajmon/MindStakes.git
cd MindStakes
dotnet build MindStakes.sln
```

Then open `project.godot` in the Godot .NET editor and press <kbd>F5</kbd>.

# Contributing

Bug reports, feature requests and pull requests are welcome. Start with the issues, follow [CONTRIBUTING.md](./CONTRIBUTING.md) for setup, branching and code style, and read [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md) before participating. Security issues go through [SECURITY.md](./SECURITY.md) - never a public issue.

# License

MindStakes is released under the [MIT License](./LICENSE).


<details>
<summary>Screenshots (WIP)</summary> 

<img width="1914" height="1076" alt="image" src="https://github.com/user-attachments/assets/9645f8fa-c4e3-4a6b-974a-2ef2a1938c06" />

<img width="1918" height="1085" alt="image" src="https://github.com/user-attachments/assets/653fb2a0-da0b-47b7-aed6-6a9c148ebd18" />

<img width="1909" height="1072" alt="image" src="https://github.com/user-attachments/assets/4cd0bc68-ae22-4cb2-8c51-f8e5666dfd6f" />

<img width="1921" height="1074" alt="image" src="https://github.com/user-attachments/assets/a81bf19f-57db-4cfe-958a-d8b835b8d86b" />
  
<img width="1910" height="1062" alt="image" src="https://github.com/user-attachments/assets/4c24d146-0e4f-453d-a7ff-4043006f2df0" />

<img width="1909" height="1068" alt="image" src="https://github.com/user-attachments/assets/1fa22950-4cb4-4cbf-ba11-37e3836d8dec" />

<img width="1911" height="1078" alt="image" src="https://github.com/user-attachments/assets/842f7a0e-1733-4582-85c1-55f13ac40934" />

</details>
