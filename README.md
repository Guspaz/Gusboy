# Gusboy
C# Gameboy emulator

I created this emulator back in 2017, got it so most games I tried were playable, but stopped before implementing audio, because it seemed too hard. Now, years later, I've picked it up again, fully implemented audio, improved performance and accuracy, cleaned up the code a bunch, etc. It's hardly "production quality" levels of accuracy, but lots of games run without obvious major glitches.

It's still a work-in-progress, and I had no idea what I was doing at the start, so the design is a huge mess. I've started cleaning up and refactoring, but it's still not great. The CPU in particular, I cringe at how I implemented it as hundreds of standalone functions, so much code instead of even just relying on reference parameters. The thing is, the CPU execution unit, as hilariously bloated as it is, seems to work perfectly (passes cpu_instr and instr_timing) and isn't a performance bottleneck, so it's not worth rewriting at the moment.

I'll fill in more details here later.
