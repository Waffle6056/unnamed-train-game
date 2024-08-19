using Godot;
using System;

public partial class MortarShell : Attack
{
	public override float BaseSpeed {get; set;} = 100.0f;
	public float VertVelo;
	public float HoriVelo;
	public void ShellBehavior(double delta){
		Position += (-Basis[2].Normalized() + new Vector3(0,1,0))* (float)delta * new Vector3(HoriVelo,VertVelo,HoriVelo);
		VertVelo += (float)delta * -10;
	}
	
	public MortarShell(){
		
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
