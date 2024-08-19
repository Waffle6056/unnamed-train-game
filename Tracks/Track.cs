using Godot;
using System;
using System.Collections.Generic;
public record Exit(Track NextTrack, int Forwards); 
[Tool]
public partial class Track : Path3D
{	[Export]
	public Track[] ForwardsExits = new Track[0];
	[Export]
	public int[] ForwardsDirs = new int[0];
	
	[Export]
	public Track[] BackwardsExits = new Track[0];
	[Export]
	public int[] BackwardsDirs = new int[0];
	public void SetForwards(Exit[] F){
		ForwardsExits = new Track[F.Length];
		ForwardsDirs = new int[F.Length];
		for (int i = 0; i < F.Length; i++){
			ForwardsExits[i] = F[i].NextTrack;
			ForwardsDirs[i] = F[i].Forwards;
			GD.Print("Forwards Exit "+i+" : "+ForwardsExits[i].Name);
		}
	}
	public void SetBackwards(Exit[] B){
		BackwardsExits = new Track[B.Length];
		BackwardsDirs = new int[B.Length];
		for (int i = 0; i < B.Length; i++){
			BackwardsExits[i] = B[i].NextTrack;
			BackwardsDirs[i] = B[i].Forwards;
			GD.Print("Backwards Exit "+i+" : "+BackwardsExits[i].Name);
		}
	}
}
