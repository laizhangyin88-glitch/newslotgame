using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public enum BeatValue
    {
        None,
        SixteenthBeat,
        SixteenthDottedBeat,
        EighthBeat,
        EighthDottedBeat,
        QuarterBeat,
        QuarterDottedBeat,
        HalfBeat,
        HalfDottedBeat,
        WholeBeat,
        WholeDottedBeat
    };

    [Flags]
    public enum BeatType
    {
        None,
        OffBeat  = 1 << 0,
        OnBeat   = 1 << 1,
        UpBeat   = 1 << 2,
        DownBeat = 1 << 3
    };

    public struct BeatDecimalValues
    {
        private static float dottedBeatModifier = 1.5f;
        private static float[] values = 
        {
            0f,
            4f, 4f / dottedBeatModifier, // SixteenthBeat, SixteenthDottedBeat
            2f, 2f / dottedBeatModifier, // EighthBeat, EighthDottedBeat
            1f, 1f / dottedBeatModifier, // QuarterBeat, QuarterDottedBeat
            0.5f, 0.5f / dottedBeatModifier, // HalfBeat, HalfDottedBeat
            0.25f, 0.25f / dottedBeatModifier // WholeBeat, WholeDottedBeat
        };

        public static float Get(BeatValue beatValue)
        {
            return values[(int)beatValue];
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName="New Metronome", menuName="SlotMaker2/Audio/Metronome")]
    public class Metronome : ScriptableObject, IPlayableObject
    {
        public float beatsPerMinute = 120f;
        public int audioFrequncy = 44100;
        public float beatScaler = 1f;

        [Serializable]
        public class BeatDefine
        {
            public BeatValue beatValue = BeatValue.QuarterBeat;
            public BeatValue beatOffset = BeatValue.None;
            public bool negativeBeatOffset = false;
            public BeatType beatType = BeatType.OnBeat;
        }
        public List<BeatDefine> beats;

        public event Action<int> onTrigger;

        private struct BeatSample
        {
            public float period;
            public float offset;
        }

        private float nextBeatSample;
        private int sequenceIndex;
        private BeatSample[] samples;

        public void Trigger()
        {
            Trigger((int)BeatType.OnBeat);
        }

        public void Trigger(int beatType)
        {
            if (onTrigger != null)
                onTrigger(beatType);
        }

        public void ResetMetronome()
        {
            samples = new BeatSample[beats.Count];

            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i].period = 60f / (beatsPerMinute * BeatDecimalValues.Get(beats[i].beatValue)) * audioFrequncy;

                if (beats[i].beatOffset != BeatValue.None)
                {
                    samples[i].offset = 60f / (beatsPerMinute * BeatDecimalValues.Get(beats[i].beatOffset)) * audioFrequncy;
                    if (beats[i].negativeBeatOffset)
                        samples[i].offset = samples[i].period - samples[i].offset;
                }

                samples[i].period *= beatScaler;
                samples[i].offset *= beatScaler;
            }

            nextBeatSample = 0f;
            sequenceIndex = 0;
        }

        public void StartPlayableObject()
        {
            ResetMetronome();

            nextBeatSample = (float)(AudioSettings.dspTime * audioFrequncy);
        }

        public void StopPlayableObject() {}

        public void UpdatePlayableObject()
        {
            float currentSample = (float)(AudioSettings.dspTime * audioFrequncy);

            if (currentSample >= nextBeatSample + samples[sequenceIndex].offset)
            {
                Trigger((int)beats[sequenceIndex].beatType);

                nextBeatSample += samples[sequenceIndex].period;
                sequenceIndex = (++sequenceIndex == samples.Length) ? 0 : sequenceIndex;
            }
        }
    }
}