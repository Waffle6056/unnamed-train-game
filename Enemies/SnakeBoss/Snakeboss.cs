using Godot;
using System;

public partial class Snakeboss : Skeleton3D
{
	[Export]
	public Contraction[] CurrentContractions;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public void ForwardContract(float delta, Contraction c){
		Quaternion rot = new Quaternion(c.Axis, c.Rotation * delta);
		for (int bone = c.Base; bone <= c.End; bone++){
			SetBonePoseRotation(bone, GetBonePoseRotation(bone) * rot);
		}
	}
	public void BackwardContract(float delta, Contraction c){
		Quaternion rot = new Quaternion(c.Axis, c.Rotation * delta);
		Quaternion nrot = new Quaternion(c.Axis, -c.Rotation * delta);
		Vector3 pos = GetBoneGlobalPose(c.Base).Origin;
		for (int bone = c.Base; bone >= c.End; bone--){
			SetBonePoseRotation(bone, GetBonePoseRotation(bone) * nrot);
			SetBonePoseRotation(c.End, GetBonePoseRotation(c.End) * rot);
		}
		Position += pos - GetBoneGlobalPose(c.Base).Origin;
		//GD.Print("not implemented");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float d = (float) delta;
		foreach (Contraction c in CurrentContractions){
			if (c.Base < c.End)
				ForwardContract(d, c);
			else if (c.Base > c.End)
				BackwardContract(d, c);
		}
	}
}
