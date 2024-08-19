using Godot;
using System;

public partial class CartInstance : Resource
{
	public CartInstance(){}
	public CartInstance(CartData D){
		Data = D;
	}
	[Export]
	public CartData Data;
	public TrainCart InstantiatedCart;
	public void Activate(Vector3 Target, TrainCart.Modifer AttackModifer){ 
		try{
			InstantiatedCart.Activate(Target, AttackModifer);
		}
		catch (Exception e){
			GD.Print(e.Message);
		}
		
	}
}
