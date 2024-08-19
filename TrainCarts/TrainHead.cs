using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public partial class TrainHead : AnimatableBody3D
{
	
	
	[Export]
	public TrainConfig Config;
	[Export]
	public float CartLength = 0;
	public Moveset MovesetCurrent;
	[Export]
	public float SpeedBase = 10;
	[Export]
	public float Distance = 1;
	[Export]
	public float FuelConsumption = 0;
	public List<TrainCart> Carts = new List<TrainCart>();
	public Vector3 Target;
	public TrainCart.Modifer AttackModifer = delegate(Attack A, TrainCart other){ return; };
	
	public player Player;
	[Export]
	public MovesetUI MovesetUI;
	
	[Export]
	public Track TrackMain;
	public bool Dir = true;
	public int TrackBranch = 0;
	
	

	private void TempAddCart(CartInstance d){
		
		int ind = Carts.Count;
		Carts.Add((TrainCart)d.Data.CartScene.Instantiate());
		Carts[ind].Head = this;
		Carts[ind].CartPosition = ind+1;
		d.InstantiatedCart = Carts[ind];
		GetTree().Root.AddChild(Carts[ind]);
		Carts[ind].PoweredOn();
		//Carts[ind].Position = new Vector3(0,0,Carts[ind].CartPosition * 11);
	}
	public async void InstantiateCarts(){
		await ToSignal(GetTree().CreateTimer(0.01f),"timeout");
		if (Carts != null){
			foreach (TrainCart C in Carts){
				C.QueueFree();
			}
		}
		Carts = new List<TrainCart>();
		int ind = 1;
		AttackModifer = delegate(Attack A, TrainCart other){ return; };
		foreach (CartInstance d in Config.TrainData){
			d.InstantiatedCart = (TrainCart)d.Data.CartScene.Instantiate();
			GetTree().Root.AddChild(d.InstantiatedCart);
			Carts.Add(d.InstantiatedCart);
			d.InstantiatedCart.CartPosition = ind++;
			d.InstantiatedCart.Head = this;
			d.InstantiatedCart.PoweredOn();
			
		}
		//Distance = Config.FollowingDistance * Carts.Count;
		//Transform = MainTrack.Curve.SampleBakedWithRotation(Distance);
		//GD.Print(Transform.Origin.DistanceTo(MainTrack.Curve.SampleBakedWithRotation(Distance).Origin));
		////Position += Basis[1].Normalized() * 10f;
		//foreach (TrainCart t in Carts){
			//t.Transform = MainTrack.Curve.SampleBakedWithRotation(Distance-t.CartPosition*Config.FollowingDistance+1.15f);
			////t.Position += Basis[1].Normalized() * 10f;
		//}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		Player = (player)GetNode("/root/Main/Player");
		//MainTrack = new Track();
		//GD.Print(MainTrack.Name);
		AddPoints(new Exit((Track)GetNode("/root/Main/TrackController").GetChild(0,true),1));
		InstantiateCarts();
		//SpeedBase+=1;
	}
	
	public async Task Shoot(Node3D T){
		for (int i = 0; i < Carts.Count; i++){
			if (Carts[i].Powered){
				Carts[i].Activate(T.GlobalPosition, AttackModifer);
				await ToSignal(GetTree().CreateTimer(0.05f),"timeout");
			}
		}
	}
	private void tempinputs(){
		if (Input.IsActionJustPressed("DebugSpeedUp")){
			SpeedBase+=1;
			GD.Print(SpeedBase);
		}
		if (Input.IsActionJustPressed("DebugSpeedDown")){
			SpeedBase-=1;
			GD.Print(SpeedBase);
		}
		if (Input.IsActionJustPressed("DebugFuelConsumptionUp")){
			FuelConsumption+=1;
			GD.Print(FuelConsumption);
		}
		if (Input.IsActionJustPressed("DebugFuelConsumptionDown")){
			FuelConsumption-=1;
			GD.Print(FuelConsumption);
		}

		if (Input.IsActionJustPressed("BranchUp")){
			TrackBranch++;
			GD.Print("TrackBranch : "+TrackBranch);
		}
		if (Input.IsActionJustPressed("BranchDown")){
			TrackBranch--;
			GD.Print("TrackBranch : "+TrackBranch);
		}
		if (Input.IsActionJustPressed("ActivateA"))
			MovesetCurrent.AssignedCarts[0].Activate(Target,AttackModifer);
		if (Input.IsActionJustPressed("ActivateB"))
			MovesetCurrent.AssignedCarts[1].Activate(Target,AttackModifer);
		if (Input.IsActionJustPressed("ActivateC"))
			MovesetCurrent.AssignedCarts[2].Activate(Target,AttackModifer);
		if (Input.IsActionJustPressed("ActivateD"))
			MovesetCurrent.AssignedCarts[3].Activate(Target,AttackModifer);
		if (Input.IsActionJustPressed("SwitchMoveset")){
			Config.MovesetsInd++;
			Config.MovesetsInd %= Config.Movesets.Length;
			MovesetUI.Load(Config.Movesets, Config.MovesetsInd);
			MovesetCurrent = Config.Movesets[Config.MovesetsInd];
		}
		
		
		
		//((Node3D)GetNode("/root/Main/Drone")).GlobalPosition = Carts[ind].GlobalPosition + Carts[ind].CameraOffset;
	}
	private void AddPoints(Exit Next){
		GD.Print("Transfered to " +Next.NextTrack.Name);
		Curve3D NextPath = Next.NextTrack.Curve;
		Curve3D CurrentPath = TrackMain.Curve;
		TrackRigger.RemoveExteriorHandles(Next.NextTrack);
		if (Next.Forwards==1){
			int end = NextPath.PointCount;
			for(int i = 0; i < end; i++){
				CurrentPath.AddPoint(NextPath.GetPointPosition(i),NextPath.GetPointIn(i),NextPath.GetPointOut(i));
				//GD.Print(NextTrack.GetPointPosition(i));
			}
		}
		else{
			for(int i = NextPath.PointCount-1; i >= 0; i--){
				CurrentPath.AddPoint(NextPath.GetPointPosition(i),NextPath.GetPointIn(i),NextPath.GetPointOut(i));
				//GD.Print(NextTrack.GetPointPosition(i));
			}
		}
		Dir = Next.Forwards==1;

		
		TrackMain.ForwardsExits = Next.NextTrack.ForwardsExits;
		TrackMain.ForwardsDirs = Next.NextTrack.ForwardsDirs;
		TrackMain.BackwardsExits = Next.NextTrack.BackwardsExits;
		TrackMain.BackwardsDirs = Next.NextTrack.BackwardsDirs;
		GD.Print("Transfer completed");
		
	}
	private float CalcSpeed(){
		float WeightLimit = 1;
		if (FuelConsumption > 0)
			WeightLimit = Mathf.Sqrt(FuelConsumption)*10;
		float Ratio = Config.WeightCurrent / WeightLimit;
		if (Ratio <= .50f)
			return SpeedBase;
		else if (Ratio <= .80f)
			return SpeedBase * .85f;
		else if (Ratio <= 1f)
			return SpeedBase * .7f;
		return SpeedBase * Mathf.Max(0, 1-(.3f * Ratio * Ratio));
	}
	private void Movement(double delta){

		Distance += CalcSpeed() * (float) delta;

		if (Distance > TrackMain.Curve.GetBakedLength()){
			foreach (Track e in TrackMain.ForwardsExits){
				GD.Print(e.Name);
			}
			foreach (int e in TrackMain.ForwardsDirs){
				GD.Print(e);
			}
			if (Dir)
				AddPoints(new Exit(TrackMain.ForwardsExits[TrackBranch],TrackMain.ForwardsDirs[TrackBranch]));
			else
				AddPoints(new Exit(TrackMain.BackwardsExits[TrackBranch],TrackMain.BackwardsDirs[TrackBranch]));
			
		}
		Transform3D NextTransform = TrackMain.Curve.SampleBakedWithRotation(Distance);
		Basis NextRot = new Basis(Transform.Basis.GetRotationQuaternion().Normalized().Slerp(NextTransform.Basis.GetRotationQuaternion().Normalized(),0.25f));
		Transform = new Transform3D(NextRot,NextTransform.Origin);
		
		//Transform = Transform.InterpolateWith(TrackMain.Curve.SampleBakedWithRotation(Distance),0.5f);
		//GD.Print(Transform.Basis.GetRotationQuaternion().Normalized()+" "+NextTransform.Basis.GetRotationQuaternion().Normalized());
		//Position += Basis[1].Normalized() * 10f;
		
		float P = Distance - CartLength/2f;
		foreach (TrainCart t in Carts){
			P -= t.CartLength/2;
			NextTransform = TrackMain.Curve.SampleBakedWithRotation(Mathf.Max(1,P));
			P -= t.CartLength/2;
			NextRot = new Basis(t.Transform.Basis.GetRotationQuaternion().Normalized().Slerp(NextTransform.Basis.GetRotationQuaternion().Normalized(),0.25f));
			t.Transform = new Transform3D(NextRot,NextTransform.Origin);
			//t.Transform = t.Transform.InterpolateWith(MainTrack.Curve.SampleBakedWithRotation(Distance-t.CartPosition*CartDistance+1.15f),0.5f);
			//t.Position += Basis[1].Normalized() * 10f;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		Target = Player.Target;
		
		tempinputs();
		Movement(delta);

		//Fuel -= Output * (float) delta;
		
		//GD.Print(Distance +" "+Fuel+" "+Speed+" "+PoweredCarts);
	}
}
