using Godot;
using System;

public partial class Spitter : Enemy{
	public PackedScene BulletScene = GD.Load<PackedScene>("res://Enemies/Spitter/Spit.tscn");
	public override float Speed {get; set;} = 10f;
	//float time
	bool Shooting = false;
	public async void Shoot(){
		if (Shooting)
			return;
		Shooting = true;
		Node3D bullet = (Node3D)BulletScene.Instantiate();
		bullet.Transform = Transform;
		GetTree().Root.AddChild(bullet);
		
		await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
		
		bullet = (Node3D)BulletScene.Instantiate();
		bullet.Transform = Transform;
		GetTree().Root.AddChild(bullet);
		
		await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
		
		bullet = (Node3D)BulletScene.Instantiate();
		bullet.Transform = Transform;
		GetTree().Root.AddChild(bullet);
		
		await ToSignal(GetTree().CreateTimer(3f),"timeout");
		Shooting = false;
	}
	double time = 0;
	public override void Behavior(double delta){
		Target = ((player)GetNode($"/root/Main/Player")).Target;
		Basis = Basis.LookingAt(Target - Position);
		if (Position.DistanceTo(Target) <= 15)
			Velocity = Basis[2].Normalized();
		else if (Position.DistanceTo(Target) >= 20)
			Velocity = -Basis[2].Normalized();
		Velocity += Basis[0].Normalized() * (float)Mathf.Sin(time+=delta);
		Velocity = Velocity.Normalized() * Speed * SpeedModifer;
		//GD.Print((Mathf.Sin(time)));
		MoveAndSlide();
		
		if (Position.DistanceTo(Target) <= 30)
			Shoot();
	}
}
