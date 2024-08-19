using Godot;
using System;

public partial class AuroraMap : MeshInstance3D
{
	[Export]
	public TextureRect mapdisplay;
	
	[Export]
	public float SheetSpeed = 1f;
	[Export]
	public Texture2D SheetTexture;
	private Image SheetImage;
	
	[Export]
	public float ScrollSpeed = 1f;
	protected ImageTexture MapTexture;
	public Image Map;
	
	[Export]
	public float DistortionCDReduction = 1f;
	[Export]
	public float DistortionMaxCD = 3f;
	[Export]
	public int Distortions = 1;
	protected ImageTexture[] DistortionTexture;
	public Image[] DistortionMap;
	
	private int Len = 256;
	public static Color Black = new Color(0,0,0,1);
	public static Color White = new Color(1,1,1,1);
	public static Color Void = new Color(0,0,0,0);
	private float TranslateRatio;
	private float TimeSec = 0f;
	[Export]
	public float TimeSpeed = 1f;
	Random RD = new Random();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SheetImage = SheetTexture.GetImage();
		if (SheetImage.IsCompressed())
			SheetImage.Decompress();
			
		Map = Image.CreateFromData(Len, Len, false, Image.Format.L8, new byte[Len * Len]);
		MapTexture = ImageTexture.CreateFromImage(Map);
		//((ShaderMaterial)MaterialOverride).SetShaderParameter("ZMap", MapTexture);
		
		DistortionTexture = new ImageTexture[Distortions];
		DistortionMap = new Image[Distortions];
		for (int i = 0; i < Distortions; i++){
			DistortionMap[i] = Image.CreateFromData(Len, Len, false, Image.Format.Rgba8, new byte[Len * Len * 4]);
			DistortionTexture[i] = ImageTexture.CreateFromImage(DistortionMap[i]);
			
		}
		((ShaderMaterial)MaterialOverride).SetShaderParameter("DistortionMap", DistortionTexture);
		mapdisplay.Texture = DistortionTexture[0];
		
		TranslateRatio = (float) SheetTexture.GetWidth() / Len;
	}
	
	
	float ScrolledDistance = 0;
	public void ScrollMap(double delta){
		ScrolledDistance += ScrollSpeed * (float)delta;
		int Offset = (int)ScrolledDistance;
		for (int i = 0; i < Len; i++){
			for (int j = 0; j < Len; j++){
				Map.SetPixel(j,i, Map.GetPixel(j,Mathf.Min(i+Offset,Len-1)));
			}
		}
		ScrolledDistance -= Offset;
	}
	
	float SheetOffset = 0;
	public void AddSheetNoise(double delta){
		SheetOffset = (SheetOffset + SheetSpeed * (float) delta) % SheetTexture.GetHeight();
		for (int i = 0; i < Len; i++){
			Color Average = new Color(0,0,0,1);
			for (int j = (int)(TranslateRatio * i); j < (int)(TranslateRatio * (i + 1)); j++)
				Average += SheetImage.GetPixel(j, (int)SheetOffset);
			Average /= (int)(TranslateRatio * (i + 1)) - (int)(TranslateRatio * i);
			Map.SetPixel(i,Len-1, Average);
			//GD.Print(Map.GetPixel(i,Len-1));
		}
	}
	public int MaxRadius(Vector2I Center, int d, int Max){
		int Radius = 0;
		while (Radius < Max){
			bool bad = false;
			for (int i = Center.Y-Radius; i <= Center.Y+Radius; i++){
				if (i < 0 || i >= Len)
					continue;
				if (Center.X-Radius >= 0 && Center.X-Radius < Len && !DistortionMap[d].GetPixel(Center.X-Radius,i).IsEqualApprox(Void))
					bad = true;
				if (Center.X+Radius >= 0 && Center.X+Radius < Len && !DistortionMap[d].GetPixel(Center.X+Radius,i).IsEqualApprox(Void))
					bad = true;
			}
			for (int j = Center.X-Radius; j <= Center.X+Radius; j++){
				if (j < 0 || j >= Len)
					continue;
				if (Center.Y-Radius >= 0 && Center.Y-Radius < Len && !DistortionMap[d].GetPixel(j,Center.Y-Radius).IsEqualApprox(Void))
					bad = true;
				if (Center.Y+Radius >= 0 && Center.Y+Radius < Len && !DistortionMap[d].GetPixel(j,Center.Y+Radius).IsEqualApprox(Void))
					bad = true;
			}
			if (bad)
				break;
			Radius++;
		}
		return Radius;
	}
	public async void AddDistortion(int d){
		
		
		//GD.Print(Center +" "+ Radius);
		float TimeScale = (float)RD.NextDouble();
		//GD.Print("TimeScale : "+C.R+" CurrentTime : "+TimeSec+" Time To Neutral : "+((Mathf.Pi - TimeSec % Mathf.Pi) / C.R / TimeSpeed));
		await ToSignal(GetTree().CreateTimer((Mathf.Pi - TimeSec * TimeScale % Mathf.Pi) / TimeScale / TimeSpeed),"timeout");
		Vector2I Center = new Vector2I(RD.Next(Len),RD.Next(Len));
		int Radius = MaxRadius(Center, d, Len/10);
		if (Radius < 10)
			return;
		Radius = RD.Next(Radius/4*3,Radius);
		Color C = new Color(TimeScale, Center.X / (float)Len, Center.Y / (float)Len, Radius / (float)Len);
		for (int i = Center.Y-Radius; i <= Center.Y+Radius; i++){
			if (i < 0 || i >= Len)
				continue;
			for (int j = Center.X-Radius; j <= Center.X+Radius; j++){
				if (j < 0 || j >= Len || Center.DistanceTo(new Vector2I(j,i)) > Radius)
					continue;
				DistortionMap[d].SetPixel(j,i, C);
			}
		}
		DistortionTexture[d].Update(DistortionMap[d]);
		await ToSignal(GetTree().CreateTimer((Radius * 30 / Len * Mathf.Pi - TimeSec * TimeScale % Mathf.Pi) / TimeScale / TimeSpeed),"timeout");
		
		for (int i = Center.Y-Radius; i <= Center.Y+Radius; i++){
			if (i < 0 || i >= Len)
				continue;
			for (int j = Center.X-Radius; j <= Center.X+Radius; j++){
				if (j < 0 || j >= Len || Center.DistanceTo(new Vector2I(j,i)) > Radius)
					continue;
				//if (DistortionMap[d].GetPixel(j,i).IsEqualApprox(C))
					DistortionMap[d].SetPixel(j,i, Void);
			}
		}
		DistortionTexture[d].Update(DistortionMap[d]);
	}
	float DistortionCooldown = 0;
	public async void AddDistortions(double delta){
		DistortionCooldown -= (float) delta;
		if (DistortionCooldown > 0)
			return;
		
		for (int d = 0; d < Distortions; d++){
			AddDistortion(d);
			await ToSignal(GetTree().CreateTimer(RD.NextDouble()*Mathf.Pi),"timeout");
		}
		DistortionCooldown = (float)RD.NextDouble() * DistortionMaxCD * DistortionCDReduction;
		//DistortionMap.SavePng("D:/godot/train/noiseTexture (2).png");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TimeSec = Time.GetTicksMsec() / 1000f * TimeSpeed;
		((ShaderMaterial)MaterialOverride).SetShaderParameter("TimeSec", TimeSec);
		ScrollMap(delta);
		AddSheetNoise(delta);
		MapTexture.Update(Map);
		AddDistortions(delta);
		//Black.R += (float)delta/30.0f;
		
		
		
		
		//MapTexture.Update(Map);
	}
}
