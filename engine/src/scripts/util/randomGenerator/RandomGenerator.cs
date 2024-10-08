﻿
using System;
using System.Linq;

public static class RandomGenerator
{
    private static Random RandomGen = new Random();

    public static int Int(int min, int max)
    {
        return RandomGen.Next(min, max + 1);
    }

    public static int Int(int max)
    {
        return RandomGen.Next(max + 1);
    }

    public static int Int()
    {
        return RandomGen.Next();
    }

    public static float Float(float min, float max)
    {
        double randomValue = RandomGen.NextDouble();
        return (float)(min + (randomValue * (max - min)));
    }

    public static float Float(float max)
    {
        double randomValue = RandomGen.NextDouble();
        return (float)(randomValue * max);
    }

    public static float Float()
    {
        return (float)RandomGen.NextDouble();
    }

    public static bool Occurs(float probability)
    {
        if (probability < 0.0f || probability > 1.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0.0 and 1.0.");
        }
        return RandomGen.NextDouble() < probability;
    }

    public static bool Occurs(int probability)
    {
        if (probability < 0 || probability > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 100.");
        }
        return RandomGen.Next(0, 101) < probability;
    }

    public static bool OccursPermille(int probability)
    {
        if (probability < 0 || probability > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1000.");
        }

        return RandomGen.Next(0, 1001) < probability;
    }

    public static bool OccursOnceIn(int times)
    {
        if (times <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(times), "Times must be greater than 0.");
        }
        return RandomGen.Next(0, times) == 0;
    }

    public static int Index(int[] weights)
    {
        if (weights.Length == 0)
        {
            throw new ArgumentException("Weights array must not be empty.");
        }

        int weightsSum = weights.Sum();
        if (weightsSum <= 0)
        {
            throw new ArgumentException("Sum of weights must be greater than 0.");
        }

        int randomValue = RandomGen.Next(0, weightsSum);
        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if (randomValue < sum)
            {
                return i;
            }
        }
        throw new InvalidOperationException("Failed to select an index based on weights.");
    }

    public static void SetSeed(int seed)
    {
        RandomGen = new Random(seed);
    }
}