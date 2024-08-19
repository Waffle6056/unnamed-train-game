using Godot;
using System;

public partial class WirePoint : StaticBody3D
{
	Vector3 Velocity;
	public Node3D Attached;
	//Vector3 Offset = new Vector3(0,0,0);
	public void Reattach(Transform3D T, Vector3 V){
		GlobalTransform = T;
		Velocity = V;
		Attached = null;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddCollisionExceptionWith(GetNode("/root/Main/Player"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Attached == null)
		{
			KinematicCollision3D c = MoveAndCollide(Velocity * (float)delta);
			if (c != null){
				Attached = (Node3D)c.GetCollider();
				Reparent(Attached);
				//Offset = c.GetPosition() - Attached.GlobalPosition;
				
			}
		}
	}
}
