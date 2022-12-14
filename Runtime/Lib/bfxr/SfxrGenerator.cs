/***
* SfxrSynth
* 
* Copyright 2010 Thomas Vian
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* 	http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
* 
* @author Thomas Vian
***/

using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Wikman.Synthesizer.Sfxr
{
	internal class SfxrGenerator
    {
	    const float k_MinLength = 0.18f;
		const int k_LoResNoisePeriod = 8; 		// should be <32

		ParameterDatabase m_Parameters;			// Params instance

		bool m_Finished;						// If the sound has finished
		float m_MasterVolume;					// masterVolume * masterVolume (for quick calculations)
		uint m_WaveType;						// The type of wave to generate
		
		float m_EnvelopeVolume;					// Current volume of the envelope
		int m_EnvelopeStage;					// Current stage of the envelope (attack, sustain, decay, end)
		float m_EnvelopeTime;					// Current time through current enelope stage
		float m_EnvelopeLength;					// Length of the current envelope stage
		float m_EnvelopeLength0;				// Length of the attack stage
		float m_EnvelopeLength1;				// Length of the sustain stage
		float m_EnvelopeLength2;				// Length of the decay stage
		float m_EnvelopeOverLength0;			// 1 / _envelopeLength0 (for quick calculations)
		float m_EnvelopeOverLength1;			// 1 / _envelopeLength1 (for quick calculations)
		float m_EnvelopeOverLength2;			// 1 / _envelopeLength2 (for quick calculations)
		int m_EnvelopeFullLength;				// Full length of the volume envelop (and therefore sound)
		
		float m_SustainPunch;					// The punch factor (louder at beginning of sustain)
		
		int m_Phase;							// Phase through the wave
		float m_Pos;							// Phase expressed as a Number from 0-1, used for fast sin approx
		float m_Period;							// Period of the wave
		float m_PeriodTemp;						// Period modified by vibrato
		float m_MaxPeriod;						// Maximum period before sound stops (from minFrequency)
		
		float m_Slide;							// Note slide
		float m_DeltaSlide;						// Change in slide
		float m_MinFrequency;					// Minimum frequency before stopping
		bool m_Muted;							// Whether or not min frequency has been attained
		
		
		int m_Overtones;						// Minimum frequency before stopping
		float m_OvertoneFalloff;				// Minimum frequency before stopping
		
		float m_VibratoPhase;					// Phase through the vibrato sine wave
		float m_VibratoSpeed;					// Speed at which the vibrato phase moves
		float m_VibratoAmplitude;				// Amount to change the period of the wave by at the peak of the vibrato wave
		
		float m_ChangePeriod;
		int m_ChangePeriodTime;
		
		float m_ChangeAmount;					// Amount to change the note by
		int m_ChangeTime;						// Counter for the note change
		int m_ChangeLimit;						// Once the time reaches this limit, the note changes
		bool m_ChangeReached;
		
		float m_ChangeAmount2;					// Amount to change the note by
		int m_ChangeTime2;						// Counter for the note change
		int m_ChangeLimit2;						// Once the time reaches this limit, the note changes
		bool m_ChangeReached2;
		
		
		float m_SquareDuty;						// Offset of center switching point in the square wave
		float m_DutySweep;						// Amount to change the duty by
		
		int m_RepeatTime;						// Counter for the repeats
		int m_RepeatLimit;						// Once the time reaches this limit, some of the variables are reset
		
		bool m_Flanger;							// If the flanger is active
		float m_FlangerOffset;					// Phase offset for flanger effect
		float m_FlangerDeltaOffset;				// Change in phase offset
		int m_FlangerInt;						// Integer flanger offset, for bit maths
		int m_FlangerPos;						// Position through the flanger buffer
		List<float> m_FlangerBuffer;			// Buffer of wave values used to create the out of phase second wave
		
		bool m_Filters;							// If the filters are active
		float m_LpFilterPos;					// Adjusted wave position after low-pass filter
		float m_LpFilterOldPos;					// Previous low-pass wave position
		float m_LpFilterDeltaPos;				// Change in low-pass wave position, as allowed by the cutoff and damping
		float m_LpFilterCutoff;					// Cutoff multiplier which adjusts the amount the wave position can move
		float m_LpFilterDeltaCutoff;			// Speed of the low-pass cutoff multiplier
		float m_LpFilterDamping;				// Damping muliplier which restricts how fast the wave position can move
		bool m_LpFilterOn;						// If the low pass filter is active
		
		float m_HpFilterPos;					// Adjusted wave position after high-pass filter
		float m_HpFilterCutoff;					// Cutoff multiplier which adjusts the amount the wave position can move
		float m_HpFilterDeltaCutoff;			// Speed of the high-pass cutoff multiplier

		List<float> m_NoiseBuffer;				// Buffer of random values used to generate noise
		List<float> m_PinkNoiseBuffer;			// Buffer of random values used to generate noise
		List<float> m_LoResNoiseBuffer;			// Buffer of random values used to generate noise
		
		int m_OneBitNoiseState;					// Buffer containing one-bit periodic noise state.
		float m_OneBitNoise;					// Current sample of one-bit noise.
		
		int m_BuzzState;							// Buffer containing 'buzz' periodic noise state.
		float m_Buzz;							// Current sample of 'buzz' noise.
		
		PinkNumber m_PinkNumber = new PinkNumber();
		
		float m_SuperSample;					// Actual sample written to the wave
		float m_Sample;							// Sub-sample calculated 8 times per actual sample, averaged out to get the super sample

		float m_BitcrushFreq;					// inversely proportional to the number of samples to skip 
		float m_BitcrushFreqSweep;				// change of the above
		float m_BitcrushPhase;					// samples when this > 1
		float m_BitcrushLast;					// last sample value
		
		float m_CompressionFactor;
		
		public SfxrGenerator()
		{
			parameters = new ParameterDatabase();
		}		
		
		public ParameterDatabase parameters
		{
			get => m_Parameters;
			private set => m_Parameters = value;
		}

		public int GetNoOfSamples() => m_EnvelopeFullLength;

		void ClampTotalLength()
		{
			var p = m_Parameters;
			var totalTime = p.GetParam(ParameterType.AttackTime) + p.GetParam(ParameterType.SustainTime) + p.GetParam(ParameterType.DecayTime);
			if (totalTime < k_MinLength ) 
			{
				var multiplier = k_MinLength / totalTime;
				p.SetParam(ParameterType.AttackTime,p.GetParam(ParameterType.AttackTime) * multiplier);
				p.SetParam(ParameterType.SustainTime,p.GetParam(ParameterType.SustainTime) * multiplier);
				p.SetParam(ParameterType.DecayTime,p.GetParam(ParameterType.DecayTime) * multiplier);
			}
		}
		
		/**
		 * Resets the running variables from the params
		 * Used once at the start (total reset) and for the repeat effect (partial reset)
		 * @param	totalReset	If the reset is total
		 */
		public void Reset(bool totalReset)
		{
			// Shorter reference
			var p = m_Parameters;
			
			m_Period = 100.0f / (p.GetParam(ParameterType.StartFrequency) * p.GetParam(ParameterType.StartFrequency) + 0.001f);
			m_MaxPeriod = 100.0f / (p.GetParam(ParameterType.MinFrequency) * p.GetParam(ParameterType.MinFrequency) + 0.001f);
						
			
			m_Slide = 1.0f - p.GetParam(ParameterType.Slide) * p.GetParam(ParameterType.Slide) * p.GetParam(ParameterType.Slide) * 0.01f;
			m_DeltaSlide = -p.GetParam(ParameterType.DeltaSlide) * p.GetParam(ParameterType.DeltaSlide) * p.GetParam(ParameterType.DeltaSlide) * 0.000001f;
			
			if ((int)(p.GetParam(ParameterType.WaveType)) == 0)
			{
				m_SquareDuty = 0.5f - p.GetParam(ParameterType.SquareDuty) * 0.5f;
				m_DutySweep = -p.GetParam(ParameterType.DutySweep) * 0.00005f;
			}
			
			m_ChangePeriod = Mathf.Max(((1 - p.GetParam(ParameterType.ChangeRepeat)) + 0.1f) / 1.1f) * 20000 + 32;
			m_ChangePeriodTime = 0;
			
			if (p.GetParam(ParameterType.ChangeAmount) > 0.0f) 	
				m_ChangeAmount = 1.0f - p.GetParam(ParameterType.ChangeAmount) * p.GetParam(ParameterType.ChangeAmount) * 0.9f;
			else 						
				m_ChangeAmount = 1.0f + p.GetParam(ParameterType.ChangeAmount) * p.GetParam(ParameterType.ChangeAmount) * 10.0f;
			
			m_ChangeTime = 0;
			m_ChangeReached = false;
			
			if (Math.Abs(p.GetParam(ParameterType.ChangeSpeed) - 1.0f) < Mathf.Epsilon) 	
				m_ChangeLimit = 0;
			else 						
				m_ChangeLimit = Mathf.RoundToInt((1.0f - p.GetParam(ParameterType.ChangeSpeed)) * (1.0f - p.GetParam(ParameterType.ChangeSpeed)) * 20000 + 32);
			
			if (p.GetParam(ParameterType.ChangeAmount2) > 0.0f) 	
				m_ChangeAmount2 = 1.0f - p.GetParam(ParameterType.ChangeAmount2) * p.GetParam(ParameterType.ChangeAmount2) * 0.9f;
			else 						
				m_ChangeAmount2 = 1.0f + p.GetParam(ParameterType.ChangeAmount2) * p.GetParam(ParameterType.ChangeAmount2) * 10.0f;
			
			
			m_ChangeTime2 = 0;			
			m_ChangeReached2 = false;
			
			if (Math.Abs(p.GetParam(ParameterType.ChangeSpeed2) - 1.0f) < Mathf.Epsilon) 	
				m_ChangeLimit2 = 0;
			else 						
				m_ChangeLimit2 = Mathf.RoundToInt((1.0f - p.GetParam(ParameterType.ChangeSpeed2)) * (1.0f - p.GetParam(ParameterType.ChangeSpeed2)) * 20000 + 32);
			
			m_ChangeLimit *= Mathf.RoundToInt(1 - p.GetParam(ParameterType.ChangeRepeat) + 0.1f / 1.1f);
			m_ChangeLimit2 *= Mathf.RoundToInt((1 - p.GetParam(ParameterType.ChangeRepeat) + 0.1f) / 1.1f);
			
			if(totalReset)
			{
				m_MasterVolume = p.GetParam(ParameterType.MasterVolume) * p.GetParam(ParameterType.MasterVolume);
				
				m_WaveType = (uint)p.GetParam(ParameterType.WaveType);
				
				if (p.GetParam(ParameterType.SustainTime) < 0.01f) 
					p.SetParam(ParameterType.SustainTime, 0.01f);
				
				ClampTotalLength();
				
				m_SustainPunch = p.GetParam(ParameterType.SustainPunch);
				
				m_Phase = 0;
				
				m_MinFrequency = p.GetParam(ParameterType.MinFrequency);
				m_Muted = false;
				m_Overtones = Mathf.RoundToInt(p.GetParam(ParameterType.Overtones) * 10f);
				m_OvertoneFalloff = p.GetParam(ParameterType.OvertoneFalloff);
								
				m_BitcrushFreq = 1 - Mathf.Pow(p.GetParam(ParameterType.BitCrush),1.0f / 3.0f);				
				m_BitcrushFreqSweep = -p.GetParam(ParameterType.BitCrushSweep) * 0.000015f;
				m_BitcrushPhase = 0;
				m_BitcrushLast = 0;				
				
				m_CompressionFactor = 1 / (1 + 4 * p.GetParam(ParameterType.CompressionAmount));
				
				m_Filters = Math.Abs(p.GetParam(ParameterType.LpFilterCutoff) - 1.0f) > Mathf.Epsilon || p.GetParam(ParameterType.HpFilterCutoff) != 0.0f;				
				
				m_LpFilterPos = 0.0f;
				m_LpFilterDeltaPos = 0.0f;
				m_LpFilterCutoff = p.GetParam(ParameterType.LpFilterCutoff) * p.GetParam(ParameterType.LpFilterCutoff) * p.GetParam(ParameterType.LpFilterCutoff) * 0.1f;
				m_LpFilterDeltaCutoff = 1.0f + p.GetParam(ParameterType.LpFilterCutoffSweep) * 0.0001f;
				m_LpFilterDamping = 5.0f / (1.0f + p.GetParam(ParameterType.LpFilterResonance) * p.GetParam(ParameterType.LpFilterResonance) * 20.0f) * (0.01f + m_LpFilterCutoff);
				if (m_LpFilterDamping > 0.8f) 
					m_LpFilterDamping = 0.8f;
				m_LpFilterDamping = 1.0f - m_LpFilterDamping;
				m_LpFilterOn = Math.Abs(p.GetParam(ParameterType.LpFilterCutoff) - 1.0f) > Mathf.Epsilon;
				
				m_HpFilterPos = 0.0f;
				m_HpFilterCutoff = p.GetParam(ParameterType.HpFilterCutoff) * p.GetParam(ParameterType.HpFilterCutoff) * 0.1f;
				m_HpFilterDeltaCutoff = 1.0f + p.GetParam(ParameterType.HpFilterCutoffSweep) * 0.0003f;
				
				m_VibratoPhase = 0.0f;
				m_VibratoSpeed = p.GetParam(ParameterType.VibratoSpeed) * p.GetParam(ParameterType.VibratoSpeed) * 0.01f;
				m_VibratoAmplitude = p.GetParam(ParameterType.VibratoDepth) * 0.5f;
				
				m_EnvelopeVolume = 0.0f;
				m_EnvelopeStage = 0;
				m_EnvelopeTime = 0;
				m_EnvelopeLength0 = p.GetParam(ParameterType.AttackTime) * p.GetParam(ParameterType.AttackTime) * 100000.0f;
				m_EnvelopeLength1 = p.GetParam(ParameterType.SustainTime) * p.GetParam(ParameterType.SustainTime) * 100000.0f;
				m_EnvelopeLength2 = p.GetParam(ParameterType.DecayTime) * p.GetParam(ParameterType.DecayTime) * 100000.0f + 10;
				m_EnvelopeLength = m_EnvelopeLength0;
				m_EnvelopeFullLength = (int)(m_EnvelopeLength0 + m_EnvelopeLength1 + m_EnvelopeLength2);
				
				m_EnvelopeOverLength0 = 1.0f / m_EnvelopeLength0;
				m_EnvelopeOverLength1 = 1.0f / m_EnvelopeLength1;
				m_EnvelopeOverLength2 = 1.0f / m_EnvelopeLength2;
				
				m_Flanger = p.GetParam(ParameterType.FlangerOffset) != 0.0f || p.GetParam(ParameterType.FlangerSweep) != 0.0f;
				
				m_FlangerOffset = p.GetParam(ParameterType.FlangerOffset) * p.GetParam(ParameterType.FlangerOffset) * 1020.0f;
				if(p.GetParam(ParameterType.FlangerOffset) < 0.0f) 
					m_FlangerOffset = -m_FlangerOffset;
				m_FlangerDeltaOffset = p.GetParam(ParameterType.FlangerSweep) * p.GetParam(ParameterType.FlangerSweep) * p.GetParam(ParameterType.FlangerSweep) * 0.2f;
				m_FlangerPos = 0;
				
				if(m_FlangerBuffer == null) 
					m_FlangerBuffer = new List<float>(new float[1024]);
				if(m_NoiseBuffer == null) 
					m_NoiseBuffer = new List<float>(new float[32]);
				if(m_PinkNoiseBuffer == null) 
					m_PinkNoiseBuffer = new List<float>(new float[32]);
				if(m_LoResNoiseBuffer == null) 
					m_LoResNoiseBuffer = new List<float>(new float[32]);
				if (m_PinkNumber == null) 
					m_PinkNumber = new PinkNumber();
				
				m_OneBitNoiseState = 1 << 14;
				m_OneBitNoise = 0;
				m_BuzzState = 1 << 14;
				m_Buzz = 0f;
				
				for(var i = 0; i < 1024; i++) 
					m_FlangerBuffer[i] = 0.0f;
				for(var i = 0; i < 32; i++) 
					m_NoiseBuffer[i] = Random.value * 2.0f - 1.0f;
				for(var i = 0; i < 32; i++) 
					m_PinkNoiseBuffer[i] = m_PinkNumber.GetNextValue();
				for(var i = 0; i < 32; i++) 
					m_LoResNoiseBuffer[i] = ((i%k_LoResNoisePeriod)==0) ? Random.value * 2.0f - 1.0f : m_LoResNoiseBuffer[i-1];							
			
				m_RepeatTime = 0;
				
				if (p.GetParam(ParameterType.RepeatSpeed) == 0.0) 	
					m_RepeatLimit = 0;
				else 						
					m_RepeatLimit = (int)((1.0f-p.GetParam(ParameterType.RepeatSpeed)) * (1.0f-p.GetParam(ParameterType.RepeatSpeed)) * 20000) + 32;
			}
			
			if (m_WaveType == 9 || m_WaveType == 11)
			{
				var sf = p.GetParam(ParameterType.StartFrequency);
				var mf = p.GetParam(ParameterType.MinFrequency);
				
				var startFrequencyMin = p.GetMin(ParameterType.StartFrequency);
				var startFrequencyMax = p.GetMax(ParameterType.StartFrequency);
				var startFrequencyMid = (startFrequencyMax+startFrequencyMin) / 2f;
				
				var minFrequencyMin = p.GetMin(ParameterType.MinFrequency);
				var minFrequencyMax = p.GetMax(ParameterType.MinFrequency);
				var minFrequencyMid = (minFrequencyMax+minFrequencyMin) / 2f;

				var deltaStart = (sf - startFrequencyMin) / (startFrequencyMax - startFrequencyMin);
				var deltaMin = (mf - minFrequencyMin) / (minFrequencyMax - minFrequencyMin);
				
				sf = startFrequencyMid + deltaStart / 2f;
				mf = minFrequencyMid + deltaMin / 2f;
				
				m_Period = 100.0f / (sf * sf + 0.001f);
				m_MaxPeriod = 100.0f / (mf * mf + 0.001f);
			}
		}
		
		/**
		 * Writes the wave to the supplied buffer ByteArray
		 * @param	buffer		A ByteArray to write the wave to
		 * @return				If the wave is finished
		 */
		public bool Generate(float[] buffer, int length, uint sampleRate = 44100, uint bitDepth = 16)
		{
			m_Finished = false;

			for(var i = 0; i < length; ++i)
			{
				if (m_Finished) 
				{
					return true;					
				}
				
				// Repeats every m_RepeatLimit times, partially resetting the sound parameters
				if(m_RepeatLimit != 0)
				{
					if(++m_RepeatTime >= m_RepeatLimit)
					{
						m_RepeatTime = 0;
						Reset(false);
					}
				}
				
				m_ChangePeriodTime++;
				if (m_ChangePeriodTime >= m_ChangePeriod)
				{				
					m_ChangeTime = 0;
					m_ChangeTime2 = 0;
					m_ChangePeriodTime = 0;
					if (m_ChangeReached)
					{
						m_Period /= m_ChangeAmount;
						m_ChangeReached=false;
					}
					if (m_ChangeReached2)
					{
						m_Period /= m_ChangeAmount2;
						m_ChangeReached2=false;
					}
				}
				
				// If m_ChangeLimit is reached, shifts the pitch
				if(!m_ChangeReached)
				{
					if(++m_ChangeTime >= m_ChangeLimit)
					{
						m_ChangeReached = true;
						m_Period *= m_ChangeAmount;
					}
				}
				
				// If m_ChangeLimit is reached, shifts the pitch
				if(!m_ChangeReached2)
				{
					if(++m_ChangeTime2 >= m_ChangeLimit2)
					{
						m_Period *= m_ChangeAmount2;
						m_ChangeReached2=true;
					}
				}
				
				// Accelerate and apply slide
				m_Slide += m_DeltaSlide;
				m_Period *= m_Slide;
				
				// Checks for frequency getting too low, and stops the sound if a minFrequency was set
				if(m_Period > m_MaxPeriod)
				{
					m_Period = m_MaxPeriod;
					if(m_MinFrequency > 0.0f) 
						m_Muted = true;
										
				}
				
				m_PeriodTemp = m_Period;
				
				// Applies the vibrato effect
				if(m_VibratoAmplitude > 0.0f)
				{
					m_VibratoPhase += m_VibratoSpeed;
					m_PeriodTemp = m_Period * (1.0f + Mathf.Sin(m_VibratoPhase) * m_VibratoAmplitude);
				}
				
				m_PeriodTemp = (int)m_PeriodTemp;
				if(m_PeriodTemp < 8) 
					m_PeriodTemp = 8;
				
				// Sweeps the square duty
				if (m_WaveType == 0)
				{
					m_SquareDuty += m_DutySweep;
					if(m_SquareDuty < 0.0f) 
						m_SquareDuty = 0.0f;
					else if (m_SquareDuty > 0.5f) 
						m_SquareDuty = 0.5f;
				}
				
				// Moves through the different stages of the volume envelope
				if(++m_EnvelopeTime > m_EnvelopeLength)
				{
					m_EnvelopeTime = 0;
					
					switch(++m_EnvelopeStage)
					{
						case 1: 
							m_EnvelopeLength = m_EnvelopeLength1; 
							break;
						case 2: 
							m_EnvelopeLength = m_EnvelopeLength2; 
							break;
					}
				}
				
				// Sets the volume based on the position in the envelope
				switch(m_EnvelopeStage)
				{
					case 0: m_EnvelopeVolume = 
						m_EnvelopeTime * m_EnvelopeOverLength0; 									
						break;
					case 1: 
						m_EnvelopeVolume = 1.0f + (1.0f - m_EnvelopeTime * m_EnvelopeOverLength1) * 2.0f * m_SustainPunch; 
						break;
					case 2: 
						m_EnvelopeVolume = 1.0f - m_EnvelopeTime * m_EnvelopeOverLength2; 								
						break;
					case 3: 
						m_EnvelopeVolume = 0.0f; 
						m_Finished = true; 													
						break;
				}
				
				// Moves the flanger offset
				if (m_Flanger)
				{
					m_FlangerOffset += m_FlangerDeltaOffset;
					m_FlangerInt = (int)m_FlangerOffset;
					if (m_FlangerInt < 0) 	
						 m_FlangerInt = -m_FlangerInt;
					else if (m_FlangerInt > 1023) 
						 m_FlangerInt = 1023;
				}
				
				// Moves the high-pass filter cutoff
				if(m_Filters && m_HpFilterDeltaCutoff != 0.0f)
				{
					m_HpFilterCutoff *= m_HpFilterDeltaCutoff;
					if (m_HpFilterCutoff < 0.00001f) 	
						 m_HpFilterCutoff = 0.00001f;
					else if(m_HpFilterCutoff > 0.1f) 		
						 m_HpFilterCutoff = 0.1f;
				}
				
				m_SuperSample = 0.0f;
				for(var j = 0; j < 8; j++)
				{
					// Cycles through the period
					m_Phase++;
					if(m_Phase >= m_PeriodTemp)
					{
						m_Phase = m_Phase - Mathf.RoundToInt(m_PeriodTemp);
						
						// Generates new random noise for this period
						if (m_WaveType == 3) 
						{ 
							for(var n = 0; n < 32; n++) 
								m_NoiseBuffer[n] = Random.value * 2.0f - 1.0f;
						}
						else if (m_WaveType == 5)
						{
							for(var n = 0; n < 32; ++n) 
								m_PinkNoiseBuffer[n] = m_PinkNumber.GetNextValue();							
						}
						else if (m_WaveType == 6)
						{
							for(var n = 0; n < 32; ++n) 
								m_LoResNoiseBuffer[n] = ((n%k_LoResNoisePeriod) == 0) ? Random.value * 2.0f - 1.0f : m_LoResNoiseBuffer[n-1];							
						}
						else if (m_WaveType == 9)
						{
							// Bitnoise
							// Based on SN76489 periodic "white" noise
							// http://www.smspower.org/Development/SN76489?sid=ae16503f2fb18070f3f40f2af56807f1#NoiseChannel
							// This one matches the behaviour of the SN76489 in the BBC Micro.
							var feedBit = (m_OneBitNoiseState >> 1 & 1) ^ (m_OneBitNoiseState & 1);
							m_OneBitNoiseState = m_OneBitNoiseState >> 1 | (feedBit << 14);
							m_OneBitNoise = (~m_OneBitNoiseState & 1) - 0.5f;
							
						} 
						else if (m_WaveType == 11)
						{
							// Based on SN76489 periodic "white" noise
							// http://www.smspower.org/Development/SN76489?sid=ae16503f2fb18070f3f40f2af56807f1#NoiseChannel
							// This one doesn't match the behaviour of anything real, but it made a nice sound, so I kept it.
							var fb = (m_BuzzState >> 3 & 1) ^ (m_BuzzState & 1);
							m_BuzzState = m_BuzzState >> 1 | (fb << 14);
							m_Buzz = (~m_BuzzState & 1) - 0.5f;
							
						}
					}
					
					m_Sample = 0;
					var overtoneStrength = 1f;
					for (var k = 0; k <= m_Overtones; ++k)
					{
						var tempPhase = (m_Phase * (k + 1)) % m_PeriodTemp;
						// Gets the sample from the oscillator
						var waveType  = m_WaveType;
						if (waveType == 10)
							waveType = (uint)Mathf.Floor(m_Phase / 4) % 10;
						
						switch(waveType)
						{
							case 0: // Square wave
							{
								m_Sample += overtoneStrength*(((tempPhase / m_PeriodTemp) < m_SquareDuty) ? 0.5f : -0.5f);
								break;
							}
							case 1: // Saw wave
							{
								m_Sample += overtoneStrength*(1.0f - (tempPhase / m_PeriodTemp) * 2.0f);
								break;
							}
							case 2: // Sine wave (fast and accurate approx)
							{								
								 m_Pos = tempPhase / m_PeriodTemp;
								 m_Pos = m_Pos > 0.5f ? (m_Pos - 1.0f) * 6.28318531f : m_Pos * 6.28318531f;
								var tempSample = m_Pos < 0f ? 1.27323954f * m_Pos + .405284735f * m_Pos * m_Pos : 1.27323954f * m_Pos - 0.405284735f * m_Pos * m_Pos;
								m_Sample += overtoneStrength*(tempSample < 0f ? .225f * (tempSample *-tempSample - tempSample) + tempSample : .225f * (tempSample * tempSample - tempSample) + tempSample);								
								break;
							}
							case 3: // Noise
							{
								m_Sample += overtoneStrength*(m_NoiseBuffer[(int)(tempPhase * 32 / (int)m_PeriodTemp) % 32]);
								break;
							}
							case 4: // Triangle Wave
							{						
								m_Sample += overtoneStrength*(Mathf.Abs(1-(tempPhase / m_PeriodTemp)*2)-1);
								break;
							}
							case 5: // Pink Noise
							{						
								m_Sample += overtoneStrength * (m_PinkNoiseBuffer[(int)(tempPhase * 32 / (int)m_PeriodTemp)%32]);
								break;
							}
							case 6: // tan
							{
								//detuned
								m_Sample += Mathf.Tan(Mathf.PI * tempPhase / m_PeriodTemp) * overtoneStrength;
								break;
							}
							case 7: // Whistle 
							{				
								// Sin wave code
								m_Pos = tempPhase / m_PeriodTemp;
								m_Pos = m_Pos > 0.5f ? (m_Pos - 1.0f) * 6.28318531f : m_Pos * 6.28318531f;
								var tempSample = m_Pos < 0f ? 1.27323954f * m_Pos + .405284735f * m_Pos * m_Pos : 1.27323954f * m_Pos - 0.405284735f * m_Pos * m_Pos;
								var value = 0.75f * (tempSample < 0 ? .225f * (tempSample *-tempSample - tempSample) + tempSample : .225f * (tempSample * tempSample - tempSample) + tempSample);
								//then whistle (essentially an overtone with frequencyx20 and amplitude0.25
								
								m_Pos = ((tempPhase*20) % m_PeriodTemp) / m_PeriodTemp;
								m_Pos = m_Pos > 0.5f ? (m_Pos - 1.0f) * 6.28318531f : m_Pos * 6.28318531f;
								tempSample = m_Pos < 0f ? 1.27323954f * m_Pos + .405284735f * m_Pos * m_Pos : 1.27323954f * m_Pos - 0.405284735f * m_Pos * m_Pos;
								value += 0.25f*(tempSample < 0 ? .225f * (tempSample *-tempSample - tempSample) + tempSample : .225f * (tempSample * tempSample - tempSample) + tempSample);
								
								m_Sample += overtoneStrength*value;//main wave
								
								break;
							}
							case 8: // Breaker
							{	
								var amp = tempPhase/m_PeriodTemp;								
								m_Sample += overtoneStrength*(Mathf.Abs(1-amp*amp*2)-1);
								break;
							}
							case 9: // Bitnoise (1-bit periodic "white" noise)
							{
								m_Sample += overtoneStrength*m_OneBitNoise;
								break;
							}
							case 11: //new2
							{
								m_Sample += overtoneStrength*m_Buzz;
								break;
							}
						}
						overtoneStrength*=(1-m_OvertoneFalloff);
						
					}					
					
					// Applies the low and high pass filters
					if (m_Filters)
					{
						m_LpFilterOldPos = m_LpFilterPos;
						m_LpFilterCutoff *= m_LpFilterDeltaCutoff;
						
						if (m_LpFilterCutoff < 0.0f) 
							m_LpFilterCutoff = 0.0f;
						else if (m_LpFilterCutoff > 0.1f) 
							m_LpFilterCutoff = 0.1f;
						
						if(m_LpFilterOn)
						{
							m_LpFilterDeltaPos += (m_Sample - m_LpFilterPos) * m_LpFilterCutoff;
							m_LpFilterDeltaPos *= m_LpFilterDamping;
						}
						else
						{
							m_LpFilterPos = m_Sample;
							m_LpFilterDeltaPos = 0.0f;
						}
						
						m_LpFilterPos += m_LpFilterDeltaPos;
						
						m_HpFilterPos += m_LpFilterPos - m_LpFilterOldPos;
						m_HpFilterPos *= 1.0f - m_HpFilterCutoff;
						m_Sample = m_HpFilterPos;
					}
					
					// Applies the flanger effect
					if (m_Flanger)
					{
						m_FlangerBuffer[m_FlangerPos&1023] = m_Sample;
						m_Sample += m_FlangerBuffer[(m_FlangerPos - m_FlangerInt + 1024) & 1023];
						m_FlangerPos = (m_FlangerPos + 1) & 1023;
					}
					
					m_SuperSample += m_Sample;
				}
				
				// Clipping if too loud
				m_SuperSample = Mathf.Clamp(m_SuperSample, -1f, 1f);

				// Averages out the super samples and applies volumes
				//m_SuperSample = m_MasterVolume * m_EnvelopeVolume * m_SuperSample * 0.125f;				
				
				//BIT CRUSH				
				m_BitcrushPhase += m_BitcrushFreq;
				if (m_BitcrushPhase > 1)
				{
					m_BitcrushPhase = 0;
					m_BitcrushLast = m_SuperSample;	 
				}
				m_BitcrushFreq = Mathf.Max(Mathf.Min(m_BitcrushFreq + m_BitcrushFreqSweep,1f),0f);
				
				m_SuperSample = m_BitcrushLast;

				//compressor
				if (m_SuperSample > 0)
					 m_SuperSample = Mathf.Pow(m_SuperSample,m_CompressionFactor);
				else
					 m_SuperSample = -Mathf.Pow(-m_SuperSample,m_CompressionFactor);

				if (m_Muted)
					 m_SuperSample = 0;

				buffer[i] = m_SuperSample;
			}
			
			return false;
		}
    }
}
