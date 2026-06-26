# Alpha5 MFC Protocol Mapping

This document maps the local MFC reference command builders to the C# `Alpha5Protocol` output.

Reference source is local only and ignored by Git:

- `References/Alpha5_Example_20230712/Alpha5_ExampleDlg.cpp`

The MFC default slave id is `01`. CRC is Modbus CRC16 appended low byte first, matching the MFC `CRCCalculation` byte swap.

## Mapping

| Command name | MFC source location | Register / Address | Payload structure | C# generated Hex | Match | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| ReadCurrentPosition | `Alpha5_ExampleDlg.cpp:1622` | `0x1006`, read `0x0002` holding registers | `01 03 1006 0002 CRC` | `01031006000220CA` | Yes | MFC timer monitoring reads current position from `1006`. |
| ServoOn | `Alpha5_ExampleDlg.cpp:1183` | `0x0000`, write `0x0002` registers | `00000001` control word | `0110000000020400000001326F` | Yes | Uses FC10, not FC05 coil write. |
| ServoOff | `Alpha5_ExampleDlg.cpp:1203` | `0x0000`, write `0x0002` registers | `00000000` control word | `0110000000020400000000F3AF` | Yes | Clears servo state bit. |
| Origin | `Alpha5_ExampleDlg.cpp:1225`, `Alpha5_ExampleDlg.cpp:1230` | `0x0000`, write `0x0002` registers | `00000011` when servo is on, `00000010` when servo is off | `011000000002040000001133A3` / `0110000000020400000010F263` | Yes | C# exposes `Origin(servoOn)` because MFC ORs the origin bit with the current servo state. |
| PositionReset | `Alpha5_ExampleDlg.cpp:1245`, `Alpha5_ExampleDlg.cpp:1260` | `0x0000`, write `0x0002` registers | reset bit then restore servo-only state | `0110000000020400000041339F` -> `0110000000020400000001326F` | Yes | MFC sends two frames when servo is on. Servo-off variant is also covered by tests. |
| Stop | `Alpha5_ExampleDlg.cpp:1289`, `Alpha5_ExampleDlg.cpp:1307` | `0x0000`, write `0x0002` registers | stop bit then position-cancel bit | `011000000002040000002133B7` -> `011000000002040000008133CF` | Yes | MFC sends two frames for the Stop button. Servo-off variant is also covered by tests. |
| PositionMove | `Alpha5_ExampleDlg.cpp:1384`, `Alpha5_ExampleDlg.cpp:1392`, `Alpha5_ExampleDlg.cpp:1406` | `0x5100`, write `0x000A` registers; then `0x0000` control | `01000000` + position + speed + acc + dec, then start bit, then restore servo bit | `01105100000A14010000000000C35000002710000003E8000003E805EF` -> `011000000002040000000933A9` -> `0110000000020400000001326F` | Yes | Test uses MFC defaults: position `50000` counts = `50.0 mm`, speed `100` -> `00002710`, acc/dec `100` -> `000003E8`. |
| JogLeftStart | `Alpha5_ExampleDlg.cpp:1449`, `Alpha5_ExampleDlg.cpp:1460` | `0x4028`, write jog speed; then `0x0000` control | speed value, then left jog bit with servo state | `0110402800020400002710DBEE` -> `0110000000020400000105323C` | Yes | Test uses MFC default speed `100` -> `00002710`. Servo-off variant is also covered by tests. |
| JogRightStart | `Alpha5_ExampleDlg.cpp:1481`, `Alpha5_ExampleDlg.cpp:1491` | `0x4028`, write jog speed; then `0x0000` control | speed value, then right jog bit with servo state | `0110402800020400002710DBEE` -> `0110000000020400000103B23E` | Yes | Test uses MFC default speed `100` -> `00002710`. Servo-off variant is also covered by tests. |
| JogStop | `Alpha5_ExampleDlg.cpp:1516`, `Alpha5_ExampleDlg.cpp:1521` | `0x0000`, write `0x0002` registers | stop bit with current servo state | `011000000002040000002133B7` / `0110000000020400000020F277` | Yes | MFC mouse-up jog stop sends the stop frame only, not the cancel frame used by the Stop button. |

## Differences Found And Resolved

- The initial C# protocol used FC05 single-coil writes for servo/control commands. MFC uses FC10 multiple-register writes to address `0x0000` with a 32-bit control payload.
- The initial C# current-position read used address `0x0000`. MFC reads address `0x1006`.
- The initial C# move/jog addresses were placeholders. MFC uses `0x5100` for position-move parameters and `0x4028` for jog speed.
- MFC commands that depend on `m_bServoOn` encode the current servo state in bit 0. C# now exposes `servoOn` parameters and sequence methods so tests can pin both the servo-on and servo-off forms where relevant.
- CRC byte order matched after switching to the MFC frame structure: Modbus CRC16 low byte first.
