using Godot;
using System;

public partial class ShotgunCart : TrainCart
{
	[Export]
	public AudioStreamPlayer3D audiotest;
	PackedScene AttackScene = GD.Load<PackedScene>("res://TrainCarts/Shotgun/ShotgunShell.tscn");
	Node3D Gun;
	public override void _Ready(){
		Gun = ((Node3D)FindChild("GunMesh")); 
	}
	public override void Activate(Vector3 Target, Modifer AttackModifer){
		audiotest.Play();
		Gun.Basis = Basis.LookingAt(Target-Gun.GlobalPosition);
		Gun.Rotation -= GlobalRotation;
		//GD.Print(Target+" "+Gun.Position+" "+GlobalPosition);
		for (int i = 0; i < 20; i++){
			Attack ts = (Attack) AttackScene.Instantiate();
			AttackModifer(ts,this);
			ts.Target = Target;
			ts.Basis = Basis.LookingAt(Target-Gun.GlobalPosition);
			ts.Rotation += new Vector3(GD.Randf()-.5f,GD.Randf()-.5f,GD.Randf()-.5f);
			ts.Position = Gun.GlobalPosition;
		GetTree().Root.AddChild(ts);
		}
		//GD.Print(ts.Damage);
	}
}
