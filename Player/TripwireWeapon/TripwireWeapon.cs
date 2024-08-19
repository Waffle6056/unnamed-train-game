using Godot;
using System;

public partial class TripwireWeapon : Area3D
{
	[Export]
	public float Speed = 200f;
	
	[Export]
	public WirePoint PointOne;
	[Export]
	public WirePoint PointTwo;
	[Export]
	CollisionShape3D Wire;
	[Export]
	MeshInstance3D WireMesh;
	//[Export]
	//public GPUParticles3D Particles;
	public TrainHead TrainHead;
	public void FirePoint(Transform3D T, Vector3 V, bool Point){
		if (!Point)
			PointOne.Reattach(T,V + Speed * -T.Basis[2]);
		else
			PointTwo.Reattach(T,V + Speed * -T.Basis[2]);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		WireMeshMesh = ((CylinderMesh)(WireMesh.Mesh));
	}
	CylinderMesh WireMeshMesh;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		((BoxShape3D)(Wire.Shape)).Size = new Vector3(PointOne.GlobalPosition.DistanceTo(PointTwo.GlobalPosition),1,1);
		Wire.GlobalPosition = (PointOne.GlobalPosition + PointTwo.GlobalPosition ) / 2;
		Wire.GlobalBasis = Basis.LookingAt(PointOne.GlobalPosition-Wire.GlobalPosition);
		Wire.GlobalBasis = Wire.GlobalBasis.Rotated(Wire.GlobalBasis[1].Normalized(),Mathf.Pi/2);
		WireMeshMesh.Height = PointOne.GlobalPosition.DistanceTo(PointTwo.GlobalPosition)-.03f;
		WireMesh.GlobalTransform = Wire.GlobalTransform.RotatedLocal(Vector3.Forward,Mathf.Pi/2);
		//((ParticleProcessMaterial)Particles.ProcessMaterial).EmissionShapeOffset = new Vector3(0,-WireMeshMesh.Height/2,0);
		
	}
	public async void Detonate(Node3D T){
		await TrainHead.Shoot(T);
		//QueueFree();
	}
	public void Tripped(Node3D b){
		if (b.IsInGroup("Enemy"))
			Detonate(b);
	}
}
