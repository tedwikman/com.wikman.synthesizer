using System;
using System.Collections.Generic;
using UnityEngine;
using Wikman.Synthesizer.Sfxr;

using Random = UnityEngine.Random;

namespace Wikman.Synthesizer
{
    public enum EffectType
    {
        None,
        Pickup,
        Laser,
        Explosion,
        PowerUp,
        Hit,
        Jump,
        Blip
    }

    public static class Synth
    {
        const int k_NoOfChannels = 1;
        static readonly SfxrInterface k_SfxrInterface = new SfxrInterface();

        public static uint sampleRate
        {
            get => k_SfxrInterface.sampleRate;
            set => k_SfxrInterface.sampleRate = value;
        }

        public static ClipData Generate(EffectParameters parameterData)
        {
            var inputData = ToBfxrFormat(parameterData);

            k_SfxrInterface.CreateWithData(inputData, out var buffer, out var noOfSamples, out var outputData);
            return CreateClipData(buffer, noOfSamples, EffectType.None, 0, outputData);
        }

        public static ClipData GenerateRandom(EffectType effectType = EffectType.None, int seed = 0, EffectParameters inputEffectParameters = null)
        {
            float[] buffer;
            int noOfSamples;
            Dictionary<ParameterType, float> inputData = null;
            Dictionary<ParameterType, float> outputData;

            if (seed == 0)
                seed = Mathf.RoundToInt(Random.value * int.MaxValue);
            Random.InitState(seed);

            if (inputEffectParameters != null)
                inputData = ToBfxrFormat(inputEffectParameters);
            
            switch (effectType)
            {
                case EffectType.Pickup:
                    k_SfxrInterface.GeneratePickup(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                case EffectType.Laser:
                    k_SfxrInterface.GenerateLaser(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                case EffectType.Explosion:
                    k_SfxrInterface.GenerateExplosion(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                case EffectType.PowerUp:
                    k_SfxrInterface.GeneratePowerUp(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                case EffectType.Hit:
                    k_SfxrInterface.GenerateHit(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                case EffectType.Jump:
                    k_SfxrInterface.GenerateJump(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                case EffectType.Blip:
                    k_SfxrInterface.GenerateBlip(inputData, out buffer, out noOfSamples, out outputData);
                    break;
                default:
                    k_SfxrInterface.GenerateRandom(inputData, out buffer, out noOfSamples, out outputData);
                    break;
            }

            return CreateClipData(buffer, noOfSamples, effectType, seed, outputData);
        }

        static ClipData CreateClipData(float[] audioBuffer, int noOfSamples, EffectType effectType, int randomSeed, Dictionary<ParameterType, float> parameterData)
        {
            var clip = ToAudioClip(audioBuffer, noOfSamples);
            var outputAdvancedData = FromBfxrFormat(parameterData);
            
            var clipData = new ClipData(clip, randomSeed, effectType, outputAdvancedData);
            return clipData;
        }

        static AudioClip ToAudioClip(float[] buffer, int noOfSamples)
        {
            var clip = AudioClip.Create("RandomSound", noOfSamples, k_NoOfChannels, (int)sampleRate, false);
            clip.SetData(buffer, 0);
            return clip;
        }
        
        static Dictionary<ParameterType, float> ToBfxrFormat(EffectParameters effectParameters)
        {
            var paramData = new Dictionary<ParameterType, float>();
            if(effectParameters.waveType != int.MinValue)
                paramData.Add(ParameterType.WaveType, effectParameters.waveType);
            if(Math.Abs(effectParameters.masterVolume - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.MasterVolume, effectParameters.masterVolume);
            if(Math.Abs(effectParameters.attackTime - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.AttackTime, effectParameters.attackTime);
            if(Math.Abs(effectParameters.sustainTime - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.SustainTime, effectParameters.sustainTime);
            if(Math.Abs(effectParameters.sustainPunch - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.SustainPunch, effectParameters.sustainPunch);
            if(Math.Abs(effectParameters.decayTime - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.DecayTime, effectParameters.decayTime);
            if(Math.Abs(effectParameters.compressionAmount - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.CompressionAmount, effectParameters.compressionAmount);
            if(Math.Abs(effectParameters.startFrequency - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.StartFrequency, effectParameters.startFrequency);
            if(Math.Abs(effectParameters.minFrequency - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.MinFrequency, effectParameters.minFrequency);
            if(Math.Abs(effectParameters.slide - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.Slide, effectParameters.slide);
            if(Math.Abs(effectParameters.deltaSlide - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.DeltaSlide, effectParameters.deltaSlide);
            if(Math.Abs(effectParameters.vibratoDepth - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.VibratoDepth, effectParameters.vibratoDepth);
            if(Math.Abs(effectParameters.vibratoSpeed - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.VibratoSpeed, effectParameters.vibratoSpeed);
            if(Math.Abs(effectParameters.overtones - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.Overtones, effectParameters.overtones);
            if(Math.Abs(effectParameters.overtoneFalloff - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.OvertoneFalloff, effectParameters.overtoneFalloff);
            if(Math.Abs(effectParameters.changeRepeat - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.ChangeRepeat, effectParameters.changeRepeat);
            if(Math.Abs(effectParameters.changeAmount - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.ChangeAmount, effectParameters.changeAmount);
            if(Math.Abs(effectParameters.changeSpeed - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.ChangeSpeed, effectParameters.changeSpeed);
            if(Math.Abs(effectParameters.changeAmount2 - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.ChangeAmount2, effectParameters.changeAmount2);
            if(Math.Abs(effectParameters.changeSpeed2 - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.ChangeSpeed2, effectParameters.changeSpeed2);
            if(Math.Abs(effectParameters.squareDuty - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.SquareDuty, effectParameters.squareDuty);
            if(Math.Abs(effectParameters.dutySweep - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.DutySweep, effectParameters.dutySweep);
            if(Math.Abs(effectParameters.repeatSpeed - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.RepeatSpeed, effectParameters.repeatSpeed);
            if(Math.Abs(effectParameters.flangerOffset - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.FlangerOffset, effectParameters.flangerOffset);
            if(Math.Abs(effectParameters.flangerSweep - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.FlangerSweep, effectParameters.flangerSweep);
            if(Math.Abs(effectParameters.lpFilterCutoff - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.LpFilterCutoff, effectParameters.lpFilterCutoff);
            if(Math.Abs(effectParameters.lpFilterCutoffSweep - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.LpFilterCutoffSweep, effectParameters.lpFilterCutoffSweep);
            if(Math.Abs(effectParameters.lpFilterResonance - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.LpFilterResonance, effectParameters.lpFilterResonance);
            if(Math.Abs(effectParameters.hpFilterCutoff - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.HpFilterCutoff, effectParameters.hpFilterCutoff);
            if(Math.Abs(effectParameters.hpFilterCutoffSweep - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.HpFilterCutoffSweep, effectParameters.hpFilterCutoffSweep);
            if(Math.Abs(effectParameters.bitCrush - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.BitCrush, effectParameters.bitCrush);
            if(Math.Abs(effectParameters.bitCrushSweep - float.MinValue) > Mathf.Epsilon)
                paramData.Add(ParameterType.BitCrushSweep, effectParameters.bitCrushSweep);

            return paramData;
        }        

        static EffectParameters FromBfxrFormat(Dictionary<ParameterType, float> parameterData)
        {
            var data = new EffectParameters()
            {
                waveType = (int)parameterData[ParameterType.WaveType],
                masterVolume = parameterData[ParameterType.MasterVolume],
                attackTime = parameterData[ParameterType.AttackTime],
                sustainTime = parameterData[ParameterType.SustainTime],
                sustainPunch = parameterData[ParameterType.SustainPunch],
                decayTime = parameterData[ParameterType.DecayTime],
                compressionAmount = parameterData[ParameterType.CompressionAmount], 
                startFrequency = parameterData[ParameterType.StartFrequency],
                minFrequency = parameterData[ParameterType.MinFrequency],
                slide = parameterData[ParameterType.Slide],
                deltaSlide = parameterData[ParameterType.DeltaSlide],
                vibratoDepth = parameterData[ParameterType.VibratoDepth],
                vibratoSpeed = parameterData[ParameterType.VibratoSpeed],
                overtones = parameterData[ParameterType.Overtones],
                overtoneFalloff = parameterData[ParameterType.OvertoneFalloff],
                changeRepeat = parameterData[ParameterType.ChangeRepeat],
                changeAmount = parameterData[ParameterType.ChangeAmount],
                changeSpeed = parameterData[ParameterType.ChangeSpeed],
                changeAmount2 = parameterData[ParameterType.ChangeAmount2],
                changeSpeed2 = parameterData[ParameterType.ChangeSpeed2],
                squareDuty = parameterData[ParameterType.SquareDuty],
                dutySweep = parameterData[ParameterType.DutySweep],
                repeatSpeed = parameterData[ParameterType.RepeatSpeed],
                flangerOffset = parameterData[ParameterType.FlangerOffset],
                flangerSweep = parameterData[ParameterType.FlangerSweep],
                lpFilterCutoff = parameterData[ParameterType.LpFilterCutoff],
                lpFilterCutoffSweep = parameterData[ParameterType.LpFilterCutoffSweep],
                lpFilterResonance = parameterData[ParameterType.LpFilterResonance],
                hpFilterCutoff = parameterData[ParameterType.HpFilterCutoff],
                hpFilterCutoffSweep = parameterData[ParameterType.HpFilterCutoffSweep],
                bitCrush = parameterData[ParameterType.BitCrush],
                bitCrushSweep = parameterData[ParameterType.BitCrushSweep]
            };
            return data;
        }
    }
}
