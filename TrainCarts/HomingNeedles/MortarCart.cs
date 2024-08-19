using Godot;
using System;

public partial class MortarCart : TrainCart
{
	[Export]
	public AudioStreamPlayer3D audiotest;
	PackedScene AttackScene = GD.Load<PackedScene>("res://TrainCarts/Mortar/MortarShell.tscn");
	Node3D Gun;
	public override void _Ready(){
		Gun = ((Node3D)FindChild("GunMesh")); 
	}
	public override void Activate(Vector3 Target, Modifer AttackModifer){
		audiotest.Play();
		//GD.Print("kira kira koseki");
		MortarShell Shell = (MortarShell) AttackScene.Instantiate();
		
		AttackModifer(Shell,this);
		Shell.Target = Target;
		Shell.Position = Gun.Position+GlobalPosition;
		float x = new Vector2(Shell.Position.X,Shell.Position.Z).DistanceTo(new Vector2(Target.X,Target.Z));
		float y = Shell.Position.Y - Target.Y;
		float g = -10;
		float v = Shell.BaseSpeed * Shell.SpeedModifer;
		//GD.Print(x+" "+y+" "+Target.X);
		float theta = Mathf.Atan((v*v+Mathf.Sqrt(v*v*v*v - g*(g*x*x + 2*y*v*v)))/(g*x));
		if (theta != theta)
			return;
		Gun.Basis = Basis.LookingAt(Target-Shell.Position);
		Gun.Rotation = new Vector3(-theta,Gun.Rotation.Y,Gun.Rotation.X);
		
		Shell.Rotation = new Vector3(0,Gun.Rotation.Y,Gun.Rotation.X);
		
		theta = Mathf.Abs(theta);
		Shell.VertVelo = Mathf.Sin(theta) * v;
		Shell.HoriVelo = Mathf.Cos(theta) * v;
		//GD.Print(shell.VertVelo+" "+shell.HoriVelo);
		GetTree().Root.AddChild(Shell);
		//GD.Print(ts.Damage);
		 return;
	}
	public override void PoweredOn(){Powered = true; Head.AttackModifer += SlowBomb;}
	public override void PoweredOff(){Powered = false; Head.AttackModifer -= SlowBomb;}
	
	public void SlowBomb(Attack A, TrainCart other){
		if (CartPosition <= other.CartPosition){
			A.SpeedModifer -= 0.25f;
			A.SizeModifer += 0.5f;
		}
	}

}
