using Godot;
using System;

public partial class Fish : Enemy{
	enum State{
		Circling,
		Feeding,
		Fleeing,
	}

	public override float Speed {get; set;} = 20f;
	State CurrentState = State.Circling;
	float Adj;
	float pAdj;
	Vector3 rand;
	async void Circling(){
		//GD.Print("Circling");
		CurrentState = State.Circling;
		float Radius = new Vector2(Position.X,Position.Z).DistanceTo(new Vector2(Target.X,Target.Z))/2;
		float time = (2 * Mathf.Pi * Radius) / 2 / Speed;
		Adj = Mathf.Pi / time;
		pAdj = (Target.Y-Position.Y)/time;
		Basis = Basis.LookingAt(Target - Position);
		Rotation += new Vector3(0,Mathf.Pi/-2,0);
		Rotation *= new Vector3(0,1,0);
		
		await ToSignal(GetTree().CreateTimer(time),"timeout");
		if (Position.DistanceTo(Target) < 5)
			Feeding();
		else
			Circling();
	}
	async void Feeding(){
		//GD.Print("Feeding");
		CurrentState = State.Feeding;
		await ToSignal(GetTree().CreateTimer(.2f),"timeout");
		
		Fleeing();
	}
	async void Fleeing(){
		//GD.Print("Fleeing");
		CurrentState = State.Fleeing;
		rand = new Vector3(GD.Randf()*3-1.5f,GD.Randf()*3-1.5f,0);
		await ToSignal(GetTree().CreateTimer(1f),"timeout");
		
		Circling();
	}
	public override void _Ready(){
		Circling();
	}
	public override void Behavior(double delta){

		Target = ((player)GetNode($"/root/Main/Player")).Target;
		Velocity = new Vector3(0,0,0);
		if (CurrentState == State.Circling){
			Velocity += new Vector3(0,pAdj,0);
			Rotation += new Vector3(0,Adj,0)  * (float)delta;
			//RotateObjectLocal(Basis[1].Normalized(),Adj * (float)delta);
			if (Position.DistanceTo(Target) < 5)
				Feeding();
		}
		else if (CurrentState == State.Feeding)
			Basis = Basis.LookingAt(Target - Position);
		else if (CurrentState == State.Fleeing)
			Basis = Basis.LookingAt(-(new Vector3(Target.X,Position.Y,Target.Z) - Position) + rand);
		
		
		
		Velocity += -Basis[2].Normalized() * Speed * SpeedModifer;
		if (CurrentState == State.Fleeing || CurrentState == State.Feeding)
			Velocity *= 2;
		//GD.Print(Rotation.Z);
		MoveAndSlide();
	}
}
