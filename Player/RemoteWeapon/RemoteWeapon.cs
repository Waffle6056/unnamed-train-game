using Godot;
using System;

public partial class RemoteWeapon : StaticBody3D
{
	[Export]
	public float Speed = 100f;
	public Vector3 Velocity;
	Node3D Attached;
	bool Detonated = false;
	[Export]
	public Node3D ProjectileMesh;
	[Export]
	public Node3D AttachedMesh;
	public TrainHead TrainHead;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddCollisionExceptionWith(GetNode("/root/Main/Player"));
	}
	public async void Detonate(){
		if (Detonated)
			return;
		Detonated = true;
		await TrainHead.Shoot(this);
		QueueFree();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Attached != null)
			return;
		KinematicCollision3D c = MoveAndCollide( (-Basis[2] * Speed + Velocity) * (float) delta );
		
		if (c != null){
			Attached = (Node3D)c.GetCollider();
			ProjectileMesh.Visible = false;
			AttachedMesh.Visible = true;
			Reparent(Attached);
		}
		ProjectileMesh.RotateY(Mathf.Pi * 4 * (float) delta);
		
	}
	public void ChildAdded(Node c){
		if (c is RemoteWeapon){
			GD.Print(c);
			c.Reparent(Attached);
			Detonate();
			((RemoteWeapon)c).Detonate();
		}
	}
}
