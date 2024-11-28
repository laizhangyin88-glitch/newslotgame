using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.Internal
{

public class BagelCodeReliabilitySystem
{
    private const uint maxSequence = 0xFFFFFFFF;
    private uint sequence = 0;
    private List<uint> ackQueue = new List<uint>();

    public uint Sequence { get { return sequence; } }

    public void Reset()
    {
        sequence = 0;
        ackQueue.Clear();
    }

    public uint FetchBlockSeq()
    {
        if (sequence == 0)
        {
            return sequence = (uint)UnityEngine.Random.Range(1, 10000 + 1);
        }
        else
        {
            return ++sequence;
        }
    }

    public void ProcessAck(uint ack)
    {
        int bitIndex = BitIndexForSequence(sequence, ack);
        if (bitIndex > 31)
        {
            Debug.LogWarning(string.Format("[BagelCodeHTTP] Too old message is received. - message seq({0}), current seq({1})", ack, sequence));
            return;
        }

        InsertAck(ack);
    }

    public int GenerateAckBits()
    {
        int ackBits = 0;
        for (int i = 0; i < ackQueue.Count; ++i)
        {
            int bitIndex = BitIndexForSequence(sequence, ackQueue[i]) - 1;
            if (bitIndex < 32)
                ackBits |= 1 << bitIndex;
        }

        if (SlotMaker.ApplicationSettings.LogNetwork())
            Debug.Log(string.Format("[BagelCodeHTTP] Seq: {0}, AckBits: {1}", sequence, Convert.ToString(ackBits, 2)));

        return ackBits;
    }

    private bool IsMoreRecentSequence(uint squence1, uint sequence2)
    {
        uint halfMax = maxSequence / 2;
        return ((squence1 > sequence2) && (squence1 - sequence2 <= halfMax)) ||
            ((sequence2 > squence1) && (sequence2 - squence1 > halfMax));
    }

    private static int BitIndexForSequence(uint seq, uint ack)
    {
        return (int)(seq - ack);
    }

    private void InsertAck(uint ack)
    {
        if (ackQueue.Count == 0)
        {
            ackQueue.Add(ack);
        }
        else
        {
            if (!IsMoreRecentSequence(ack, ackQueue[0]))
            {
                ackQueue.Insert(0, ack);
            }
            else if (IsMoreRecentSequence(ack, ackQueue[ackQueue.Count - 1]))
            {
                ackQueue.Add(ack);
            }
            else
            {
                for (int i = 1; i < ackQueue.Count; ++i)
                {
                    if (ackQueue[i] == ack)
                    {
                        Debug.LogWarning(string.Format("[BagelCodeHTTP] Already received message. - blockseq({0})", ack));
                        return;
                    }
                    else if (IsMoreRecentSequence(ackQueue[i], ack))
                    {
                        ackQueue.Insert(i, ack);
                        break;
                    }
                }
            }
        }

        while (ackQueue.Count > 32)
            ackQueue.RemoveAt(0);
    }
}

}
