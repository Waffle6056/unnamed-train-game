using Godot;
using System;

public partial class TestCart : TrainCart
{
	PackedScene AttackScene = GD.Load<PackedScene>("res://TrainCarts/TestCart/test_attack.tscn");
	Node3D Gun;
	public override void _Ready(){
		Gun = ((Node3D)FindChild("GunMesh")); 
	}
	public override void Activate(Vector3 Target, Modifer AttackModifer){
		Gun.Basis = Basis.LookingAt(Target-Gun.Position-Position);
		//GD.Print("kira kira koseki");
		Attack ts = (Attack) AttackScene.Instantiate();
		AttackModifer(ts,this);
		ts.Target = Target;
		ts.Basis = Basis.LookingAt(Target-Gun.Position-Position);
		ts.Position += Gun.Position;
		AddChild(ts);
		//GD.Print(ts.Damage);
	}
}
