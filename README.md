# Synthesizer

An audio synthesizer built on [Sfxr](https://www.drpetter.se/project_sfxr.html) / [Bfxr](https://www.bfxr.net/), packaged for Unity.

### In the package
The package comes with an editor window for edit time authoring, and runtime API for runtime generation of AudioClips.

#### Editor

![Editor Window](./img~/editor.png "Editor Window")

The editor window can be found at __Window > Audio > Synthesizer__.

#### API
Generate a random AudioClip:
```c#
ClipData clipData =  Wikman.Synthesizer.Synth.GenerateRandom();
AudioClip clip = clipData.clip;
```

Generate a random laser AudioClip:
```c#
ClipData clipData =  Wikman.Synthesizer.Synth.GenerateRandom(EffectType.Laser);
AudioClip clip = clipData.clip;
```

Generate a jump AudioClip using a seed:
```c#
int seed = 12345;
ClipData clipData =  Wikman.Synthesizer.Synth.GenerateRandom(EffectType.Jump, seed);
AudioClip clip = clipData.clip;
```

Generate a custom AudioClip:
```c#
var parameters = new Wikman.Synthesizer.EffectParameters()
{
    waveType = 8,
    sustainTime = 0.3f,
    startFrequency = 0.7f,
    slide = -0.2f
};
ClipData clipData = Wikman.Synthesizer.Synth.Generate(parameters);
AudioClip clip = clipData.clip;
```

### How to get the package

Prerequisite: Unity 2021.3 or newer

Step 1: Open the Package Manager, press the + in the top left corner and select __Add package from git URL...__

![Step 1](./img~/install_00.png "Step 1")

Step 2: Add the following url to the field and press __Add__
```
git@github.com:tedwikman/com.wikman.synthesizer.git
```
![Step 2](./img~/install_01.png "Step 2")
