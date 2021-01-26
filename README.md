# Gusboy
C# Gameboy Color emulator

I created this emulator back in 2017, got it so a bunch of games I tried were playable, but stopped before implementing audio, because it seemed too hard. Now, years later, I've picked it up again, implemented audio (passes 8/12 dmg_sound) and Gameboy Color support, improved performance, dramatically improved accuracy, cleaned up the code a bunch, etc. It's hardly "production quality" levels of accuracy or polish (very very far from it), but most games run without obvious major glitches.

It's still a work-in-progress, and I had no idea what I was doing at the start, so the design is a huge mess, warped by troubleshooting things until they work. I've been cleaning up and refactoring, but it's still not great. The CPU in particular, I cringe at how I implemented it as hundreds of standalone functions, so much code instead of even just relying on reference parameters. The thing is, the CPU execution unit, as hilariously bloated as it is, seems to work perfectly (passes cpu_instrs and instr_timing) and isn't a performance bottleneck, so it's not worth rewriting at the moment.

GBS file support is in too, it will detect GBS files and use a custom mapper to play them (left/right controls which song plays). There is also a separate music player UI that can be used for GBS files.

The UI depends on NAudio and WinForms, but the emulator itself should be platform-independent (a cross-platform OpenTK ui is in the works) It interfaces with the UI through a fairly thin API. Video is handled via an int32 framebuffer and a callback function to notify the application that the Gameboy has finished rendering a frame (it hit its vblank). Audio is handled by a float buffer that the application can consume as desired. Input is handled by KeyUp and KeyDown methods that the application can call. It's left up to the application to wire itself up to that and call the emulator's tick function in a loop.

Speed control is left up to the application. You can either loop the emulator until it notifies you that a frame is ready, and then halt emulation until the next vsync, synchronizing to video, or you can drain the audio buffer at the same speed as playback, synchronizing to audio. The included UI does the latter, every time NAudio wants to read more audio data, the emulator is run until its internal audio buffer has enough data to satisfy NAudio's read request. You get smooth audio at the expense of frame pacing (which needs work even if we have audio sync).

#### TODOs/Wishlist

In no particular order

- Implement cross-platform UI in OpenTK (in progress)
- Improve GPU caching mechanism to be more granular
- Implement real HDMA (currently faked as GDMA)
- Simplify APU if possible
- Rewrite the CPU execution unit to not have a dedicated function for each and every opcode
  - Maybe a first attempt would use reference parameters for registers
- Try to move more of the GPU code into the cache functions
- In general, pass more tests
- Other general improvements, there's always more that can be done
