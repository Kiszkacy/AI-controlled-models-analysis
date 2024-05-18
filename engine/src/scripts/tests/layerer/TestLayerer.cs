
using System.Collections.Generic;
using System;

using System.Reflection;

public  class TestLayerer
{
    private readonly List<Type> queue = new();
    private readonly List<List<Type>> layers = new() { new List<Type>() }; // one default layer
    private int layerZero = 0;
    private readonly Dictionary<Type, List<Type>> mustBeBefore = new();

    public List<List<Type>> GetTestLayers(List<Type> tests)
    {
        this.GetBeforeDictionary(tests);

        foreach (Type test in tests)
        {
            bool added = this.TryAddingToLayers(test);
            if (added) this.UpdateQueue();
        }

        return layers;
    }

    private void AddToLayer(Type test, int index)
    {
        if (index == -1) // new head layer
        {
            this.layers.Insert(0, new List<Type>());
            index += 1;
            this.layerZero += 1;
        } 
        else if (index == this.layers.Count) // new tail layer
        {
            this.layers.Add(new List<Type>());
        }
        
        this.layers[index].Add(test);
    }
    
    private bool TryAddingToLayers(Type test)
    {
        int? targetLayerIndex = this.GetTargetLayerIndex(test);

        if (targetLayerIndex.HasValue)
        {
            this.AddToLayer(test, targetLayerIndex.Value);
            return true;
        }
        
        return false;
    }

    private int? GetTargetLayerIndex(Type test)
    {
        // does not require any tests before running itself
        if (this.mustBeBefore[test].Count == 0)
        {
            return this.layerZero;
        }
        // does require some tests
        int targetLayer = this.layerZero;
        foreach (Type requiredType in this.mustBeBefore[test])
        {
            int requiredTypeLayer = this.layers.FindIndex(layer => layer.Contains(requiredType));
            if (requiredTypeLayer == -1) // not found => test not present in layers so add to queue
            {
                this.queue.Add(test);
                return null;
            }

            targetLayer = requiredTypeLayer - 1 < targetLayer ? requiredTypeLayer - 1 : targetLayer;
        }

        return targetLayer;
    }
    
    private void UpdateQueue()
    {
        List<Type> copy = new List<Type>(this.queue);
        
        foreach (Type test in copy)
        {
            int? targetLayerIndex = this.GetTargetLayerIndex(test);
            if (targetLayerIndex.HasValue)
            {
                this.AddToLayer(test, targetLayerIndex.Value);
                this.queue.Remove(test);
            }
        }
    }

    private void GetBeforeDictionary(List<Type> tests)
    {
        tests.ForEach(test => mustBeBefore[test] = new List<Type>());

        foreach (Type test in tests)
        {
            TestAfterAttribute after = test.GetCustomAttribute<TestAfterAttribute>();
            if (after != null)
                foreach (string name in after.After) this.mustBeBefore[Type.GetType($"{name}Test")!].Add(test);

            TestBeforeAttribute before = test.GetCustomAttribute<TestBeforeAttribute>();
            if (before != null)
                foreach (string name in before.Before) this.mustBeBefore[test].Add(Type.GetType($"{name}Test")!);
        }
    }
}