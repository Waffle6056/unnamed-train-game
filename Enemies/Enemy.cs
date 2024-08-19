using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	public virtual float Health {get; set;} = 1f;
	public virtual float HealthModifer {get; set;} = 1f;
	public virtual float Speed {get; set;} = 1.0f;
	public virtual float SpeedModifer {get; set;} = 1.0f;
	public virtual float SizeModifer {get; set;} = 1.0f;
	public virtual float[] Weakness {get; set;} = {0,0,0,0,0};
	public virtual float[] Resistances {get; set;} = {0,0,0,0,0};
	public Vector3 Target;
	public virtual void Hit(Attack A){
		float Damage = A.Damage * A.DamageModifer;
		if (A.Physical)
			Damage *= Mathf.Abs(A.BaseSpeed * A.SpeedModifer - Velocity.Length()) * 0.1f;
		Health -= Damage;
		if (Health < 0)
			QueueFree();
	}
	public virtual void Behavior(double delta){
		GD.Print("not binded");
	}
	public override void _Process(double delta){
		Behavior(delta);
	}
}
