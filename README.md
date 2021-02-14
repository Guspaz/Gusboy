![Latest Build](https://github.com/Guspaz/Gusboy/workflows/Latest%20Build/badge.svg)
# Gusboy
C# Gameboy Color emulator

Gusboy is a Gameboy Color emulator written in C# and targeting .NET 5.0. It plays most games without obvious issues, including those with things like hicolour effects. It's still a work-in-progress, and the design is something of a mess, as I had no idea what I was doing when I started.

The primary front-end relies on SDL2 (and should theoretically be cross-platform), though a WinForms UI is available (but disabled in the build by default). The front-end interfaces with the emulator via a fairly thin API whereby audio and video are exposed via float and int32 buffers, input events are sent in via function calls, and a callback is used to let the front-end know when the emulator has hit vblank. It's left to the front-end to time the emulator execution: the SDL2 front-end uses a 32,768 audio sample rate to run the emulator at the right speed.

#### Features

- Full sound and video emulation
- DMG and CGB (colour) support
- SDL2 front-end
- Sub-scanline renderer (background tiles only)
- SRAM/RTC save support
- Plays most games without obvious issues
- GBS support (press left/right to change tracks)
- Supports MBC1/MBC3+RTC/MBC5/GBS mappers
- CGB-on-CGB and DMG-on-DMG support

#### TODOs/Wishlist

In random order:

- Make emulator configurable via config file and commandline parameters
  - Currently most things are compile-time options
- Add support for 44-byte RTC save format
- Implement more mappers
- Improve GPU caching mechanism to be more granular
  - Currently expires entire tile cache on any write to vram
- Improve sub-scanline renderer to support both the Pocket demo and GBVideoPlayer2 simultaneously
  - First-pixel offset of 6 clocks works for pocket demo, 8 clocks works for GBVideoPlayer2, timing must be off somewhere
- Implement real HDMA
  - Currently faked as a GDMA, which seems to cause few issues
- Implement DMG-on-CGB mode
- Simplify APU if possible
  - The NR register implementation is quite verbose
- Rewrite the CPU execution unit to not have a dedicated function for each and every opcode
  - Maybe a first attempt would use reference parameters for registers
- In general, pass more tests
  - Setting up a branch of the gameboy emulator shootout repo would help with this
- Other general improvements, there's always more that can be done
