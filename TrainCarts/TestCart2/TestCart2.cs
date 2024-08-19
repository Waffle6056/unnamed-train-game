using Godot;
using System;

public partial class TestCart2 : TrainCart
{
	
	public void name(Attack A, TrainCart other){
		if (CartPosition < other.CartPosition){
				A.SpeedModifer += .25f;
				A.SizeModifer -= .125f;
			}
	}
	public override void PoweredOn(){Powered = true; Head.AttackModifer += name;}
	public override void PoweredOff(){Powered = false; Head.AttackModifer -= name;}
	public override void _Ready(){
	}
}
