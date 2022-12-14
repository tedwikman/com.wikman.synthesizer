/***
* SfxrParams
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wikman.Synthesizer.Sfxr
{
	internal enum ParameterType
	{
		None,
		WaveType,
		MasterVolume,
		AttackTime,
		SustainTime,
		SustainPunch,
		DecayTime,
		CompressionAmount,
		StartFrequency,
		MinFrequency,
		Slide,
		DeltaSlide,
		VibratoDepth,
		VibratoSpeed,
		Overtones,
		OvertoneFalloff,
		ChangeRepeat,
		ChangeAmount,
		ChangeSpeed,
		ChangeAmount2,
		ChangeSpeed2,
		SquareDuty,
		DutySweep,
		RepeatSpeed,
		FlangerOffset,
		FlangerSweep,
		LpFilterCutoff,
		LpFilterCutoffSweep,
		LpFilterResonance,
		HpFilterCutoff,
		HpFilterCutoffSweep,
		BitCrush,
		BitCrushSweep
	}
	
	internal class ParameterData
	{
		public string realName { get; set; }
		public string description { get; set; }
		public int grouping { get; set; }
		public ParameterType parameterType { get; set; }
		public float defaultValue { get; set; }
		public float minimum { get; set; }
		public float maximum { get; set; }
	}	
	
	internal class ParameterDatabase
    {
	    public static readonly ParameterData[] database =
		{
			new ParameterData()
			{
				realName = "Wave Type",
				description = "Shape of the wave.",
				grouping = 0,
				parameterType = ParameterType.WaveType,
				defaultValue = 2f,
				minimum = 0f,
				maximum = 12f
			},
			new ParameterData()
			{
				realName = "Sound Volume",
				description = "Overall volume of the current sound.",
				grouping = 1,
				parameterType = ParameterType.MasterVolume,
				defaultValue = 0.5f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Attack Time",
				description = "Length of the volume envelope attack.",
				grouping = 1,
				parameterType = ParameterType.AttackTime,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Sustain Time",
				description = "Length of the volume envelope sustain.",
				grouping = 1,
				parameterType = ParameterType.SustainTime,
				defaultValue = 0.3f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Punch",
				description = "Tilts the sustain envelope for more 'pop'.",
				grouping = 1,
				parameterType = ParameterType.SustainPunch,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Decay Time",
				description = "Length of the volume envelope decay.",
				grouping = 1,
				parameterType = ParameterType.DecayTime,
				defaultValue = 0.4f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Compression",
				description = "Pushes amplitudes together into a narrower range to make them stand out more.",
				grouping = 15,
				parameterType = ParameterType.CompressionAmount,
				defaultValue = 0.3f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Frequency",
				description = "Base note of the sound.",
				grouping = 2,
				parameterType = ParameterType.StartFrequency,
				defaultValue = 0.3f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Frequency Cutoff",
				description = "If sliding, the sound will stop at this frequency, to prevent really low notes.",
				grouping = 2,
				parameterType = ParameterType.MinFrequency,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Frequency Slide",
				description = "Slides the frequency up or down.",
				grouping = 3,
				parameterType = ParameterType.Slide,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Delta Slide",
				description = "Accelerates the frequency slide.  Can be used to get the frequency to change direction.",
				grouping = 3,
				parameterType = ParameterType.DeltaSlide,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Vibrato Depth",
				description = "Strength of the vibrato effect.",
				grouping = 4,
				parameterType = ParameterType.VibratoDepth,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Vibrato Speed",
				description = "Speed of the vibrato effect (i.e. frequency).",
				grouping = 4,
				parameterType = ParameterType.VibratoSpeed,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Harmonics",
				description = "Overlays copies of the waveform with copies and multiples of its frequency.",
				grouping = 13,
				parameterType = ParameterType.Overtones,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Harmonics Falloff",
				description = "The rate at which higher overtones should decay.",
				grouping = 13,
				parameterType = ParameterType.OvertoneFalloff,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Pitch Jump Repeat Speed",
				description = "Larger Values means more pitch jumps, which can be useful for aggregation.",
				grouping = 5,
				parameterType = ParameterType.ChangeRepeat,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Pitch Jump Amount 1",
				description = "Jump in pitch, either up or down.",
				grouping = 5,
				parameterType = ParameterType.ChangeAmount,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Pitch Jump Onset 1",
				description = "How quickly the note shift happens.",
				grouping = 5,
				parameterType = ParameterType.ChangeSpeed,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Pitch Jump Amount 2",
				description = "Jump in pitch, either up or down.",
				grouping = 5,
				parameterType = ParameterType.ChangeAmount2,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Pitch Jump Onset 2",
				description = "How quickly the note shift happens.",
				grouping = 5,
				parameterType = ParameterType.ChangeSpeed2,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Square Duty",
				description = "Square waveform only : Controls the ratio between the up and down states of the square wave, changing the tibre.",
				grouping = 8,
				parameterType = ParameterType.SquareDuty,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Duty Sweep",
				description = "Square waveform only : Sweeps the duty up or down.",
				grouping = 8,
				parameterType = ParameterType.DutySweep,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Repeat Speed",
				description = "Speed of the note repeating - certain variables are reset each time.",
				grouping = 9,
				parameterType = ParameterType.RepeatSpeed,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Flanger Offset",
				description = "Offsets a second copy of the wave by a small phase, changing the tibre.",
				grouping = 10,
				parameterType = ParameterType.FlangerOffset,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Flanger Sweep",
				description = "Sweeps the phase up or down.",
				grouping = 10,
				parameterType = ParameterType.FlangerSweep,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Low-pass Filter Cutoff",
				description = "Frequency at which the low-pass filter starts attenuating higher frequencies.",
				grouping = 11,
				parameterType = ParameterType.LpFilterCutoff,
				defaultValue = 1f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Low-pass Filter Cutoff Sweep",
				description = "Sweeps the low-pass cutoff up or down.",
				grouping = 11,
				parameterType = ParameterType.LpFilterCutoffSweep,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Low-pass Filter Resonance",
				description = "Changes the attenuation rate for the low-pass filter, changing the timbre.",
				grouping = 11,
				parameterType = ParameterType.LpFilterResonance,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "High-pass Filter Cutoff",
				description = "Frequency at which the high-pass filter starts attenuating lower frequencies.",
				grouping = 12,
				parameterType = ParameterType.HpFilterCutoff,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "High-pass Filter Cutoff Sweep",
				description = "Sweeps the high-pass cutoff up or down.",
				grouping = 12,
				parameterType = ParameterType.HpFilterCutoffSweep,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Bit Crush",
				description = "Resamples the audio at a lower frequency.",
				grouping = 14,
				parameterType = ParameterType.BitCrush,
				defaultValue = 0f,
				minimum = 0f,
				maximum = 1f
			},
			new ParameterData()
			{
				realName = "Bit Crush Sweep",
				description = "Sweeps the Bit Crush filter up or down.",
				grouping = 14,
				parameterType = ParameterType.BitCrushSweep,
				defaultValue = 0f,
				minimum = -1f,
				maximum = 1f
			}
		};

	    // Stores values for all the parameters above
		readonly Dictionary<ParameterType, float> m_Params = new Dictionary<ParameterType, float>();
		// Stores a list of strings, these strings represent parameters that will be locked during randomization/mutation
		readonly List<ParameterType> m_LockedParams = new List<ParameterType>();							

		public ParameterDatabase()
		{
			for (var i = 0; i < database.Length; ++i)
			{
				var data = database[i];
				m_Params.Add(data.parameterType, -100f);
			}
			
			ResetParams();
		}
		
		public float GetDefault(ParameterType parameterType)
		{
			return GetProperty(parameterType).defaultValue;
		}
		
		public float GetMin(ParameterType parameterType)
		{
			return GetProperty(parameterType).minimum;
		}
		
		public float GetMax(ParameterType parameterType)
		{
			return GetProperty(parameterType).maximum;
		}
		
		static ParameterData GetProperty(ParameterType parameterType)
		{
			for (var i = 0; i < database.Length; i++)
			{
				if (database[i].parameterType == parameterType)
				{
					return database[i];
				}
			}

			Debug.Log($"Could not find param with name: {parameterType}");
			return null;
		}
		
		public float GetParam(ParameterType parameterType)
		{
			if(!m_Params.ContainsKey(parameterType))
			{
				Debug.Log($"Could not find param with name: {parameterType}");
			}
			
			return m_Params[parameterType];			
		}		

		public void SetParam(ParameterType parameterType, float value, bool checkLocked = false)
		{
			if(!m_Params.ContainsKey(parameterType))
			{
				Debug.Log($"Could not set param. Param not found: {parameterType}");
				return;
			}

			if (checkLocked)
			{
				if (IsParamLocked(parameterType) == true)
					return;
			}
			
			m_Params[parameterType] = Mathf.Clamp(value, GetMin(parameterType),GetMax(parameterType));
		}
		
		public bool IsParamLocked(ParameterType parameterType)
		{
			return m_LockedParams.FindIndex(x => x == parameterType) >= 0;
		}
		
		public void SetAllLocked(bool locked)
		{
			m_LockedParams.Clear();
			
			if (locked)
			{
				for (var i = 0; i < database.Length; ++i)
				{
					m_LockedParams.Add(database[i].parameterType);
				}
			}
		}
		
		public void SetParamLocked(ParameterType parameterType, bool locked)
		{
			var index = m_LockedParams.FindIndex(x => x == parameterType);
			
			if (locked)
			{
				if (index == -1)
				{
					m_LockedParams.Add(parameterType);
				}
			}
			else
			{
				if (index >= 0)
				{
					m_LockedParams.RemoveAt(index);
				}
			}
		}

		public void ResetParams(List<ParameterType> paramsToReset = null, bool allowUnlock = true)
		{
			if (allowUnlock && paramsToReset == null)
			{
				m_LockedParams.Clear();
				//lock this one by default
				m_LockedParams.Add(ParameterType.MasterVolume);
			}

			var paramKeys = m_Params.Keys.ToList();
			foreach (var param in paramKeys)
			{
				if (paramsToReset == null || paramsToReset.FindIndex(x => x == param) >= 0) 
				{
					if (IsParamLocked(param) == false)
					{
						m_Params[param] = GetDefault(param);
					}
				}
			}
		}
    }
}
