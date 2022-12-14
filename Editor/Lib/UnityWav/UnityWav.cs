/*
	<summary>
	WAV utility for recording and audio playback functions in Unity.
	Version: 1.0 alpha 1

	- Use "ToAudioClip" method for loading wav file / bytes.
	Loads .wav (PCM uncompressed) files at 8,16,24 and 32 bits and converts data to Unity's AudioClip.

	- Use "FromAudioClip" method for saving wav file / bytes.
	Converts an AudioClip's float data into wav byte array at 16 bit.
	</summary>
	<remarks>
	For documentation and usage examples: https://github.com/deadlyfingers/UnityWav
	</remarks>
*/

using UnityEngine;
using System.Text;
using System.IO;
using System;

namespace Wikman.Synthesizer.UnityWav
{
	internal class UnityWav
	{
		const ushort k_BitDepth = 16;

		/// <summary>
		/// Load PCM format *.wav audio file (using Unity's Application data path) and convert to AudioClip.
		/// </summary>
		/// <returns>The AudioClip.</returns>
		/// <param name="filePath">Local file path to .wav file</param>
		public static AudioClip ToAudioClip(string filePath)
		{
			if (!filePath.StartsWith(Application.persistentDataPath) && !filePath.StartsWith (Application.dataPath)) 
			{
				Debug.LogWarning("This only supports files that are stored using Unity's Application data path. \nTo load bundled resources use 'Resources.Load(\"filename\") typeof(AudioClip)' method. \nhttps://docs.unity3d.com/ScriptReference/Resources.Load.html");
				return null;
			}
			var fileBytes = File.ReadAllBytes(filePath);
			return ToAudioClip(fileBytes, 0);
		}

		public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
		{
			var subChunk = BitConverter.ToInt32(fileBytes, 16);
			var audioFormat = BitConverter.ToUInt16(fileBytes, 20);

			// NB: Only uncompressed PCM wav files are supported.
			var formatCode = FormatCode(audioFormat);
			Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

			var channels = BitConverter.ToUInt16(fileBytes, 22);
			var sampleRate = BitConverter.ToInt32(fileBytes, 24);
			var bitDepth = BitConverter.ToUInt16(fileBytes, 34);

			var headerOffset = 16 + 4 + subChunk + 4;
			var subChunk2 = BitConverter.ToInt32(fileBytes, headerOffset);

			float[] data;
			switch (bitDepth) 
			{
				case 8:
					data = Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
					break;
				case 16:
					data = Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
					break;
				case 24:
					data = Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
					break;
				case 32:
					data = Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
					break;
				default:
					throw new Exception(bitDepth + " bit depth is not supported.");
			}

			var audioClip = AudioClip.Create(name, data.Length, channels, sampleRate, false);
			audioClip.SetData(data, 0);
			return audioClip;
		}

		#region wav file bytes to Unity AudioClip conversion methods

		static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			var wavSize = BitConverter.ToInt32(source, headerOffset);
			headerOffset += sizeof(int);
			Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

			var data = new float[wavSize];

			const sbyte maxValue = sbyte.MaxValue;
			var i = 0;
			while (i < wavSize) 
			{
				data [i] = (float)source [i] / maxValue;
				++i;
			}

			return data;
		}

		static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			var wavSize = BitConverter.ToInt32(source, headerOffset);
			headerOffset += sizeof(int);
			Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

			// block size = 2
			const int x = sizeof(short); 
			var convertedSize = wavSize / x;

			var data = new float[convertedSize];

			const short maxValue = short.MaxValue;
			var i = 0;
			while (i < convertedSize) 
			{
				var offset = i * x + headerOffset;
				data [i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
				++i;
			}

			Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);
			return data;
		}

		static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			var wavSize = BitConverter.ToInt32(source, headerOffset);
			headerOffset += sizeof(int);
			Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

			// block size = 3
			const int x = 3;
			var convertedSize = wavSize / x;

			const int maxValue = int.MaxValue;
			
			// using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset
			var block = new byte[sizeof(int)];
			var data = new float[convertedSize];

			var i = 0;
			while (i < convertedSize) 
			{
				var offset = i * x + headerOffset;
				Buffer.BlockCopy(source, offset, block, 1, x);
				data [i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
				++i;
			}

			Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);
			return data;
		}

		static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			var wavSize = BitConverter.ToInt32(source, headerOffset);
			headerOffset += sizeof(int);
			Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

			//  block size = 4
			const int x = sizeof(float); 
			var convertedSize = wavSize / x;

			const int maxValue = int.MaxValue;
			var data = new float[convertedSize];

			var i = 0;
			while (i < convertedSize) 
			{
				var offset = i * x + headerOffset;
				data [i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
				++i;
			}

			Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);
			return data;
		}

		#endregion

		public static byte[] FromAudioClip(AudioClip audioClip, string savePath)
		{
			var stream = new MemoryStream();

			const int headerSize = 44;
			
			// total file size = 44 bytes for header format and audioClip.samples * factor due to float to Int16 / sbyte conversion
			var fileSize = audioClip.samples * BlockSize(k_BitDepth) + headerSize;

			// chunk descriptor (riff)
			WriteFileHeader(ref stream, fileSize);
			// file header (fmt)
			WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, k_BitDepth);
			// data chunks (data)
			WriteFileData(ref stream, audioClip, k_BitDepth);

			var bytes = stream.ToArray();

			// Validate total bytes
			Debug.AssertFormat(bytes.Length == fileSize, "Unexpected AudioClip to wav format byte count. byte.length: {0} == fileSize: {1}", bytes.Length, fileSize);

			// Save file to persistant storage location
			Directory.CreateDirectory(Path.GetDirectoryName(savePath));
			File.WriteAllBytes(savePath, bytes);

			stream.Dispose();

			return bytes;
		}

		#region write .wav file functions

		static int WriteFileHeader(ref MemoryStream stream, int fileSize)
		{
			var count = 0;
			const int total = 12;

			// riff chunk id
			var riff = Encoding.ASCII.GetBytes("RIFF");
			count += WriteBytesToMemoryStream(ref stream, riff, "ID");

			// riff chunk size
			var chunkSize = fileSize - 8; // total size - 8 for the other two fields in the header
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes (chunkSize), "CHUNK_SIZE");

			var wave = Encoding.ASCII.GetBytes ("WAVE");
			count += WriteBytesToMemoryStream(ref stream, wave, "FORMAT");

			// Validate header
			Debug.AssertFormat(count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);

			return count;
		}

		static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, ushort bitDepth)
		{
			var count = 0;
			const int total = 24;

			var id = Encoding.ASCII.GetBytes("fmt ");
			count += WriteBytesToMemoryStream(ref stream, id, "FMT_ID");

			// 24 - 8
			const int subChunk1Size = 16;
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subChunk1Size), "SUBCHUNK_SIZE");

			const ushort audioFormat = 1;
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(audioFormat), "AUDIO_FORMAT");

			var numChannels = Convert.ToUInt16(channels);
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(numChannels), "CHANNELS");

			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate), "SAMPLE_RATE");

			var byteRate = sampleRate * channels * BytesPerSample(bitDepth);
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(byteRate), "BYTE_RATE");

			var blockAlign = Convert.ToUInt16(channels * BytesPerSample (bitDepth));
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(blockAlign), "BLOCK_ALIGN");

			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth), "BITS_PER_SAMPLE");

			// Validate format
			Debug.AssertFormat(count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);

			return count;
		}

		static int WriteFileData(ref MemoryStream stream, AudioClip audioClip, ushort bitDepth)
		{
			var count = 0;
			const int total = 8;
			
			var data = new float[audioClip.samples * audioClip.channels];
			audioClip.GetData(data, 0);
			data = ClippingAudioDataForExport(data);

			byte[] bytes;
			switch (bitDepth)
			{
				case 8:
					bytes = ConvertAudioClipDataTo8BitByteArray(data);
					break;
				case 16:
				default:
					bytes = ConvertAudioClipDataToInt16ByteArray(data);
					break;
			}

			var id = Encoding.ASCII.GetBytes("data");
			count += WriteBytesToMemoryStream(ref stream, id, "DATA_ID");

			var subChunkSize = audioClip.samples * BlockSize(bitDepth);
			count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes (subChunkSize), "SAMPLES");

			// Validate header
			Debug.AssertFormat(count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

			// Write bytes to stream
			count += WriteBytesToMemoryStream(ref stream, bytes, "DATA");

			// Validate audio data
			Debug.AssertFormat(bytes.Length == subChunkSize, "Unexpected AudioClip to wav subChunkSize size. bytes.Length: {0} == subChunkSize: {1}", bytes.Length, subChunkSize);

			return count;
		}
		
		static float[] ClippingAudioDataForExport(float[] buffer)
		{
			for (var i = 0; i < buffer.Length; ++i)
				buffer[i] *= 0.1f;
			return buffer;
		}
		
		static byte[] ConvertAudioClipDataTo8BitByteArray(float[] data)
		{
			var dataStream = new MemoryStream();
			
			const int sizeOfSByte = sizeof(sbyte);
			const sbyte maxValue = sbyte.MaxValue;
			
			var i = 0;
			while (i < data.Length)
			{
				var convertToSByte = Convert.ToSByte(data[i] * maxValue);
				var byteData = BitConverter.GetBytes(convertToSByte);
				dataStream.Write(byteData, 0, sizeOfSByte);
				++i;
			}
			var bytes = dataStream.ToArray();
			
			// Validate converted bytes
			Debug.AssertFormat(data.Length * sizeOfSByte == bytes.Length, "Unexpected float[] to sbyte to byte[] size: {0} == {1}", data.Length * sizeOfSByte, bytes.Length);
			
			dataStream.Dispose();
			return bytes;
		}		

		static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
		{
			var dataStream = new MemoryStream();
			
			const int sizeOfInt16 = sizeof(short);
			const short maxValue = short.MaxValue;

			var i = 0;
			while (i < data.Length)
			{
				var convertToInt16 = Convert.ToInt16(data[i] * maxValue);
				var byteData = BitConverter.GetBytes(convertToInt16);
				dataStream.Write(byteData, 0, sizeOfInt16);
				++i;
			}
			var bytes = dataStream.ToArray();

			// Validate converted bytes
			Debug.AssertFormat(data.Length * sizeOfInt16 == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * sizeOfInt16, bytes.Length);

			dataStream.Dispose();
			return bytes;
		}

		static int WriteBytesToMemoryStream (ref MemoryStream stream, byte[] bytes, string tag = "")
		{
			var count = bytes.Length;
			stream.Write (bytes, 0, count);
			return count;
		}

		#endregion

		/// <summary>
		/// Calculates the bit depth of an AudioClip
		/// </summary>
		/// <returns>The bit depth. Should be 8 or 16 or 32 bit.</returns>
		/// <param name="audioClip">Audio clip.</param>
		public static ushort BitDepth (AudioClip audioClip)
		{
			var bitDepth = Convert.ToUInt16 (audioClip.samples * audioClip.channels * audioClip.length / audioClip.frequency);
			Debug.AssertFormat (bitDepth == 8 || bitDepth == 16 || bitDepth == 32, "Unexpected AudioClip bit depth: {0}. Expected 8 or 16 or 32 bit.", bitDepth);
			return bitDepth;
		}

		static int BytesPerSample (ushort bitDepth)
		{
			return bitDepth / 8;
		}

		static int BlockSize (UInt16 bitDepth)
		{
			switch (bitDepth) 
			{
				case 32:
					return sizeof(int); // 32-bit -> 4 bytes (int)
				case 16:
					return sizeof(short); // 16-bit -> 2 bytes (short)
				case 8:
					return sizeof(sbyte); // 8-bit -> 1 byte (sbyte)
				default:
					throw new Exception (bitDepth + " bit depth is not supported.");
			}
		}

		static string FormatCode (ushort code)
		{
			switch (code) 
			{
				case 1:
					return "PCM";
				case 2:
					return "ADPCM";
				case 3:
					return "IEEE";
				case 7:
					return "μ-law";
				case 65534:
					return "WaveFormatExtensible";
				default:
					Debug.LogWarning ("Unknown wav code format:" + code);
					return "";
			}
		}

	}
}
