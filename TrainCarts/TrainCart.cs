using Godot;
using System;

public partial class TrainCart : AnimatableBody3D
{
	[Export]
	public float Weight = 0;
	[Export]
	public float HeatLimit = 0;
	[Export]
	public Texture2D Icon;
	public float HeatCurrent = 0;
	
	[Export]
	public float CartLength = 12.15f;
	public int CartPosition;
	
	public TrainHead Head;
	public virtual void Activate(Vector3 Target, Modifer AttackModifer){ GD.Print(Name +" Activate not binded"); return;}
	public delegate void Modifer(Attack A, TrainCart other);
	
	public bool Powered = false;
	public virtual void PoweredOn(){Powered = true;}
	public virtual void PoweredOff(){Powered = false;}
	//public virtual Vector3 CameraOffset{get;} = new Vector3(0.003f, 3.59f, -0.128f);
}
