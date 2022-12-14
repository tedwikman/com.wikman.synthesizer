using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Wikman.Synthesizer.Editor
{
    internal static class ResourceLoader
    {
        const string k_ResourcePath = "Packages/com.wikman.synthesizer/Editor/UI";

        internal static T Load<T>(string path) where T : Object
        {
            var assetPath = Path.Combine(k_ResourcePath, path);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return asset;
        }
    }

    internal static class EditorUtilities
    {
        static ObjectDestroyer s_ObjectDestroyer = new ObjectDestroyer();
        
        public static void PlayClip(AudioClip clip, float volume = 1f)
        {
            var gameObject = new GameObject("One shot audio");
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            gameObject.transform.position = Vector3.zero;
            var audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();

            s_ObjectDestroyer.DestroyObject(gameObject, clip.length + 0.5f);
        }

        public static void SaveClipToWav(AudioClip clip)
        {
            if (clip == null)
                return;

            var savePath = GetSavePath("Sounds");
            UnityWav.UnityWav.FromAudioClip(clip, savePath);
            Debug.Log($"Clip saved. Location: {savePath}");
            
            AssetDatabase.Refresh();
        }

        static string GetSavePath(string directoryName)
        {
            var fileName = $"{DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff")}.wav";
            return $"{Application.dataPath}/{directoryName}/{fileName}";            
        }
    }

    internal class ObjectDestroyer
    {
        struct Pair
        {
            public GameObject Obj;
            public float DestroyTime;
        }

        static float CurrentTime => Time.realtimeSinceStartup;
        
        Queue<Pair> m_ObjToDestroy = new Queue<Pair>();

        public ObjectDestroyer()
        {
            EditorApplication.update += Update;
        }
        
        ~ObjectDestroyer()
        {
            EditorApplication.update -= Update;
        }

        public void DestroyObject(GameObject go, float waitTime)
        {
            m_ObjToDestroy.Enqueue(new Pair()
            {
                Obj = go,
                DestroyTime = CurrentTime + waitTime
            });
        }

        void Update()
        {
            if (m_ObjToDestroy.Count == 0)
                return;
            if (CurrentTime < m_ObjToDestroy.Peek().DestroyTime)
                return;

            do
            {
                var pair = m_ObjToDestroy.Dequeue();
                
                if(Application.isPlaying)
                    Object.Destroy(pair.Obj);
                else
                    Object.DestroyImmediate(pair.Obj);
                
            } while (m_ObjToDestroy.Count != 0 && CurrentTime > m_ObjToDestroy.Peek().DestroyTime);
        }
    }
}