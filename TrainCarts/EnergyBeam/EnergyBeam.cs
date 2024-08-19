using Godot;
using System;

public partial class EnergyBeam : Attack{
	Vector3 Speed = new Vector3(0,0,500);
	public void TestBehavior(double delta){
		Scale += Speed * SpeedModifer * (float)delta;
		Position += -Basis[2] * Scale.Z / 2 * (float)delta;
	}

	public EnergyBeam(){
		
		ProjBehavior += TestBehavior;
	}
	public override float Damage {get; set;} = 5.0f;
}
