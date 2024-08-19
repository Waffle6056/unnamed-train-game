using Godot;
using System;

public partial class EnergyBeamCart : TrainCart
{
	[Export]
	public AudioStreamPlayer3D audiotest;
	PackedScene AttackScene = GD.Load<PackedScene>("res://TrainCarts/EnergyBeam/EnergyBeam.tscn");
	Node3D Gun;
	public override void _Ready(){
		Gun = ((Node3D)FindChild("GunMesh")); 
	}
	public override void Activate(Vector3 Target, Modifer AttackModifer){
		audiotest.Play();
		Gun.Basis = Basis.LookingAt(Target-Gun.GlobalPosition);
		Gun.Rotation -= GlobalRotation;
		//GD.Print("kira kira koseki");
		EnergyBeam ts = (EnergyBeam) AttackScene.Instantiate();
		AttackModifer(ts,this);
		ts.Target = Target;
		ts.Basis = Basis.LookingAt(Target-Gun.GlobalPosition);
		ts.Position = Gun.GlobalPosition;
		GetTree().Root.AddChild(ts);
		//GD.Print(ts.Damage);
	}
}
