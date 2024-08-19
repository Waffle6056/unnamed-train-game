using Godot;
using System;

public partial class Attack : Area3D
{
	public virtual bool Physical {get; set;} = true;
	public virtual float Damage {get; set;} = 1.0f;
	public virtual float DamageModifer {get; set;} = 1.0f;
	public virtual float BaseSpeed {get; set;} = 50.0f;
	public virtual float SpeedModifer {get; set;} = 1.0f;
	public virtual float SizeModifer {get; set;} = 1.0f;
	
	public Vector3 Target {get; set;} = new Vector3(0,0,0);
	public delegate void Behavior(double delta);
	public virtual Behavior ProjBehavior {get; set;}
	public override void _Ready(){
		Scale *= SizeModifer;
		Delete();
	}
	async void Delete(){
		await ToSignal(GetTree().CreateTimer(30f),"timeout");
		QueueFree();
	}
	public override void _Process(double delta)
	{
		ProjBehavior(delta);
	}
	public virtual void OnBodyEntered(Node3D body)
	{
		if (body.IsInGroup("Enemy"))
			((Enemy)body).Hit(this);
	// Replace with function body.
	}
}
