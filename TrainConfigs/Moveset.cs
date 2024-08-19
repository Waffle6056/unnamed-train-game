using Godot;
using System;

public partial class Moveset : Resource{
	public Moveset(){}
	public Moveset(CartInstance[] CI){
		AssignedCarts = CI;
	}
	[Export] public CartInstance[] AssignedCarts;
}
