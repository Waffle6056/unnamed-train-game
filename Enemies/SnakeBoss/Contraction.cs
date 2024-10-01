using Godot;
using System;
[Tool]
public partial class Contraction : Resource
{
	[Export]
	public int Base = 1;
	[Export]
	public int End = 1;
	[Export]
	public Vector3 Axis;
	[Export]
	public float Duration;
	[Export]
	public float SnapRotation;
}
