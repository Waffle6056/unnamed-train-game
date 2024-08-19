using Godot;
using System;

public partial class ItemCart : InventoryItem
{
	[Export]
	public CartData Data;
	public override void UpdateIcon(){
		if (Data != null)
			Handle.TextureNormal = Data.Icon;
		else
			Handle.TextureNormal = DefaultIcon;
	}
}
