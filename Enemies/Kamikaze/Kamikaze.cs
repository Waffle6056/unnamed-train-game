using Godot;
using System;

public partial class Kamikaze : Enemy{
	public PackedScene ExplosionScene = GD.Load<PackedScene>("res://Enemies/Kamikaze/KamikazeExplosion.tscn");
	public override float Speed {get; set;} = 5f;
	public bool Exploded = false;
	public async void Explode(){
		Exploded = true;
		Scale *= 20f;
		Area3D Explosion = (Area3D)ExplosionScene.Instantiate();
		AddChild(Explosion);
		await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
		
		foreach (Node3D other in Explosion.GetOverlappingBodies()){
			//if (other.IsInGroup("Enemy"))
			GD.Print(other.Name);
		}
		QueueFree();
	}

	public override void Behavior(double delta){
		if (Exploded)
			return;
		if (Position.DistanceTo(Target) <= 5)
			Explode();
		Target = ((player)GetNode($"/root/Main/Player")).Target;
		Basis = Basis.LookingAt(Target - Position);
		Velocity = -Basis[2].Normalized() * Speed * SpeedModifer;
		//GD.Print(Rotation.Z);
		MoveAndSlide();
	}
}
