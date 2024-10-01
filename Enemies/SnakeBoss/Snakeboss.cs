using Godot;
using System;
[Tool]
public partial class Snakeboss : Skeleton3D
{
	[Export]
	public bool EditorOn = false;
	[Export]
	public bool PoseSnap = false;
	[Export]
	public AnimationPlayer Anims;
	[Export]
	public String Anim;
	[Export]
	public Contraction[] CurrentContractions;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("test");
		Anims.Play(Anim);
		GD.Print(Anims.IsPlaying());
	}
	
	public void ForwardContract(float delta, Contraction c){
		float rad = c.SnapRotation / c.Duration * delta;
		if (PoseSnap)
			rad = c.SnapRotation;
		Quaternion rot = new Quaternion(c.Axis, rad);
		
		for (int bone = c.Base; bone <= c.End; bone++){
			SetBonePoseRotation(bone, GetBonePoseRotation(bone) * rot);
		}
	}
	public void BackwardContract(float delta, Contraction c){
		float rad = c.SnapRotation / c.Duration * delta;
		if (PoseSnap)
			rad = c.SnapRotation;
		Quaternion rot = new Quaternion(c.Axis, rad);
		Quaternion nrot = new Quaternion(c.Axis, -rad);
		Vector3 pos = GetBoneGlobalPose(c.Base).Origin;
		for (int bone = c.Base; bone >= c.End; bone--){
			SetBonePoseRotation(bone, GetBonePoseRotation(bone) * nrot);
			SetBonePoseRotation(1, GetBonePoseRotation(1) * rot);
		}
		Position += pos - GetBoneGlobalPose(c.Base).Origin;
		//GD.Print("not implemented");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Rotation += new Vector3(1,1,1) * (float) delta;
		//GD.Print(Rotation);
		if (PoseSnap){
			Position = Vector3.Zero;
			ResetBonePoses();
		}
		if (Engine.IsEditorHint() && !EditorOn){
			//Position = Vector3.Zero;
			//ResetBonePoses();
			return;
		}
		
		//GD.Print(Anims.CurrentAnimation);
		float d = (float) delta;
		foreach (Contraction c in CurrentContractions){
			if (c.Base < c.End)
				ForwardContract(d, c);
			else if (c.Base > c.End)
				BackwardContract(d, c);
			else if (c.Axis.LengthSquared() < 0.1)
				ResetBonePoses();
				
		}
	}
}
