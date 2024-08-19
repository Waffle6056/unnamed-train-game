using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryController : Control
{
	[Export]
	TrainConfig CurrentConfig;
	[Export]
	TrainHead TrainHead;
	[Export]
	PackedScene ItemCartInstance;
	[Export]
	PackedScene ItemCartToggle;
	[Export]
	Container CartContainer;
	[Export]
	Container MovesetContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TrainToUi();
		MovesetToUi();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("InventoryToggle")){
			Visible = !Visible;
			if (Visible)
				Input.MouseMode = Input.MouseModeEnum.Visible;
			else
				Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
	public void UiToTrain(){
		int i = 0;
		CurrentConfig.TrainData = new CartInstance[CartContainer.GetChildCount()-1];
		foreach (Node Child in CartContainer.GetChildren()){
			if (Child is ItemCartInstance){
				CurrentConfig.TrainData[i++] = ((ItemCartInstance)Child).Instance;
				//GD.Print(Child);
			}
		}
		TrainHead.InstantiateCarts();
	}
	public void TrainToUi(){
		int j = 0;
		int i = 0;
		for (; j < CartContainer.GetChildCount() && i < CurrentConfig.TrainData.Length; j++){
			Node Child = CartContainer.GetChild(j);
			if (Child is ItemCartInstance){
				((ItemCartInstance)Child).Instance = CurrentConfig.TrainData[i++];
				((ItemCartInstance)Child).UpdateIcon();
			}
		}
		for (; j < CartContainer.GetChildCount(); j++)
			CartContainer.GetChild(j).QueueFree();
		for (; i < CurrentConfig.TrainData.Length; i++){
			ItemCartInstance C = (ItemCartInstance)ItemCartInstance.Instantiate();
			C.Instance = CurrentConfig.TrainData[i];
			C.UpdateIcon();
			C.Controller = this;
			
			CartContainer.AddChild(C);
		}
			
	}
	public void UiToMoveset(){
		int i = 0;
		foreach (Node Child in MovesetContainer.GetChildren()){
			if (Child is ItemCartToggle){
				CurrentConfig.Movesets[i/4].AssignedCarts[i%4] = ((ItemCartToggle)Child).Instance;
				i++;
			}
		}
	}
	public void MovesetToUi(){
		int j = 0;
		int i = 0;
		for (; j < MovesetContainer.GetChildCount() && i < CurrentConfig.Movesets.Length * 4; j++){
			Node Child = MovesetContainer.GetChild(j);
			if (Child is ItemCartToggle){
				((ItemCartToggle)Child).Instance = CurrentConfig.Movesets[i/4].AssignedCarts[i%4];
				((ItemCartToggle)Child).UpdateIcon();
				i++;
			}
		}
		for (; j < MovesetContainer.GetChildCount(); j++)
			MovesetContainer.GetChild(j).QueueFree();
		for (; i < CurrentConfig.Movesets.Length * 4; i++){
			ItemCartToggle C = (ItemCartToggle)ItemCartToggle.Instantiate();
			C.Instance = CurrentConfig.Movesets[i/4].AssignedCarts[i%4];
			C.UpdateIcon();
			C.Controller = this;
			MovesetContainer.AddChild(C);
		}
	}
	
	public bool InsertCart(ItemCartInstance Item){
		//GD.Print("InstanceCartCalled");
		Vector2 ItemPos = Item.Handle.GlobalPosition;
		foreach (Node N in CartContainer.GetChildren()){
			Vector2 NPos = ((Control) N).GlobalPosition;
			//GD.Print(ItemPos.DistanceTo(NPos));
			if (ItemPos.DistanceTo(NPos) < 132f && ItemPos.X > NPos.X){
				if (Item.GetParent() != null)
					Item.GetParent().RemoveChild(Item);
				N.AddSibling(Item);
				return true;
			}
		}
		return false;
	}
	public bool AssignCart(CartInstance Instance, Vector2 ItemPos){
		foreach (Node N in MovesetContainer.GetChildren()){
			if (!(N is ItemCartToggle))
				continue;
			
			Vector2 NPos = ((Control) N).GlobalPosition;
			if (ItemPos.DistanceTo(NPos) < 66f){
				((ItemCartToggle) N).Instance = Instance;
				((ItemCartToggle) N).UpdateIcon();
				//MovesetContainer.RemoveChild()
				return true;
			}
		}
		return false;
	}
	public void AddRow(){
		Moveset[] Added = new Moveset[CurrentConfig.Movesets.Length+1];
		int i = 0;
		foreach (Moveset MV in CurrentConfig.Movesets)
			Added[i++] = MV;
		Added[i] = new Moveset(new CartInstance[4]);
		CurrentConfig.Movesets = Added;
		MovesetToUi();
	}
	public void RemoveRow(){
		Moveset[] Removed = new Moveset[CurrentConfig.Movesets.Length-1];
		for (int i = 0; i < Removed.Length; i++)
			Removed[i] = CurrentConfig.Movesets[i];
		CurrentConfig.Movesets = Removed;
		MovesetToUi();
	}
	public ItemCartInstance CartToInstance(ItemCart Item){
		ItemCartInstance C = (ItemCartInstance)ItemCartInstance.Instantiate();
		C.Instance = new CartInstance(((ItemCart)Item).Data);
		C.Handle.GlobalPosition = Item.Handle.GlobalPosition;
		C.Controller = Item.Controller;
		return C;
	}
	public void ItemInstance(InventoryItem Item){
		if (Item is ItemCart)
			if (InsertCart(CartToInstance((ItemCart) Item)))
				UiToTrain();
		
		if (Item is ItemCartInstance){
			if (InsertCart((ItemCartInstance)Item))
				UiToTrain();
			else if (AssignCart(((ItemCartInstance)Item).Instance, Item.Handle.GlobalPosition))
				UiToMoveset();
			else{
				Item.GetParent().RemoveChild(Item);
				Item.QueueFree();
				UiToTrain();
			}
		}
		if (Item is ItemCartToggle){
			AssignCart(((ItemCartToggle)Item).Instance, Item.Handle.GlobalPosition);
			((ItemCartToggle)Item).Instance = null;
			((ItemCartToggle)Item).UpdateIcon();
			UiToMoveset();
		}
		
		
	}

}
