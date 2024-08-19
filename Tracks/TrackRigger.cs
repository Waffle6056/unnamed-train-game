#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class TrackRigger : EditorScript
{
	private Vector3 GetStart(Exit N){
		if (N.Forwards==1)
			return N.NextTrack.Curve.SampleBaked(0);
		else
			return N.NextTrack.Curve.SampleBaked(N.NextTrack.Curve.GetBakedLength());
	}
	private float GetOrder(Transform3D Start, Exit N){
		return (-Start.Basis[0].Normalized()).AngleTo((GetStart(N)-Start.Origin).Normalized());
	}
	private bool Near(Transform3D Start, Vector3 N){
		return Start.Origin.DistanceTo(N) < 5f;
	}
	private bool InFront(Transform3D Start,Vector3 N, int dir){
		return (Start.Basis[2].Normalized() * -1 * dir).AngleTo((N-Start.Origin).Normalized()) < Mathf.Pi/4;
	}
	private void Rig(Track A){
		List<Exit> ForwardsExits = new List<Exit>();
		List<Exit> BackwardsExits = new List<Exit>();
		
		Transform3D Entrance = A.Curve.SampleBakedWithRotation(0) * A.GlobalTransform;
		Transform3D Exit = A.Curve.SampleBakedWithRotation(A.Curve.GetBakedLength()) * A.GlobalTransform;
		foreach(Node n2 in GetScene().GetChildren()){
			if (n2 != A && n2 is Track){
				GD.Print(n2.Name+" being checked");
				Track B = (Track) n2;
				Vector3 StartB = B.Curve.SampleBaked(0) + B.GlobalPosition;
				Vector3 EndB = B.Curve.SampleBaked(B.Curve.GetBakedLength()) + B.GlobalPosition;
				if (Near(Exit,StartB) && InFront(Exit,StartB,1)){
					ForwardsExits.Add(new Exit(B,1)); 
					GD.Print("ForwardExit Forward added");
				}
				if (Near(Exit,EndB) && InFront(Exit,EndB,1)){
					ForwardsExits.Add(new Exit(B,0)); 
					GD.Print("ForwardExit Backwards added");
				}
				
				
				if (Near(Entrance,StartB) && InFront(Entrance,StartB,-1)){
					BackwardsExits.Add(new Exit(B,1));
					GD.Print("BackwardsExit Forward added");
				}
				if (Near(Entrance,EndB) && InFront(Entrance,EndB,-1)){
					BackwardsExits.Add(new Exit(B,0)); 
					GD.Print("BackwardsExit Backwards added");
				}
				
			}
		}
		
		ForwardsExits.Sort((x,y) => (GetOrder(Exit,x)<GetOrder(Exit,y)?-1:1));
		BackwardsExits.Sort((x,y) => (GetOrder(Entrance,x)<GetOrder(Entrance,y)?1:-1));
		
		A.SetForwards(ForwardsExits.ToArray());
		A.SetBackwards(BackwardsExits.ToArray());
		GD.Print("SORTED & EXITED");
	}
	public static void RemoveExteriorHandles(Track N){
		N.Curve.SetPointIn(0,new Vector3(0,0,0));
		N.Curve.SetPointOut(0,new Vector3(0,0,0));
		N.Curve.SetPointIn(N.Curve.PointCount-1,new Vector3(0,0,0));
		N.Curve.SetPointOut(N.Curve.PointCount-1,new Vector3(0,0,0));
	}
	// Called when the script is executed (using File -> Run in Script Editor).
	public override void _Run()
	{
		
		foreach(Node n in GetScene().GetChildren()){
			if (n is Track){
				GD.Print(n.Name+"WIP");
				Rig((Track)n);
				
			}
		}
	}
}
#endif
