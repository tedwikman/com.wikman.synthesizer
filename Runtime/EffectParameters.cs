using UnityEngine;

namespace Wikman.Synthesizer
{
    public class EffectParameters
    {
        /// <summary>
        /// Shape of the wave.
        /// </summary>
        public int waveType
        {
            get => m_WaveType;
            set => m_WaveType = Mathf.Clamp(value, 0, 12);
        }
        int m_WaveType = int.MinValue;
        
        /// <summary>
        /// Overall volume of the current sound.
        /// </summary>
        public float masterVolume
        {
            get => m_MasterVolume;
            set => m_MasterVolume = Mathf.Clamp(value, 0f, 1f);
        }
        float m_MasterVolume = float.MinValue;            
        
        /// <summary>
        /// Length of the volume envelope attack.
        /// </summary>
        public float attackTime
        {
            get => m_AttackTime;
            set => m_AttackTime = Mathf.Clamp(value, 0f, 1f);
        }
        float m_AttackTime = float.MinValue;

        /// <summary>
        /// Length of the volume envelope sustain.
        /// </summary>
        public float sustainTime
        {
            get => m_SustainTime;
            set => m_SustainTime = Mathf.Clamp(value, 0f, 1f);
        }
        float m_SustainTime = float.MinValue;

        /// <summary>
        /// Tilts the sustain envelope for more 'pop'.
        /// </summary>
        public float sustainPunch
        {
            get => m_SustainPunch;
            set => m_SustainPunch = Mathf.Clamp(value, 0f, 1f);
        }
        float m_SustainPunch = float.MinValue;

        /// <summary>
        /// Length of the volume envelope decay.
        /// </summary>
        public float decayTime
        {
            get => m_DecayTime;
            set => m_DecayTime = Mathf.Clamp(value, 0f, 1f);
        }
        float m_DecayTime = float.MinValue;
        
        /// <summary>
        /// Pushes amplitudes together into a narrower range to make them stand out more.
        /// </summary>
        public float compressionAmount
        {
            get => m_CompressionAmount;
            set => m_CompressionAmount = Mathf.Clamp(value, 0f, 1f);
        }
        float m_CompressionAmount = float.MinValue;     

        /// <summary>
        /// Base note of the sound.
        /// </summary>
        public float startFrequency
        {
            get => m_StartFrequency;
            set => m_StartFrequency = Mathf.Clamp(value, 0f, 1f);
        }
        float m_StartFrequency = float.MinValue; 
        
        /// <summary>
        /// If sliding, the sound will stop at this frequency, to prevent really low notes.
        /// </summary>
        public float minFrequency
        {
            get => m_MinFrequency;
            set => m_MinFrequency = Mathf.Clamp(value, 0f, 1f);
        }
        float m_MinFrequency = float.MinValue;     
        
        /// <summary>
        /// Slides the frequency up or down.
        /// </summary>
        public float slide
        {
            get => m_Slide;
            set => m_Slide = Mathf.Clamp(value, -1f, 1f);
        }
        float m_Slide = float.MinValue;
        
        /// <summary>
        /// Accelerates the frequency slide.  Can be used to get the frequency to change direction.
        /// </summary>
        public float deltaSlide
        {
            get => m_DeltaSlide;
            set => m_DeltaSlide = Mathf.Clamp(value, -1f, 1f);
        }
        float m_DeltaSlide = float.MinValue;
        
        /// <summary>
        /// Strength of the vibrato effect.
        /// </summary>
        public float vibratoDepth
        {
            get => m_VibratoDepth;
            set => m_VibratoDepth = Mathf.Clamp(value, 0f, 1f);
        }
        float m_VibratoDepth = float.MinValue;
        
        /// <summary>
        /// Speed of the vibrato effect (i.e. frequency).
        /// </summary>
        public float vibratoSpeed
        {
            get => m_VibratoSpeed;
            set => m_VibratoSpeed = Mathf.Clamp(value, 0f, 1f);
        }
        float m_VibratoSpeed = float.MinValue;   
        
        /// <summary>
        /// Overlays copies of the waveform with copies and multiples of its frequency.
        /// </summary>
        public float overtones
        {
            get => m_Overtones;
            set => m_Overtones = Mathf.Clamp(value, 0f, 1f);
        }
        float m_Overtones = float.MinValue;
        
        /// <summary>
        /// The rate at which higher overtones should decay.
        /// </summary>
        public float overtoneFalloff
        {
            get => m_OvertoneFalloff;
            set => m_OvertoneFalloff = Mathf.Clamp(value, 0f, 1f);
        }
        float m_OvertoneFalloff = float.MinValue;
        
        /// <summary>
        /// Larger Values means more pitch jumps, which can be useful for arpeggiation.
        /// </summary>
        public float changeRepeat
        {
            get => m_ChangeRepeat;
            set => m_ChangeRepeat = Mathf.Clamp(value, 0f, 1f);
        }
        float m_ChangeRepeat = float.MinValue;
        
        /// <summary>
        /// Jump in pitch, either up or down.
        /// </summary>
        public float changeAmount
        {
            get => m_ChangeAmount;
            set => m_ChangeAmount = Mathf.Clamp(value, -1f, 1f);
        }
        float m_ChangeAmount = float.MinValue;
        
        /// <summary>
        /// How quickly the note shift happens.
        /// </summary>
        public float changeSpeed
        {
            get => m_ChangeSpeed;
            set => m_ChangeSpeed = Mathf.Clamp(value, 0f, 1f);
        }
        float m_ChangeSpeed = float.MinValue;
        
        /// <summary>
        /// Jump in pitch, either up or down.
        /// </summary>
        public float changeAmount2
        {
            get => m_ChangeAmount2;
            set => m_ChangeAmount2 = Mathf.Clamp(value, -1f, 1f);
        }
        float m_ChangeAmount2 = float.MinValue;  
        
        /// <summary>
        /// How quickly the note shift happens.
        /// </summary>
        public float changeSpeed2
        {
            get => m_ChangeSpeed2;
            set => m_ChangeSpeed2 = Mathf.Clamp(value, 0f, 1f);
        }
        float m_ChangeSpeed2 = float.MinValue;
        
        /// <summary>
        /// Square waveform only: Controls the ratio between the up and down states of the square wave, changing the tibre.
        /// </summary>
        public float squareDuty
        {
            get => m_SquareDuty;
            set => m_SquareDuty = Mathf.Clamp(value, 0f, 1f);
        }
        float m_SquareDuty = float.MinValue;
        
        /// <summary>
        /// Square waveform only: Sweeps the duty up or down.
        /// </summary>
        public float dutySweep
        {
            get => m_DutySweep;
            set => m_DutySweep = Mathf.Clamp(value, -1f, 1f);
        }
        float m_DutySweep = float.MinValue;
        
        /// <summary>
        /// Speed of the note repeating - certain variables are reset each time.
        /// </summary>
        public float repeatSpeed
        {
            get => m_RepeatSpeed;
            set => m_RepeatSpeed = Mathf.Clamp(value, 0f, 1f);
        }
        float m_RepeatSpeed = float.MinValue;
        
        /// <summary>
        /// Offsets a second copy of the wave by a small phase, changing the tibre.
        /// </summary>
        public float flangerOffset
        {
            get => m_FlangerOffset;
            set => m_FlangerOffset = Mathf.Clamp(value, -1f, 1f);
        }
        float m_FlangerOffset = float.MinValue;
        
        /// <summary>
        /// Sweeps the phase up or down.
        /// </summary>
        public float flangerSweep
        {
            get => m_FlangerSweep;
            set => m_FlangerSweep = Mathf.Clamp(value, -1f, 1f);
        }
        float m_FlangerSweep = float.MinValue;
        
        /// <summary>
        /// Frequency at which the low-pass filter starts attenuating higher frequencies.
        /// </summary>
        public float lpFilterCutoff
        {
            get => m_LpFilterCutoff;
            set => m_LpFilterCutoff = Mathf.Clamp(value, 0f, 1f);
        }
        float m_LpFilterCutoff = float.MinValue;
        
        /// <summary>
        /// Sweeps the low-pass cutoff up or down.
        /// </summary>
        public float lpFilterCutoffSweep
        {
            get => m_LpFilterCutoffSweep;
            set => m_LpFilterCutoffSweep = Mathf.Clamp(value, -1f, 1f);
        }
        float m_LpFilterCutoffSweep = float.MinValue;
        
        /// <summary>
        /// Changes the attenuation rate for the low-pass filter, changing the timbre.
        /// </summary>
        public float lpFilterResonance
        {
            get => m_LpFilterResonance;
            set => m_LpFilterResonance = Mathf.Clamp(value, 0f, 1f);
        }
        float m_LpFilterResonance = float.MinValue;
        
        /// <summary>
        /// Frequency at which the high-pass filter starts attenuating lower frequencies.
        /// </summary>
        public float hpFilterCutoff
        {
            get => m_HpFilterCutoff;
            set => m_HpFilterCutoff = Mathf.Clamp(value, 0f, 1f);
        }
        float m_HpFilterCutoff = float.MinValue;
        
        /// <summary>
        /// Sweeps the high-pass cutoff up or down.
        /// </summary>
        public float hpFilterCutoffSweep
        {
            get => m_HpFilterCutoffSweep;
            set => m_HpFilterCutoffSweep = Mathf.Clamp(value, -1f, 1f);
        }
        float m_HpFilterCutoffSweep = float.MinValue;
        
        /// <summary>
        /// Resamples the audio at a lower frequency.
        /// </summary>
        public float bitCrush
        {
            get => m_BitCrush;
            set => m_BitCrush = Mathf.Clamp(value, 0f, 1f);
        }
        float m_BitCrush = float.MinValue;
        
        /// <summary>
        /// Sweeps the Bit Crush filter up or down.
        /// </summary>
        public float bitCrushSweep
        {
            get => m_BitCrushSweep;
            set => m_BitCrushSweep = Mathf.Clamp(value, -1f, 1f);
        }
        float m_BitCrushSweep = float.MinValue;

        public override string ToString()
        {
            var output = "";
            output += $"WaveType = {waveType} \n";
            output += $"MasterVolume = {masterVolume}f \n";
            output += $"AttackTime = {attackTime}f \n";
            output += $"SustainTime = {sustainTime}f \n";
            output += $"SustainPunch = {sustainPunch}f \n";
            output += $"DecayTime = {decayTime}f \n";
            output += $"CompressionAmount = {compressionAmount}f \n";
            output += $"StartFrequency = {startFrequency}f \n";
            output += $"MinFrequency = {minFrequency}f \n";
            output += $"Slide = {slide}f \n";
            output += $"DeltaSlide = {deltaSlide}f \n";
            output += $"VibratoDepth = {vibratoDepth}f \n";
            output += $"VibratoSpeed = {vibratoSpeed}f \n";
            output += $"Overtones = {overtones}f \n";
            output += $"OvertoneFalloff = {overtoneFalloff}f \n";
            output += $"ChangeRepeat = {changeRepeat}f \n";
            output += $"ChangeAmount = {changeAmount}f \n";
            output += $"ChangeSpeed = {changeSpeed}f \n";
            output += $"ChangeAmount2 = {changeAmount2}f \n";
            output += $"ChangeSpeed2 = {changeSpeed2}f \n";
            output += $"SquareDuty = {squareDuty}f \n";
            output += $"DutySweep = {dutySweep}f \n";
            output += $"RepeatSpeed = {repeatSpeed}f \n";
            output += $"FlangerOffset = {flangerOffset}f \n";
            output += $"FlangerSweep = {flangerSweep}f \n";
            output += $"LpFilterCutoff = {lpFilterCutoff}f \n";
            output += $"LpFilterCutoffSweep = {lpFilterCutoffSweep}f \n";
            output += $"LpFilterResonance = {lpFilterResonance}f \n";
            output += $"HpFilterCutoff = {hpFilterCutoff}f \n";
            output += $"HpFilterCutoffSweep = {hpFilterCutoffSweep}f \n";
            output += $"BitCrush = {bitCrush}f \n";
            output += $"BitCrushSweep = {bitCrushSweep}f \n";
            return output;
        }
    }
}
