using Godot;
using System;

public partial class Drone : Area3D
{
	[Export]
	public float Speed = 10f;
	enum DroneState{
		Floating,
		Tunneling,
		Stationary
	}
	public Vector3 Velocity;
	DroneState State = DroneState.Floating;
	Godot.Collections.Array<Node3D> LastOverlapping;
	//Vector3 Offset;
	Node3D Attached;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (State == DroneState.Floating){
			Position += -Basis[2] * Speed * (float) delta;
			Position += Velocity * (float) delta;
			if (!OverlapsBody(GetNode("/root/Main/Player"))&&HasOverlappingBodies()){
				State = DroneState.Tunneling;
				LastOverlapping = GetOverlappingBodies();
			}
		}
		else if (State == DroneState.Tunneling){
			Position += -Basis[2] * Speed * (float) delta;
			Position += Velocity * (float) delta;
			if (!HasOverlappingBodies()){
				State = DroneState.Stationary;
				Attached = LastOverlapping[0];
				Reparent(Attached);
				//Offset = GlobalPosition-Attached.GlobalPosition;
				
			}
			LastOverlapping = GetOverlappingBodies();
		}
		//else
			//Position = Attached.GlobalPosition+Offset;
		
		
		
	}
}
