shader_type spatial;
render_mode world_vertex_coords;

uniform float GreenOffset = 6.0;
uniform float VioletOffset = 1.3;

uniform sampler2D ZMap : hint_default_black;
uniform float ZXStretch = 1.0;
uniform float ZYStretch = 1.0;
uniform float ZTimeFactor = 1.0;
uniform float ZAmplitude = 50.0;
uniform sampler2D YMap : hint_default_black;
uniform sampler2D DistortionMap[7];
uniform float YAmplitude = 50.0;
uniform sampler2D TransparencyMap : hint_default_black;
uniform float TXStretch = 1.0;
uniform float TYStretch = 1.0;
uniform float TTimeFactor = 1.0;

uniform float FadeFactor = 20.0;
uniform float FadeOffset = 4.0;

uniform float TimeSec = 0;
uniform float Width = 10000.0;

float DistanceInfluence(float Dis, float Radius){
	float Percent = Dis/Radius;
	//if (Percent > 0.5)
	return sin((Percent - 0.25) * 2.0 * PI) / 2.0 + 0.5;
	//return Percent;
}
vec2 UVToLocal(vec2 uv){
	return uv*Width - vec2(Width/2.0,Width/2.0);
}
vec2 LocalToUV(vec2 Loc){
	return (Loc + vec2(Width/2.0,Width/2.0))/Width;
}

vec2 Rotated(vec2 uv, sampler2D Map){
	vec4 rgba = texture(Map,uv);
	vec2 vecDis = rgba.gb-uv;
	float Dis = sqrt(vecDis.x * vecDis.x + vecDis.y * vecDis.y);

	vec4 gbcen = texture(Map,rgba.gb);
	if (Dis <= rgba.a &&
		abs(gbcen.r - rgba.r) < 0.1 && abs(gbcen.g - rgba.g) < 0.1 && abs(gbcen.b - rgba.b) < 0.1 && abs(gbcen.a - rgba.a) < 0.1){
		//vecDis = vec2(-vecDis.y, vecDis.x);
		float currentRad = atan(vecDis.y/vecDis.x);
		if (vecDis.x > 0.0)
			currentRad += PI;
		float Rad = sin(TimeSec * rgba.r) * DistanceInfluence(Dis, rgba.a) * YAmplitude + currentRad;
		vec2 Tar = vec2(cos(Rad),sin(Rad)) * Dis + rgba.gb;
		//VERTEX.y -= Dis * 5000.0;

		return UVToLocal(Tar);
	}
	return UVToLocal(uv);
}


void vertex() {
	//VERTEX.xz += texture(ZMap,(VERTEX + NODE_POSITION_WORLD).xz * vec2(ZXStretch,ZYStretch) + TIME * ZTimeFactor).r * 50.0 - 12.0;
	VERTEX.y -= texture(ZMap,UV * vec2(ZXStretch,ZYStretch) + TIME * ZTimeFactor).r * ZAmplitude;
	//VERTEX.y -= texture(ZMap,UV).r * ZAmplitude;
	for (int i = 0; i < DistortionMap.length(); i++)
		VERTEX.xz += Rotated(LocalToUV(VERTEX.xz),DistortionMap[i]) + NODE_POSITION_WORLD.xz - VERTEX.xz;

	//ALBEDO = texture(ZMap,UV).rgb;

	//VERTEX.x -= texture(YMap,UV * vec2(YXStretch,YYStretch) + TIME * YTimeFactor).r * YAmplitude;


	//VERTEX += cos(TIME * 0.5 + VERTEX.x / 5.0 + VERTEX.y / 10.0) * 2.0;
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 local_vertex = INV_VIEW_MATRIX * vec4(VERTEX, 1.0) - vec4(NODE_POSITION_WORLD,1.0);
	float g = min(1,(local_vertex.y+GreenOffset)/5.0);
	float rb = VioletOffset-g;

	vec3 s = vec3(rb,g,rb);
	// Called for every pixel the material is visible on.


	ALBEDO = s;
	ALPHA = (-local_vertex.y+FadeOffset)/FadeFactor;
	ALPHA -= texture(TransparencyMap,UV * vec2(TXStretch,TYStretch) + TIME * TTimeFactor).r;
	//vec4 rgba = texture(YMap,UV);
	//vec2 vecDis = rgba.gb-UV;
	//float Dis = distance(UV,rgba.gb);
	//ALBEDO = vec3(Dis/rgba.a,Dis/rgba.a<0.1?1.0:0.0,0.0);
	//if (rgba.gb == UV)
		//ALBEDO = vec3(0,0,1);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
