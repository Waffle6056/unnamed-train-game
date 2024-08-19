using Godot;
using System;

public partial class ItemCartToggle : InventoryItem
{
	[Export]
	public CartInstance Instance;
	public override void UpdateIcon(){
		if (Instance != null)
			Handle.TextureNormal = Instance.Data.Icon;
		else
			Handle.TextureNormal = DefaultIcon;
	}
}
