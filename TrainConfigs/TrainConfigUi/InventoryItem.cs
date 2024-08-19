using Godot;
using System;

public partial class InventoryItem : CenterContainer
{
	[Export]
	public InventoryController Controller;
	[Export]
	public TextureButton Handle;
	[Export]
	public Texture2D DefaultIcon;
	// Called when the node enters the scene tree for the first time.
	
	public virtual void UpdateIcon(){
		Handle.TextureNormal = DefaultIcon;
	}
	public override void _Ready(){
		UpdateIcon();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Handle.ButtonPressed)
			Handle.GlobalPosition = GetViewport().GetMousePosition() - Handle.Size / 2;
		else
			Handle.Position = Vector2.Zero;
	}
	public void OnRelease(){
		//GD.Print("OnReleaseCalled");
		Controller.ItemInstance(this);
	}
}
