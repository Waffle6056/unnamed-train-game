using Godot;
using System;

public partial class SwitchModCart : TrainCart
{
	public void Mod(Attack A, TrainCart other){
		if (CartPosition < other.CartPosition){
			if (Powered){
				A.SpeedModifer += .25f;
				A.SizeModifer -= .25f;
			} else{
				A.SpeedModifer -= .25f;
				A.SizeModifer += .25f;
			}
		}
		
	}
	public override void _Ready(){
		Head.AttackModifer += Mod;
	}
}
