using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Wikman.Synthesizer.Editor
{
    public class SynthEditor : EditorWindow
    {
        static readonly List<string> k_WaveTypes = new List<string>
        {
            "Square",
            "Sawtooth",
            "Sine",
            "Noise",
            "Triangle",
            "PinkNoise",
            "Tan",
            "Whistle",
            "Breaker",
        };
        
        ClipData m_LatestClip;
        bool m_ShouldRandomize = false;
        int m_WaveTypeIndex;
        DropdownField m_WaveTypeDropdown;
        Slider m_AttackTimeSlider;
        Slider m_SustainTimeSlider;
        Slider m_SustainPunchSlider;
        Slider m_DecayTimeSlider;
        Slider m_StartFrequencySlider;
        Slider m_MinFrequencySlider;
        Slider m_SlideSlider;
        Slider m_DeltaSlideSlider;
        Slider m_VibratoDepthSlider;
        Slider m_VibratoSpeedSlider;
        Slider m_ChangeAmountSlider;
        Slider m_ChangeSpeedSlider;
        Slider m_SquareDutySlider;
        Slider m_DutySweepSlider;
        Slider m_RepeatSpeedSlider;
        Slider m_FlangerOffsetSlider;
        Slider m_FlangerSweepSlider;
        Slider m_LpFilterCutoffSlider;
        Slider m_LpFilterCutoffSweepSlider;
        Slider m_LpFilterResonanceSlider;
        Slider m_HpFilterCutoffSlider;
        Slider m_HpFilterCutoffSweepSlider;
        
        [MenuItem("Window/Audio/Synthesizer")]
        static void Init()
        {
            var window = GetWindow(typeof(SynthEditor));
            window.minSize = new Vector2(900, 620);
            window.titleContent = new GUIContent("Synthesizer");
            window.Show();
        }

        void CreateGUI()
        {
            LoadUxml();
            HookUpUI();
        }

        void LoadUxml()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SynthWindow.uxml");
            var labelFromUxml = visualTree.Instantiate();
            rootVisualElement.Add(labelFromUxml);

            var sheet = ResourceLoader.Load<StyleSheet>("WindowStyle.uss");
            rootVisualElement.styleSheets.Add(sheet);
        }

        void HookUpUI()
        {
            LeftSideUI();
            MiddleUI();
        }

        void LeftSideUI()
        {
            var btn = rootVisualElement.Q<Button>("Pickup");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.Pickup);
                    PlayClip(m_LatestClip);
                };   
            
            btn = rootVisualElement.Q<Button>("Laser");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.Laser);
                    PlayClip(m_LatestClip);
                }; 
            
            btn = rootVisualElement.Q<Button>("Explosion");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.Explosion);
                    PlayClip(m_LatestClip);
                };          
            
            btn = rootVisualElement.Q<Button>("Power up");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.PowerUp);
                    PlayClip(m_LatestClip);
                };       
            
            btn = rootVisualElement.Q<Button>("Hit");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.Hit);
                    PlayClip(m_LatestClip);
                };  
            
            btn = rootVisualElement.Q<Button>("Jump");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.Jump);
                    PlayClip(m_LatestClip);
                };             
            
            btn = rootVisualElement.Q<Button>("Blip");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.Blip);
                    PlayClip(m_LatestClip);
                };             
            
            btn = rootVisualElement.Q<Button>("Random");
            btn.clicked += () =>
                {
                    GenerateClip(EffectType.None);
                    PlayClip(m_LatestClip);
                };

            btn = rootVisualElement.Q<Button>("PlaySound");
            btn.clicked += () =>
                {
                    ModifyClip();
                    PlayClip(m_LatestClip);
                };
            
            btn = rootVisualElement.Q<Button>("PrintToConsole");
            btn.clicked += () =>
                {
                    if(m_LatestClip != null)
                        Debug.Log(m_LatestClip.parameters.ToString());
                };            
            
            btn = rootVisualElement.Q<Button>("ExportSound");
            btn.clicked += () =>
                {
                    if(m_LatestClip != null)
                        EditorUtilities.SaveClipToWav(m_LatestClip.clip);
                };

            var toggle = rootVisualElement.Q<Toggle>("Randomize");
            m_ShouldRandomize = toggle.value;
            toggle.RegisterValueChangedCallback((x) =>
            {
                m_ShouldRandomize = x.newValue;
            });
        }

        void MiddleUI()
        {
            m_WaveTypeDropdown = rootVisualElement.Q<DropdownField>("WaveType");
            m_WaveTypeDropdown.choices = k_WaveTypes;
            m_WaveTypeDropdown.index = 0;
            m_WaveTypeDropdown.RegisterValueChangedCallback((x) =>
            {
                m_WaveTypeIndex = m_WaveTypeDropdown.index;
            });
            
            m_AttackTimeSlider = rootVisualElement.Q<Slider>("AttackTime");
            m_SustainTimeSlider = rootVisualElement.Q<Slider>("SustainTime");
            m_SustainPunchSlider = rootVisualElement.Q<Slider>("SustainPunch");
            m_DecayTimeSlider = rootVisualElement.Q<Slider>("DecayTime");
            m_StartFrequencySlider = rootVisualElement.Q<Slider>("StartFrequency");
            m_MinFrequencySlider = rootVisualElement.Q<Slider>("MinFrequency");
            m_SlideSlider = rootVisualElement.Q<Slider>("Slide");
            m_DeltaSlideSlider = rootVisualElement.Q<Slider>("DeltaSlide");
            m_VibratoDepthSlider = rootVisualElement.Q<Slider>("VibratoDepth");
            m_VibratoSpeedSlider = rootVisualElement.Q<Slider>("VibratoSpeed");
            m_ChangeAmountSlider = rootVisualElement.Q<Slider>("ChangeAmount");
            m_ChangeSpeedSlider = rootVisualElement.Q<Slider>("ChangeSpeed");
            m_SquareDutySlider = rootVisualElement.Q<Slider>("SquareDuty");
            m_DutySweepSlider = rootVisualElement.Q<Slider>("DutySweep");
            m_RepeatSpeedSlider = rootVisualElement.Q<Slider>("RepeatSpeed");
            m_FlangerOffsetSlider = rootVisualElement.Q<Slider>("FlangerOffset");
            m_FlangerSweepSlider = rootVisualElement.Q<Slider>("FlangerSweep");
            m_LpFilterCutoffSlider = rootVisualElement.Q<Slider>("LpFilterCutoff");
            m_LpFilterCutoffSweepSlider = rootVisualElement.Q<Slider>("LpFilterCutoffSweep");
            m_LpFilterResonanceSlider = rootVisualElement.Q<Slider>("LpFilterResonance");
            m_HpFilterCutoffSlider = rootVisualElement.Q<Slider>("HpFilterCutoff");
            m_HpFilterCutoffSweepSlider = rootVisualElement.Q<Slider>("HpFilterCutoffSweep");          
        }

        void GenerateClip(EffectType effectType)
        {
            if (m_ShouldRandomize)
            {
                if(effectType != EffectType.None)
                    m_LatestClip = Synth.GenerateRandom(effectType);
                else
                    m_LatestClip = Synth.GenerateRandom();                
            }
            else
            {
                EffectParameters fxParams = null;
                switch (effectType)
                {
                    case EffectType.Pickup:
                        fxParams = TemplateDefaults.Pickup;
                        break;
                    case EffectType.Laser:
                        fxParams = TemplateDefaults.Laser;
                        break;
                    case EffectType.Explosion:
                        fxParams = TemplateDefaults.Explosion;
                        break;
                    case EffectType.PowerUp:
                        fxParams = TemplateDefaults.PowerUp;
                        break;
                    case EffectType.Hit:
                        fxParams = TemplateDefaults.Hit;
                        break;
                    case EffectType.Jump:
                        fxParams = TemplateDefaults.Jump;
                        break;
                    case EffectType.Blip:
                        fxParams = TemplateDefaults.Blip;
                        break;
                    case EffectType.None:
                    default:
                        fxParams = TemplateDefaults.FullSpectrum;
                        break;
                }
                m_LatestClip = Synth.Generate(fxParams);
            }

            UpdateSliders();
        }

        void UpdateSliders()
        {
            m_WaveTypeIndex = m_LatestClip.parameters.waveType;
            m_WaveTypeDropdown.index = WaveTypeToIndex(m_WaveTypeIndex);

            m_AttackTimeSlider.value = m_LatestClip.parameters.attackTime;
            m_SustainTimeSlider.value = m_LatestClip.parameters.sustainTime;
            m_SustainPunchSlider.value = m_LatestClip.parameters.sustainPunch;
            m_DecayTimeSlider.value = m_LatestClip.parameters.decayTime;
            m_StartFrequencySlider.value = m_LatestClip.parameters.startFrequency;
            m_MinFrequencySlider.value = m_LatestClip.parameters.minFrequency;
            m_SlideSlider.value = m_LatestClip.parameters.slide;
            m_DeltaSlideSlider.value = m_LatestClip.parameters.deltaSlide;
            m_VibratoDepthSlider.value = m_LatestClip.parameters.vibratoDepth;
            m_VibratoSpeedSlider.value = m_LatestClip.parameters.vibratoSpeed;
            m_ChangeAmountSlider.value = m_LatestClip.parameters.changeAmount;
            m_ChangeSpeedSlider.value = m_LatestClip.parameters.changeSpeed;
            m_SquareDutySlider.value = m_LatestClip.parameters.squareDuty;
            m_DutySweepSlider.value = m_LatestClip.parameters.dutySweep;
            m_RepeatSpeedSlider.value = m_LatestClip.parameters.repeatSpeed;
            m_FlangerOffsetSlider.value = m_LatestClip.parameters.flangerOffset;
            m_FlangerSweepSlider.value = m_LatestClip.parameters.flangerSweep;
            m_LpFilterCutoffSlider.value = m_LatestClip.parameters.lpFilterCutoff;
            m_LpFilterCutoffSweepSlider.value = m_LatestClip.parameters.lpFilterCutoffSweep;
            m_LpFilterResonanceSlider.value = m_LatestClip.parameters.lpFilterResonance;
            m_HpFilterCutoffSlider.value = m_LatestClip.parameters.hpFilterCutoff;
            m_HpFilterCutoffSweepSlider.value = m_LatestClip.parameters.hpFilterCutoffSweep;
        }

        void ModifyClip()
        {
            if (m_LatestClip == null)
                return;

            m_LatestClip.parameters.waveType = m_WaveTypeIndex;
            m_LatestClip.parameters.attackTime = m_AttackTimeSlider.value;
            m_LatestClip.parameters.sustainTime = m_SustainTimeSlider.value;
            m_LatestClip.parameters.sustainPunch = m_SustainPunchSlider.value;
            m_LatestClip.parameters.decayTime = m_DecayTimeSlider.value;
            m_LatestClip.parameters.startFrequency = m_StartFrequencySlider.value;
            m_LatestClip.parameters.minFrequency = m_MinFrequencySlider.value;
            m_LatestClip.parameters.slide = m_SlideSlider.value;
            m_LatestClip.parameters.deltaSlide = m_DeltaSlideSlider.value;
            m_LatestClip.parameters.vibratoDepth = m_VibratoDepthSlider.value;
            m_LatestClip.parameters.vibratoSpeed = m_VibratoSpeedSlider.value;
            m_LatestClip.parameters.changeAmount = m_ChangeAmountSlider.value;
            m_LatestClip.parameters.changeSpeed = m_ChangeSpeedSlider.value;
            m_LatestClip.parameters.squareDuty = m_SquareDutySlider.value;
            m_LatestClip.parameters.dutySweep = m_DutySweepSlider.value;
            m_LatestClip.parameters.repeatSpeed = m_RepeatSpeedSlider.value;
            m_LatestClip.parameters.flangerOffset = m_FlangerOffsetSlider.value;
            m_LatestClip.parameters.flangerSweep = m_FlangerSweepSlider.value;
            m_LatestClip.parameters.lpFilterCutoff = m_LpFilterCutoffSlider.value;
            m_LatestClip.parameters.lpFilterCutoffSweep = m_LpFilterCutoffSweepSlider.value;
            m_LatestClip.parameters.lpFilterResonance = m_LpFilterResonanceSlider.value;
            m_LatestClip.parameters.hpFilterCutoff = m_HpFilterCutoffSlider.value;
            m_LatestClip.parameters.hpFilterCutoffSweep = m_HpFilterCutoffSweepSlider.value;
            
            m_LatestClip = Synth.GenerateRandom(m_LatestClip.fxType, m_LatestClip.seed, m_LatestClip.parameters);
        }

        static void PlayClip(ClipData clipData)
        {
            if (clipData == null || clipData.clip == null)
                return;
            EditorUtilities.PlayClip(clipData.clip);
        }

        static int WaveTypeToIndex(int waveType)
        {
            if (waveType > 8)
                waveType = 8;
            return waveType;
        }
    }
}
