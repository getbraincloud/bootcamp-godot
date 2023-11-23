// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;

public class ObjectPool<Type> where Type : BaseNode
{
    private List<Type> pool;

    public ObjectPool(PackedScene prefab, Node2D parent, int poolSize)
    {
        pool = new List<Type>();
        Type temp;
        for (int i = 0; i < poolSize; i++)
        {   
            temp = prefab.Instantiate<Type>();
            temp.Name = $"{temp.Name}_{i}";
            parent.AddChild(temp);

            temp.Disable();
            pool.Add(temp);
        }
    }

    public Type GetNodeFromPool()
    {
        for(int i = 0; i < pool.Count; i++)
            if(!pool[i].IsEnabled())
                return pool[i];

        return null;
    }

    public void DisableAll()
    {
        for (int i = 0; i < pool.Count; i++)
            pool[i].Disable();
    }

    public List<Type> GetActiveNodes()
    {
        List<Type> activeNodes = new List<Type>();

        for (int i = 0; i < pool.Count; i++)
            if (pool[i].IsEnabled())
                activeNodes.Add(pool[i]);

        return activeNodes;
    }
}
