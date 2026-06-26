# Codex Project Instructions

This repository is public. Do not commit local reference material, binaries, generated build output, or equipment parameter files.

## Required QA Split

All Codex work must separate automated QA from manual QA.

- Automated QA is required for checks Codex can run locally.
- Manual QA is required for real stage hardware, COM port, servo, motion, sensor, and safety checks that Codex cannot directly verify.
- Never report real-hardware QA as complete unless the user has performed it and reported the result.

Follow the detailed policy in `docs/QA_POLICY.md`.

## Automated QA Checklist

Run the relevant automated checks before reporting completion:

- `dotnet build`
- Protocol/test project execution, currently:
  `dotnet run --project .\Alpha5MiniStageController.Protocol.Tests\Alpha5MiniStageController.Protocol.Tests.csproj`
- Compile warning/error review.
- Protocol, CRC, converter, and expected Hex tests.
- `git status --short --branch`
- Confirm ignored/local-only materials are not staged or committed, especially `References/`, `bin/`, `obj/`, `Debug/`, `Release/`, and `.vs/`.

Do not commit if automated tests fail.

## Manual QA Boundaries

SerialPort code, WPF UI, and real hardware behavior must not be treated as fully validated by automated QA alone. When hardware validation is needed, provide the user with a manual QA procedure using the format defined in `docs/QA_POLICY.md`.
