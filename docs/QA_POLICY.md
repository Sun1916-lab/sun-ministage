# QA Policy

This project separates Codex-run automated QA from user-run manual QA. The current stage is Protocol/Core/MFC Mapping validation.

## 1. Automated QA Principles

Codex must run every validation it can execute by itself for each task.

Automated QA includes:

- `dotnet build`.
- `dotnet run` or the relevant test project execution.
- Compile warning and error review.
- `Protocol.Tests` execution.
- Expected Hex value comparisons.
- CRC, converter, and protocol unit test verification.
- `git status` review.
- `.gitignore` verification.
- Confirmation that `References/`, `bin/`, `obj/`, `Debug/`, `Release/`, `.vs/`, and other generated or local-only files are not included in commit candidates.
- Re-running relevant tests after documentation or source changes.
- Reviewing the changed file list before commit.

## 2. Automated QA Report Format

Every completion report must include:

- Automated QA commands executed.
- Build result.
- Test result.
- PASS count.
- Warning and error count.
- `git status` summary.
- Files created, modified, or deleted.
- Commit hash, when a commit was created.
- Push status.

## 3. Manual QA Principles

Codex must not mark items it cannot directly verify as PASS. These items must be separated as manual QA and clearly assigned to the user.

Manual QA includes:

- Actual stage power ON/OFF.
- Actual COM port connection.
- Actual Servo On/Off operation.
- Actual Current Position update.
- Actual Origin motion.
- Actual Position Reset operation.
- Small movement of 1 mm or less.
- Actual Jog Left/Right operation.
- Actual Stop button stop behavior.
- Actual Moving Back direction.
- Actual Repeat Move repetition.
- Limit Sensor, Home Sensor, and Alarm state checks.
- Emergency Stop or power cutoff safety checks.
- Interference check before long-distance movement.

## 4. Manual QA Instruction Format

When manual QA is needed, Codex must provide instructions with this structure:

- QA purpose.
- Preconditions and safety conditions.
- Buttons or input values the user must operate directly.
- Expected normal result.
- Stop conditions for abnormal results.
- Results the user must report back.

## 5. Prohibited Actions

- Do not report "real hardware QA complete" without actual equipment verification from the user.
- Do not commit with failing tests.
- Do not commit `References/`.
- Do not commit existing MFC source files.
- Do not commit EXE, ZIP, PPTX, DOCX, XLSX, or A5P files.
- Do not treat SerialPort or WPF UI changes as complete based only on automated QA when real hardware behavior is required.

## 6. Current Project Stage

Current stage: Protocol/Core/MFC Mapping validation.

Current automated QA completion scope:

- `dotnet build`.
- `Protocol.Tests`.
- MFC expected Hex tests.
- `ProtocolMapping.md` documentation.
- `References/` ignore verification.

Current manual QA incomplete scope:

- Actual serial communication.
- Actual stage motion.
- Servo, Origin, Move, Jog, and Stop behavior verification.
