using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PriorityQueue<T>
{
    int maxRandomVariation;

    public PriorityQueue(int maxRandomVariation)
    {
        this.maxRandomVariation = maxRandomVariation;
    }


    internal class PQNode : IComparable<PQNode>
    {
        public int Priority;
        public T O;
        public int CompareTo(PQNode other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }

    private MinHeap<PQNode> minHeap = new MinHeap<PQNode>();

    public void Add(int priority, T element)
    {
        int fixedPriority = priority + Mathf.FloorToInt(Random.value * maxRandomVariation * 0.99f);
        minHeap.Add(new PQNode() { Priority = fixedPriority, O = element });
    }

    public T RemoveMin()
    {
        return minHeap.RemoveMin().O;
    }

    public T Peek()
    {
        return minHeap.Peek().O;
    }

    public int PeekPriority()
    {
        return minHeap.Peek().Priority;
    }

    public int Count
    {
        get
        {
            return minHeap.Count;
        }
    }
}

