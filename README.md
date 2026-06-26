# Alpha5 Mini Stage Controller

Initial .NET 8 WPF solution for protocol-first Alpha5 Mini Stage Controller work.

The first pass intentionally contains no UI and no serial read/write loop. It only includes:

- Modbus CRC16 calculation.
- Alpha5 command frame generation.
- Unit conversion helpers for a 20 mm lead screw and 20,000 count/rev.
- Hex-string protocol tests before real serial communication is added.

## Projects

- `Alpha5MiniStageController`: WPF app shell targeting `net8.0-windows`.
- `Alpha5MiniStageController.Protocol`: protocol, CRC, and conversion code.
- `Alpha5MiniStageController.Protocol.Tests`: dependency-free console test runner.

## Test

```powershell
dotnet run --project .\Alpha5MiniStageController.Protocol.Tests\Alpha5MiniStageController.Protocol.Tests.csproj
```

This workspace currently has .NET SDK 7 installed, while the project targets .NET 8. Install .NET 8 SDK before building or running tests.

## Protocol Constants

`Alpha5Registers`, `Alpha5Coils`, and `Alpha5CommandCodes` isolate the command map so the values can be compared against the existing `Alpha5_Example` MFC source without reusing its UI/thread structure.
