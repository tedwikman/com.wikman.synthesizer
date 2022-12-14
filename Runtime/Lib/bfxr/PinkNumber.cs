using UnityEngine;
using System.Collections.Generic;

namespace Wikman.Synthesizer.Sfxr
{
	internal class PinkNumber
	{
		readonly int m_MaxKey;
		readonly List<uint> m_WhiteValues = new List<uint>();
		readonly uint m_Range;
		int m_Key;

		public PinkNumber()
		{
			// Five bits set
			m_MaxKey = 0x1f;
			m_Range = 128;
			m_Key = 0;
			for (var i = 0; i < 5; i++)
				m_WhiteValues.Add((uint)(Random.value * (m_Range / 5f)));
		}
		
		// Returns a number between -1 and 1		
		public float GetNextValue()
		{
			var lastKey = m_Key;
			
			m_Key++;
			if (m_Key > m_MaxKey)
				m_Key = 0;
			// Exclusive-Or previous value with current value. This gives
			// a list of bits that have changed.
			var diff = lastKey ^ m_Key;
			uint sum = 0;
			for (var i = 0; i < 5; ++i)
			{
				// If bit changed get new random number for corresponding
				// white_value
				if ((diff & (1 << i)) > 0)
					m_WhiteValues[i] = ((uint)(Random.value * (m_Range / 5f)));
				sum += m_WhiteValues[i];
			}
			return sum / 64f - 1f;
		}
	}; 
}