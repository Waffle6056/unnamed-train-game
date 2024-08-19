using Godot;
using System;

public partial class RocketShell : Attack
{
	public override float BaseSpeed {get; set;} = 300.0f;
	public void ShellBehavior(double delta){
		Position += -Basis[2].Normalized() * BaseSpeed * SpeedModifer * (float)delta;
	}
	
	public RocketShell(){
		
		ProjBehavior += ShellBehavior;
	}
	public bool Exploded = false;
	public async void Explode(){
		Exploded = true;
		Scale *= 20f;
		ProjBehavior = delegate(double delta){return;};
		await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
		foreach (Node3D other in GetOverlappingBodies()){
			//if (other.IsInGroup("Enemy"))
			if (other.IsInGroup("Enemy"))
				((Enemy)other).Hit(this);
		}
		QueueFree();
	}
	public override void OnBodyEntered(Node3D body)
	{
		if (!Exploded)
			Explode();
	// Replace with function body.
	}
}
