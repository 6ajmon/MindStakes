# Security policy

## Supported versions

Only the latest release and the current `main` branch are supported. Fixes are not backported to older releases.

## Reporting a vulnerability

Please **do not** open a public issue for security problems.

Use GitHub's private vulnerability reporting: <https://github.com/6ajmon/MindStakes/security/advisories/new>. Include:

- the affected version or commit,
- steps to reproduce,
- the impact you expect,
- any suggested fix.

You can expect an acknowledgement within a few days. Please give the maintainer a reasonable window to ship a fix before disclosing the issue publicly.

## What is in scope

- Crashes or memory-safety issues reachable from untrusted input such as question files or category resources.
- Path traversal or arbitrary file writes in the question loader and Question Creator.
- Code execution in exported builds caused by crafted `.tres`/`.json` data.
- Supply-chain risks in the build pipeline.

## Known and accepted limitations

- MindStakes is a client-side game: to reveal the correct answer the running game has to know it. Anyone with full access to the machine running the game can recover the questions and answers. This is a design limitation, not a vulnerability. Keeping question data outside the repository and outside the exported `.pck` is covered in the README.
- The in-game answer timer is enforced locally and can be bypassed by the machine running the game.
