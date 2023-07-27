// Writen by Martin Nerurkar ( www.sharkbombs.com)

float getShineTime(float freq, float pause, float x, float width, float fade) {
	x = -pause + fmod(x * freq, 1 + pause);
	
	#if _SHINE_REVERSE_ON
		x = 1 - x;
	#endif

	// Time Curve Setting
	#if _SHINE_SMOOTH_ON
		x = smoothstep(0, 1, x);
	#elif _SHINE_CUBIC_0N
		x = pow(x, 3);
	#endif
	
	return - (width + fade) + (x * (1 + 2 * (width + fade)));
}


float getShineUV(float x, float width, float fade, float2 uv) {
	if (x > (-width - fade))
		return (smoothstep(x - width - fade, x - width, uv.x) - smoothstep(x + width, x + width + fade, uv.x)) * uv.y;
	else 
		return 0;
}