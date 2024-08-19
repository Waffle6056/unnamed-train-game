using Godot;
using System;

public partial class ShotgunShell : Attack{
	public override float BaseSpeed {get; set;} = 200.0f;
	public void ShellBehavior(double delta){
		Position += -Basis[2] * (float)delta * BaseSpeed * SpeedModifer;
	}

	public ShotgunShell(){
		
		ProjBehavior += ShellBehavior;
	}
	public override float Damage {get; set;} = 5.0f;
}
