using Godot;
using System;
using System.Collections.Generic;

public partial class player : CharacterBody3D
{
	[Export]
	public Vector3 CharacterSpeed = new Vector3(5,5,5);
	[Export]
	public float SprintIncrease = 1.25f;
	[Export]
	public Vector3 DroneSpeed = new Vector3(15,15,15);
	[Export]
	public RayCast3D LedgeDetect;
	public float LedgeCooldown = 0;
	[Export]
	public float CameraSpeed = .0001f;
	[Export]
	public Camera3D Camera;
	[Export]
	public Node3D CameraPivot;
	[Export]
	public Node3D Rig;
	[Export]
	public Skeleton3D Skeleton;
	[Export]
	public PackedScene DroneScene;
	[Export]
	public PackedScene RemoteWeaponScene;
	[Export]
	public Marker3D RemoteStart;
	[Export]
	public PackedScene TripwireScene;
	[Export]
	public TrainHead TrainHead;
	[Export]
	public AnimationPlayer Animations;
	[Export]
	public AnimationPlayer WeaponsAnim;
	[Export]
	public AudioStreamPlayer3D WeaponsAudio;
	public Drone Drone;
	
	public bool DroneActive = false;
	public double GunCD = 0;
	public RemoteWeapon RemoteWeapon;
	public bool RemoteWeaponActive = false;
	public TripwireWeapon CurrentTripwire;
	public bool TripwireActive = false;
	public Vector3 Target;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		//GD.Print(GetChild(0).Name);
	}
	private float ClimbCooldown = 0f;
	private void RotateCamPivot(){
		Vector2 mVelo = Input.GetLastMouseVelocity();
		CameraPivot.RotateY(-mVelo.X * CameraSpeed);
		CameraPivot.Rotate(CameraPivot.Basis[0],Mathf.Clamp(-mVelo.Y * CameraSpeed, Mathf.Pi/-2-Camera.Rotation.X,Mathf.Pi/2-Camera.Rotation.X));
	}
	private void MovementAnims(Vector3 InputVelo){
		if (InputVelo.LengthSquared() > 0){
			Rig.GlobalBasis = Rig.GlobalBasis.Orthonormalized().Slerp(Basis.LookingAt(-InputVelo).Orthonormalized(), 0.2f);
			//GD.Print((-InputVelo+Rig.GlobalPosition));
			if (Input.IsActionPressed("Shift"))
				Animations.Play("BRun");
			else
				Animations.Play("BWalk",-1,CharacterSpeed.X);
		}
		else
			Animations.Play("BIdle");
	}
	private void LedgeHop(){
		Vector3 Col = LedgeDetect.GetCollisionPoint();
		PhysicsRayQueryParameters3D RayInfo = PhysicsRayQueryParameters3D.Create(Col+new Vector3(0,2f+1f,0), Col+new Vector3(0,-1f,0));
		var Ray = GetWorld3D().DirectSpaceState.IntersectRay(RayInfo);
		if (Ray.ContainsKey("position") && ((Vector3) Ray["position"]).Y - GlobalPosition.Y < 1f){
			//Print("test");
			Target = (Vector3) Ray["position"];
			Velocity += new Vector3(0,.75f,0) * CharacterSpeed.Y + -Camera.GlobalBasis[2] * CharacterSpeed.X * new Vector3(1,0,1); // GlobalPosition - (Vector3) Ray["position"];
			LedgeCooldown = 0.2f;
		}
	}
	private void Jump(double delta){
		if (LedgeCooldown > 0)
			LedgeCooldown -= (float) delta;
		if (Input.IsActionJustPressed("Jump")){
			if (IsOnFloor())
				Velocity += new Vector3(0,1,0) * CharacterSpeed.Y;
			else if (LedgeDetect.IsColliding() && LedgeCooldown <= 0){
				LedgeHop();
			}
		}
	}
	private void PlayerMovement(double delta){
		Vector3 Forward = -new Vector3(Camera.GlobalBasis[2].X,0,Camera.GlobalBasis[2].Z).Normalized();
		Vector3 Right = new Vector3(Camera.GlobalBasis[0].X,0,Camera.GlobalBasis[0].Z).Normalized();
		Vector3 InputVelo = new Vector3(0,0,0);

		if (Input.IsActionPressed("Left"))
			InputVelo -= Right;

		if (Input.IsActionPressed("Down"))
			InputVelo -= Forward;
		
		if (Input.IsActionPressed("Right"))
			InputVelo += Right;
	
		if (Input.IsActionPressed("Up"))
			InputVelo += Forward;
			
		InputVelo = InputVelo.Normalized() * CharacterSpeed;
		if (Input.IsActionPressed("Shift"))
				InputVelo *= SprintIncrease;
		
		MovementAnims(InputVelo);
		
		Velocity += InputVelo;
		
		
		if (MoveAndSlide()){
			Vector3 oth = GetLastSlideCollision().GetColliderVelocity();
			Velocity = new Vector3(oth.X, Velocity.Y, oth.Z);
		}
		else
			Velocity -= InputVelo;
		if (!IsOnFloor())
			Velocity += new Vector3(0,-10*(float)delta,0);
		else
			Velocity *= new Vector3(0f,1,0f);
	}
	
	private void CharacterInputs(double delta){
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
			RotateCamPivot();
		Jump(delta);
		PlayerMovement(delta);
	}
	
	private void DroneInputs(double delta){
		if (Input.MouseMode == Input.MouseModeEnum.Captured){
			Vector2 mVelo = Input.GetLastMouseVelocity();
			Drone.Rotation += new Vector3(-mVelo.Y,-mVelo.X,0) * CameraSpeed;
		}
		
		MoveAndSlide();
		if (!IsOnFloor())
			Velocity += new Vector3(0,-10*(float)delta,0);
		else
			Velocity *= new Vector3(0.5f,1,0.5f);
		
		if (Input.IsMouseButtonPressed(MouseButton.Left)){
			PhysicsRayQueryParameters3D RayInfo = PhysicsRayQueryParameters3D.Create(Drone.GlobalPosition, -Drone.GlobalBasis[2]*2000);
			var Ray = GetWorld3D().DirectSpaceState.IntersectRay(RayInfo);
			if (Ray.ContainsKey("position")){
				Target = (Vector3)(Ray["position"]);
				//GD.Print(Ray["position"]+" "+Ray["collider"]);
				
			}
		}
	}
	private void SwitchCamera(){
		DroneActive = !DroneActive;
		if (DroneActive){
			Drone = (Drone)DroneScene.Instantiate();
			GetTree().Root.AddChild(Drone);
			Drone.Transform = Camera.GlobalTransform;
			Drone.Velocity = GetRealVelocity();
			Camera.Visible = false;
			Camera = (Camera3D)Drone.GetChild(0);
			Camera.Visible = true;
		}
		else{
			Drone.QueueFree();
			Camera.Visible = false;
			Camera = (Camera3D)CameraPivot.GetChild(0);
			Camera.Visible = true;
		}
		Camera.Current = true;
	}
	public Basis RemoteAim(Vector3 Ori){
		PhysicsRayQueryParameters3D RayInfo = PhysicsRayQueryParameters3D.Create(Camera.GlobalPosition, -Camera.GlobalBasis[2]*Camera.Far);
		var Ray = GetWorld3D().DirectSpaceState.IntersectRay(RayInfo);
		if (Ray.ContainsKey("position"))
			return Basis.LookingAt((Vector3)(Ray["position"]) - Ori);
		else
			return Basis.LookingAt(-Camera.GlobalBasis[2]*Camera.Far - Ori);
	}
	HashSet<Node3D> entered = new HashSet<Node3D>();
	private void TargetGunAnims(){
		if (Input.IsActionJustPressed("PrimaryFire")){
			WeaponsAnim.Play("MonacleOn");
			entered = new HashSet<Node3D>();
		}
		if (Input.IsActionJustReleased("PrimaryFire"))
			WeaponsAnim.PlayBackwards("MonacleOn");
	}
	private void TargetGun(double delta){
		if (GunCD > 0)
			GunCD -= delta;
		
		TargetGunAnims();
		if (Input.IsActionPressed("PrimaryFire") && GunCD <= 0){
			foreach (Node3D other in ((Area3D)Camera.GetChild(0)).GetOverlappingBodies()){
			//if (other.IsInGroup("Enemy"))
				if (other.IsInGroup("Enemy")){
					if (!entered.Contains(other)){
						entered.Add(other);
						WeaponsAudio.Play();
					}
					
					TrainHead.Shoot(other);
				}
			}
			GunCD = 0.2f;
			//GD.Print("shooting");
		}
	}
	
	private void RemoteGun(){
		if (Input.IsActionJustPressed("SecondaryFire")){
			//GD.Print(RemoteWeaponActive);
			if (!RemoteWeaponActive){
				WeaponsAnim.Play("ShootRemote");
				RemoteWeaponActive = true;
				RemoteWeapon = (RemoteWeapon) RemoteWeaponScene.Instantiate();
				GetTree().Root.AddChild(RemoteWeapon);
				RemoteWeapon.Velocity = GetRealVelocity();
				
				RemoteWeapon.GlobalPosition = RemoteStart.GlobalPosition;
				RemoteWeapon.Basis = RemoteAim(RemoteWeapon.GlobalPosition);
				
				RemoteWeapon.TrainHead = TrainHead;
				
				//GD.Print("Instantiate " + RemoteWeapon);
			}
			else{
				
				RemoteWeaponActive = false;
				RemoteWeapon.Detonate();
				//GD.Print("Delete Finished " + RemoteWeapon+" "+RemoteWeaponActive);
				
			}
		}
	}
	private void TripwireGun(){
		if (Input.IsActionJustPressed("TripwireFire")){
			if (!TripwireActive){
				TripwireActive = true;
				CurrentTripwire = (TripwireWeapon) TripwireScene.Instantiate();
				CurrentTripwire.TrainHead = TrainHead;
				GetTree().Root.AddChild(CurrentTripwire);
				CurrentTripwire.FirePoint(Camera.GlobalTransform,GetRealVelocity(),false);
				CurrentTripwire.PointTwo.Attached = this;
				CurrentTripwire.PointTwo.GlobalPosition = GlobalPosition;
				CurrentTripwire.PointTwo.Reparent(this);
				//GD.Print("Instantiate " + CurrentTripwire);
			}
			else{
				CurrentTripwire.FirePoint(Camera.GlobalTransform,GetRealVelocity(),true);
				TripwireActive = false;
				CurrentTripwire = null;
				//GD.Print("SecondFire " + CurrentTripwire);
			}
		}
		//else if (TripwireActive){
			//if (CurrentTripwire.PointOne.Attached != null && GlobalPosition.DistanceTo(CurrentTripwire.PointOne.GlobalPosition) > 10.0){
				//Vector3 Trip = CurrentTripwire.PointOne.GlobalPosition;
				//Vector3 Dir = (GlobalPosition - Trip).Normalized();
				//Velocity += Trip + Dir * 10 - GlobalPosition;
				//GD.Print(Trip + Dir * 10 - GlobalPosition + " "+Dir);
			//}
		//}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	float d = 0;
	public override void _Process(double delta)
	{	
		
		//GD.Print(Velocity);
		//GD.Print(Camera.Quaternion);
		Basis boneBasis = (Skeleton.GlobalBasis.Inverse() * RemoteAim(GlobalPosition)).Orthonormalized();
		boneBasis = boneBasis.Rotated(boneBasis[1],(float)Math.PI);
		Skeleton.SetBoneGlobalPose(Skeleton.FindBone("Bone.005"),
			new Transform3D(
				boneBasis,
				Skeleton.GetBoneGlobalPose(Skeleton.FindBone("Bone.005")).Origin
			));
		//GD.Print(Skeleton.GetBonePose(Skeleton.FindBone("Bone.005")));
		if (Input.IsActionJustPressed("Switch"))
			SwitchCamera();
		
		if (DroneActive)
			DroneInputs(delta);
		else
			CharacterInputs(delta);
		
		if (Input.MouseMode == Input.MouseModeEnum.Captured){
			TargetGun(delta);
			RemoteGun();
		}
		TripwireGun();
		
		
		((Node3D)GetParent().FindChild("TargetCursor")).GlobalPosition = TrainHead.GlobalPosition;
		((WorldEnvironment)GetParent().FindChild("WorldEnvironment")).Environment.BackgroundEnergyMultiplier = .75f+Mathf.Sin(d+=(float)delta/5f)/4f;
	}
}
