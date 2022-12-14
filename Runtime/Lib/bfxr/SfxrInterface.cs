using System.Collections.Generic;

namespace Wikman.Synthesizer.Sfxr
{
	internal class SfxrInterface
	{
		uint m_SampleRate = 44100;
		uint m_BitDepth = 16;
		
		readonly SfxrGenerator m_Generator = new SfxrGenerator();
		
		public uint sampleRate
		{
			get => m_SampleRate;
			set => m_SampleRate = value;
		}

		public uint bitDepth
		{
			get => m_BitDepth;
			set => m_BitDepth = value;
		}

		public void GenerateRandom(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.Randomize(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}

		public void GeneratePickup(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GeneratePickupCoin(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}
		
		public void GenerateLaser(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GenerateLaserShoot(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}		
		
		public void GenerateExplosion(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GenerateExplosion(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}		
		
		public void GeneratePowerUp(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GeneratePowerUp(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}	
		
		public void GenerateHit(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GenerateHitHurt(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}
		
		public void GenerateJump(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GenerateJump(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}		
		
		public void GenerateBlip(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			TemplateEffects.GenerateBlipSelect(m_Generator.parameters);
			CreateWithData(inputData, out audioBuffer, out noOfSamples, out outputData);
		}
		
		public void CreateWithData(Dictionary<ParameterType, float> inputData, out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> outputData)
		{
			OverrideWithInputData(inputData);
			Generate(out audioBuffer, out noOfSamples, out outputData);
		}		

		void OverrideWithInputData(Dictionary<ParameterType, float> inputData)
		{
			if (inputData == null)
				return;
			
			foreach (var parameterType in inputData.Keys)
				m_Generator.parameters.SetParam(parameterType, inputData[parameterType]);
		}

		void Generate(out float[] audioBuffer, out int noOfSamples, out Dictionary<ParameterType, float> parameterData)
		{
			m_Generator.Reset(true);
			
			noOfSamples = m_Generator.GetNoOfSamples();
			audioBuffer = new float[noOfSamples];
			
			m_Generator.Generate(audioBuffer, noOfSamples, m_SampleRate, m_BitDepth);

			parameterData = new Dictionary<ParameterType, float>();
			foreach (var paramId in System.Enum.GetValues(typeof(ParameterType)))
			{
				var param = (ParameterType)paramId;
				if (param == ParameterType.None)
					continue;
				parameterData.Add(param, m_Generator.parameters.GetParam(param));
			}
		}
	}
}
