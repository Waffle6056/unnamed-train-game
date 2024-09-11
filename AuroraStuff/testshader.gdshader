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
uniform sampler2D DistortionMap[20];
uniform float YAmplitude = 50.0;
uniform sampler2D TransparencyMap : hint_default_black;
uniform sampler2D BackgroundMap : hint_default_black;
uniform float TXStretch = 1.0;
uniform float TYStretch = 1.0;
uniform float TTimeFactor = 1.0;

uniform float FadeFactor = 20.0;
uniform float FadeOffset = 4.0;

uniform float TimeSec = 0;
uniform float Width = 10000.0;

float GetHeight(vec2 uv){
	return min(-texture(ZMap,uv * vec2(ZXStretch,ZYStretch) + TIME * ZTimeFactor).r * ZAmplitude, 
	-texture(BackgroundMap,uv).r * ZAmplitude / 7.0);
}

vec3 getNormal(vec2 uv) {
	const vec3 off = vec3(-1,0,1);
    vec2 texelSize = 1.0 / vec2(textureSize(ZMap, 0)) / 2.0;

    float left = GetHeight( uv + texelSize * off.xy);
    float right = GetHeight( uv + texelSize * off.zy);
    float top = GetHeight( uv + texelSize * off.yx);
    float down = GetHeight( uv + texelSize * off.yz);

    return normalize(vec3(left-right, 2.0, down-top));
}

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
	VERTEX.y += GetHeight(UV);
	//VERTEX.y -= texture(BackgroundMap,UV).r * ZAmplitude/7.0;
	for (int i = 0; i < DistortionMap.length(); i++)
		VERTEX.xz += Rotated(LocalToUV(VERTEX.xz),DistortionMap[i]) + NODE_POSITION_WORLD.xz - VERTEX.xz;
	VERTEX.y += texture(BackgroundMap,UV * vec2(ZXStretch,ZYStretch) + TIME * ZTimeFactor).r * ZAmplitude * 2.0;
	//NORMAL = getNormal(UV);
	//ALBEDO = texture(ZMap,UV).rgb;

	//VERTEX.x -= texture(YMap,UV * vec2(YXStretch,YYStretch) + TIME * YTimeFactor).r * YAmplitude;


	//VERTEX += cos(TIME * 0.5 + VERTEX.x / 5.0 + VERTEX.y / 10.0) * 2.0;
	// Called for every vertex the material is visible on.
}

float GetTransp(vec2 uv){
	//float dis = abs(local_vertex.y-FadeOffset);
	//if (dis > 200.0)
	//	ALPHA -= 0.2;
	float normal = abs(getNormal(uv).g);
//	ALPHA -= normal*normal*normal*normal*normal*FadeFactor;
	float o = 1.0-min(min(
				normal*normal*normal*normal*normal*FadeFactor,
				normal*50.0),
				0.35
				);
	//
	o *= 1.0+texture(TransparencyMap,uv * vec2(TXStretch,TYStretch) + TIME * TTimeFactor).r * 0.5;
	o -= (1.0 - abs(GetHeight(uv)/ZAmplitude)) * 0.85;
	return o;
}

void fragment() {
	vec4 local_vertex = INV_VIEW_MATRIX * vec4(VERTEX, 1.0) - vec4(NODE_POSITION_WORLD,1.0);
	float g = min(1,(local_vertex.y+GreenOffset)/5.0);
	float rb = VioletOffset-g;

	vec3 s = vec3(0,1,0);//*(texture(TransparencyMap,UV * vec2(TXStretch,0.3)).rgb + 0.7);
	// Called for every pixel the material is visible on.

	//if (dis > 200.0)
	//	ALPHA -= 0.2;
	
	
	ALBEDO = s;
	float normal = abs(getNormal(UV).g);
//	ALPHA -= normal*normal*normal*normal*normal*FadeFactor;
	ALPHA = 0.0;
	for (int i = -1; i <= 1; i++){
		for (int j = -1; j <= 1; j++){
			ALPHA += GetTransp(UV+vec2(float(i)*0.001,float(j)*0.001));
		}
	}
	ALPHA /= 9.0;
	//vec4 rgba = texture(YMap,UV);
	//vec2 vecDis = rgba.gb-UV;
	//float Dis = distance(UV,rgba.gb);
	//ALBEDO = vec3(Dis/rgba.a,Dis/rgba.a<0.1?1.0:0.0,0.0);
	//if (rgba.gb == UV)
		//ALBEDO = vec3(0,0,1);
}

