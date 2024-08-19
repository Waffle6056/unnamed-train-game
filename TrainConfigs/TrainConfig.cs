using Godot;
using System;
using System.Collections.Generic;

public partial class TrainConfig : Resource
{
	[Export]
	public CartInstance[] TrainData;
	[Export]
	public float WeightCurrent = 0;
	[Export]
	public Moveset[] Movesets;
	public int MovesetsInd;
	[Export]
	public String Name;
	[Export]
	public float Fuel = 100.0f;
	//public int TrainDaa;
	//public Godot.Collections.Array<PackedScene> TrainDat;

}
