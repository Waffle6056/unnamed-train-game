using Godot;
using System;

public partial class TestAttack : Attack{
	public void TestBehavior(double delta){
		Position += -Basis[2] * (float)delta * new Vector3(20f,20f,20f) * SpeedModifer;
	}

	public TestAttack(){
		
		ProjBehavior += TestBehavior;
	}
	public override float Damage {get; set;} = 5.0f;
}

