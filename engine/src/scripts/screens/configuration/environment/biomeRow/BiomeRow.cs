using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public partial class BiomeRow : VBoxContainer
{
    // [Export] 
    private readonly List<List<int>> DefaultBiomes;

    public event Action Pressed;
    // this.Pressed?.Invoke();

    public event EventHandler<BaseEventArgs<BiomeType[][]>> Edited;

    private List<List<BiomeType>> biomes = new();

    public override void _Ready()
    {
        this.LoadBiomes();
    }

    private void LoadBiomes()
    {
        this.biomes = this.DefaultBiomes.Select(
            innerList => innerList.Select(
                biomeIndex => (BiomeType)Enum.ToObject(typeof(BiomeType), biomeIndex)
            ).ToList()
        ).ToList();
    }

}