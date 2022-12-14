using UnityEngine;
using System.Collections.Generic;

namespace Wikman.Synthesizer.Sfxr
{
    internal static class TemplateEffects
    {
	    // Some constants used for weighting random values 
	    static readonly Dictionary<ParameterType, float> k_RandomizationPower = new Dictionary<ParameterType, float>()
	    {
		    { ParameterType.AttackTime, 4 },
		    { ParameterType.SustainTime, 2f },
		    { ParameterType.SustainPunch, 2f },
		    { ParameterType.Overtones, 3f },
		    { ParameterType.OvertoneFalloff, 0.25f },
		    { ParameterType.VibratoDepth, 3f },
		    { ParameterType.DutySweep, 3f },
		    { ParameterType.FlangerOffset, 3f },
		    { ParameterType.FlangerSweep, 3f },
		    { ParameterType.LpFilterCutoff, 0.3f },
		    { ParameterType.LpFilterCutoffSweep, 3f },
		    { ParameterType.HpFilterCutoff, 5f },
		    { ParameterType.HpFilterCutoffSweep, 5f },
		    { ParameterType.BitCrush, 4f },
		    { ParameterType.BitCrushSweep, 5f }
	    };
			
	    static readonly int[] k_WaveTypeWeights =
	    {
		    1, // 0:square
		    1, // 1:saw
		    1, // 2:sin
		    1, // 3:noise
		    1, // 4:triangle
		    1, // 5:pink
		    1, // 6:tan
		    1, // 7:whistle
		    1, // 8:breaker
		    1, // 9:bitnoise
		    1, // 10:new 1
		    1, // 11:buzz
	    };	    
	    
        public static void GeneratePickupCoin(ParameterDatabase database)
        {
            database.ResetParams(null,false);
			
            database.SetParam(ParameterType.StartFrequency,0.4f + Random.value * 0.5f,true);
			
            database.SetParam(ParameterType.SustainTime, Random.value * 0.1f,true);
            database.SetParam(ParameterType.DecayTime, 0.1f + Random.value * 0.4f,true);
            database.SetParam(ParameterType.SustainPunch, 0.3f + Random.value * 0.3f,true);
			
            if (Random.value < 0.5f) 
            {
                database.SetParam(ParameterType.ChangeSpeed, 0.5f + Random.value * 0.2f,true);
                var cnum = (int)(Random.value*7f) + 1;
                var cden = cnum + (int)(Random.value * 7f) + 2;
				
                database.SetParam(ParameterType.ChangeAmount, cnum / (float)cden,true);
            }
        }
        
		public static void GenerateLaserShoot(ParameterDatabase database)
		{
			database.ResetParams(null,false);
			
			database.SetParam(ParameterType.WaveType,(uint)(Random.value * 3),true);
			if ((int)database.GetParam(ParameterType.WaveType) == 2 && Random.value < 0.5f) 
			{
				database.SetParam(ParameterType.WaveType, 
					(uint)(Random.value * 2),true);
			}
			
			database.SetParam(ParameterType.StartFrequency, 
				0.5f + Random.value * 0.5f,true);
			database.SetParam(ParameterType.MinFrequency, 
				database.GetParam(ParameterType.StartFrequency) - 0.2f - Random.value * 0.6f,true);
			
			if (database.GetParam(ParameterType.MinFrequency) < 0.2f) 
				database.SetParam(ParameterType.MinFrequency,0.2f,true);
			
			database.SetParam(ParameterType.Slide, -0.15f - Random.value * 0.2f,true);			
			 
			if (Random.value < 0.33f)
			{
				database.SetParam(ParameterType.StartFrequency, Random.value * 0.6f,true);
				database.SetParam(ParameterType.MinFrequency, Random.value * 0.1f,true);
				database.SetParam(ParameterType.Slide, -0.35f - Random.value * 0.3f,true);
			}
			
			if (Random.value < 0.5f) 
			{
				database.SetParam(ParameterType.SquareDuty, Random.value * 0.5f,true);
				database.SetParam(ParameterType.DutySweep, Random.value * 0.2f,true);
			}
			else
			{
				database.SetParam(ParameterType.SquareDuty, 0.4f + Random.value * 0.5f,true);
				database.SetParam(ParameterType.DutySweep,- Random.value * 0.7f,true);	
			}
			
			database.SetParam(ParameterType.SustainTime, 0.1f + Random.value * 0.2f,true);
			database.SetParam(ParameterType.DecayTime, Random.value * 0.4f,true);
			if (Random.value < 0.5f) 
				database.SetParam(ParameterType.SustainPunch, Random.value * 0.3f,true);
			
			if (Random.value < 0.33f)
			{
				database.SetParam(ParameterType.FlangerOffset, Random.value * 0.2f,true);
				database.SetParam(ParameterType.FlangerSweep, -Random.value * 0.2f,true);
			}
			
			if (Random.value < 0.5f) 
				database.SetParam(ParameterType.HpFilterCutoff, Random.value * 0.3f,true);
		}  
		
		public static void GenerateExplosion(ParameterDatabase database)
		{
			database.ResetParams(null,false);
			if (Random.value < 0.5f)
				database.SetParam(ParameterType.WaveType, 3,true);
			else				
				database.SetParam(ParameterType.WaveType, 9,true);

			if (Random.value < 0.5f)
			{
				database.SetParam(ParameterType.StartFrequency, 0.1f + Random.value * 0.4f,true);
				database.SetParam(ParameterType.Slide, -0.1f + Random.value * 0.4f,true);
			}
			else
			{
				database.SetParam(ParameterType.StartFrequency, 0.2f + Random.value * 0.7f,true);
				database.SetParam(ParameterType.Slide, -0.2f - Random.value * 0.2f,true);
			}
			
			database.SetParam(ParameterType.StartFrequency, database.GetParam(ParameterType.StartFrequency) * database.GetParam(ParameterType.StartFrequency),true);
			
			if (Random.value < 0.2f) 
				database.SetParam(ParameterType.Slide, 0.0f,true);
			if (Random.value < 0.33f) 
				database.SetParam(ParameterType.RepeatSpeed, 0.3f + Random.value * 0.5f,true);
			
			database.SetParam(ParameterType.SustainTime, 0.1f + Random.value * 0.3f,true);
			database.SetParam(ParameterType.DecayTime, Random.value * 0.5f,true);
			database.SetParam(ParameterType.SustainPunch, 0.2f + Random.value * 0.6f,true);
			
			if (Random.value < 0.5f)
			{
				database.SetParam(ParameterType.FlangerOffset, -0.3f + Random.value * 0.9f,true);
				database.SetParam(ParameterType.FlangerSweep, -Random.value * 0.3f,true);
			}
			
			if (Random.value < 0.33f)
			{
				database.SetParam(ParameterType.ChangeSpeed, 0.6f + Random.value * 0.3f,true);
				database.SetParam(ParameterType.ChangeAmount, 0.8f - Random.value * 1.6f,true);
			}
		}
		
		public static void GeneratePowerUp(ParameterDatabase database)
		{
			database.ResetParams(null,false);
			
			if (Random.value < 0.5f) 
				database.SetParam(ParameterType.WaveType, 1,true);
			else 					
				database.SetParam(ParameterType.SquareDuty, Random.value * 0.6f,true);
			
			if (Random.value < 0.5f)
			{
				database.SetParam(ParameterType.StartFrequency, 0.2f + Random.value * 0.3f,true);
				database.SetParam(ParameterType.Slide, 0.1f + Random.value * 0.4f,true);
				database.SetParam(ParameterType.RepeatSpeed, 0.4f + Random.value * 0.4f,true);
			}
			else
			{
				database.SetParam(ParameterType.StartFrequency, 0.2f + Random.value * 0.3f,true);
				database.SetParam(ParameterType.Slide, 0.05f + Random.value * 0.2f,true);
				
				if (Random.value < 0.5f)
				{
					database.SetParam(ParameterType.VibratoDepth, Random.value * 0.7f,true);
					database.SetParam(ParameterType.VibratoSpeed, Random.value * 0.6f,true);
				}
			}
			
			database.SetParam(ParameterType.SustainTime, Random.value * 0.4f,true);
			database.SetParam(ParameterType.DecayTime, 0.1f + Random.value * 0.4f,true);
		}	
		
		public static void GenerateHitHurt(ParameterDatabase database)
		{
			database.ResetParams(null,false);
			
			database.SetParam(ParameterType.WaveType, (uint)Random.value * 4f,true);
			if ((int)database.GetParam(ParameterType.WaveType) == 2) 
				database.SetParam(ParameterType.WaveType, 3,true);	// White noise
			else if ((int)database.GetParam(ParameterType.WaveType) == 3) 
				database.SetParam(ParameterType.WaveType, 9,true);	// Bit noise
			else if ((int)database.GetParam(ParameterType.WaveType) == 0) 
				database.SetParam(ParameterType.SquareDuty, Random.value * 0.6f);
			
			database.SetParam(ParameterType.StartFrequency, 0.2f + Random.value * 0.6f,true);
			database.SetParam(ParameterType.Slide, -0.3f - Random.value * 0.4f,true);
			
			database.SetParam(ParameterType.SustainTime, Random.value * 0.1f,true);
			database.SetParam(ParameterType.DecayTime, 0.1f + Random.value * 0.2f,true);
			
			if (Random.value < 0.5f) 
				database.SetParam(ParameterType.HpFilterCutoff, Random.value * 0.3f,true);
		}		
		
		public static void GenerateJump(ParameterDatabase database)
		{
			database.ResetParams(null,false);
			
			database.SetParam(ParameterType.WaveType, 0f,true);
			database.SetParam(ParameterType.SquareDuty, Random.value * 0.6f,true);
			database.SetParam(ParameterType.StartFrequency, 0.3f + Random.value * 0.3f,true);
			database.SetParam(ParameterType.Slide, 0.1f + Random.value * 0.2f,true);
			
			database.SetParam(ParameterType.SustainTime, 0.1f + Random.value * 0.3f,true);
			database.SetParam(ParameterType.DecayTime, 0.1f + Random.value * 0.2f,true);
			
			if (Random.value < 0.5f) 
				database.SetParam(ParameterType.HpFilterCutoff, Random.value * 0.3f,true);
			if (Random.value < 0.5f) 
				database.SetParam(ParameterType.LpFilterCutoff, 1.0f - Random.value * 0.6f,true);
		}
		
		public static void GenerateBlipSelect(ParameterDatabase database)
		{
			database.ResetParams(null,false);
			
			database.SetParam(ParameterType.WaveType, (uint)Random.value * 2f,true);
			if ((int)database.GetParam(ParameterType.WaveType) == 0) 
				database.SetParam(ParameterType.SquareDuty, Random.value * 0.6f,true);
			
			database.SetParam(ParameterType.StartFrequency, 0.2f + Random.value * 0.4f,true);
			
			database.SetParam(ParameterType.SustainTime, 0.1f + Random.value * 0.1f,true);
			database.SetParam(ParameterType.DecayTime, Random.value * 0.2f,true);
			database.SetParam(ParameterType.HpFilterCutoff, 0.1f,true);
		}	
		
		public static void Randomize(ParameterDatabase database)
		{
			foreach (var paramId in System.Enum.GetValues(typeof(ParameterType)))
			{
				var param = (ParameterType)paramId;
				if (param == ParameterType.None)
					continue;
				
				if (!database.IsParamLocked(param))
				{
					var min = database.GetMin(param);
					var max = database.GetMax(param);
					var r = Random.value;
					if (k_RandomizationPower.ContainsKey(param))
						r = Mathf.Pow(r, k_RandomizationPower[param]);
					database.SetParam(param, min  + (max-min)*r);
				}
			}
			
			if (!database.IsParamLocked(ParameterType.WaveType))
			{
				var count = 0;
				for (var i = 0; i < k_WaveTypeWeights.Length;i++)
				{
					count += k_WaveTypeWeights[i];
				}
				var r = Random.value * count;
				for (var i = 0; i < k_WaveTypeWeights.Length;i++)
				{
					r -= k_WaveTypeWeights[i];
					if (r<=0)
					{
						database.SetParam(ParameterType.WaveType,i);
						break;
					}
				}
				
			}
			
			if (!database.IsParamLocked(ParameterType.RepeatSpeed))
			{
				if (Random.value < 0.5f)
					database.SetParam(ParameterType.RepeatSpeed,0f);
			}
						
			if (!database.IsParamLocked(ParameterType.Slide))
			{
				var r= Random.value*2-1;
				r = Mathf.Pow(r,5);
				database.SetParam(ParameterType.Slide,r);
			}
			if (!database.IsParamLocked(ParameterType.DeltaSlide))
			{
				var r = Random.value * 2 - 1;
				r = Mathf.Pow(r,3);
				database.SetParam(ParameterType.DeltaSlide,r);
			}
			
			if (!database.IsParamLocked(ParameterType.MinFrequency))
				database.SetParam(ParameterType.MinFrequency,0);
			
			if (!database.IsParamLocked(ParameterType.StartFrequency))
				database.SetParam(ParameterType.StartFrequency, (Random.value < 0.5f) ? Mathf.Pow(Random.value * 2f - 1f, 2) : (Mathf.Pow(Random.value * 0.5f, 3) + 0.5f));
			
			if ((!database.IsParamLocked(ParameterType.SustainTime)) && (!database.IsParamLocked(ParameterType.DecayTime)))
			{
				if(database.GetParam(ParameterType.AttackTime) + database.GetParam(ParameterType.SustainTime) + database.GetParam(ParameterType.DecayTime) < 0.2)
				{
					database.SetParam(ParameterType.SustainTime, 0.2f + Random.value * 0.3f);
					database.SetParam(ParameterType.DecayTime, 0.2f + Random.value * 0.3f);
				}
			}
			
			if (!database.IsParamLocked(ParameterType.Slide))
			{
				if((database.GetParam(ParameterType.StartFrequency) > 0.7 && 
					database.GetParam(ParameterType.Slide) > 0.2) || 
					(database.GetParam(ParameterType.StartFrequency) < 0.2 && 
						database.GetParam(ParameterType.Slide) < -0.05)) 
				{
					database.SetParam(ParameterType.Slide, -database.GetParam(ParameterType.Slide));
				}
			}
			
			if (!database.IsParamLocked(ParameterType.LpFilterCutoffSweep))
			{
				if(database.GetParam(ParameterType.LpFilterCutoff) < 0.1 && database.GetParam(ParameterType.LpFilterCutoffSweep) < -0.05) 
				{
					database.SetParam(ParameterType.LpFilterCutoffSweep, -database.GetParam(ParameterType.LpFilterCutoffSweep));
				}
			}
		}
		
		public static void Mutate(ParameterDatabase database, float mutation = 0.05f)
		{			
			foreach (var paramId in System.Enum.GetValues(typeof(ParameterType)))
			{
				var param = (ParameterType)paramId;
				if (param == ParameterType.None)
					continue;
				
				if (!database.IsParamLocked(param))
				{
					if (Random.value < 0.5f)
					{
						database.SetParam(param, database.GetParam(param) + Random.value * mutation * 2 - mutation);
					}
				}
			}
		}		
    }
}
