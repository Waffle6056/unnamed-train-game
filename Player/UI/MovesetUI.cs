using Godot;
using System;

public partial class MovesetUI : Control
{

	[Export]
	public Texture2D Empty;
	[Export]
	public TextureRect[] MovesetIcons;
	int q = 0;
	public async void Load(Moveset[] Movesets, int MovesetsInd){
		int len = MovesetIcons.Length/4;
		for (int i = 0; i+3 < MovesetIcons.Length; i += 4){
			Moveset Current = Movesets[MovesetsInd];
			for (int j = i; j < i+4; j++){
				MovesetIcons[j].Modulate = new Color(1,1,1,1 - i/4 * .20f);
				if (Current.AssignedCarts[j-i] != null)
					MovesetIcons[j].Texture = Current.AssignedCarts[j-i].Data.Icon;
				else
					MovesetIcons[j].Texture = Empty;
					
			}
			MovesetsInd++;
			MovesetsInd %= Movesets.Length;
		}
		q++;
		
		await ToSignal(GetTree().CreateTimer(1f),"timeout");
		q--;
		if (q == 0){
			for (int i = 4; i < MovesetIcons.Length; i += 4){
				for (int j = i; j < i+4 && j < MovesetIcons.Length; j++){
					MovesetIcons[j].Modulate = new Color(1,1,1,0);
				}
			}
		}
	}
}
