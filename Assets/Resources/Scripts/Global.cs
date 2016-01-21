using System.Runtime.InteropServices;
using UnityEngine;

/*
 * a bunch of global stuff that everyone needs put into the same place
 */
public class Global
{
	public const float TransitionToContextExamTime = 1f;
	public const float ResetToIdleTime = 2.5f;
	public const float StirUpSpeed = 50f;
	public const float EnlargeMultipler = 1.5f;

	public static bool FiftyFifty { get { return UnityEngine.Random.value > 0.5f; } }
	
	// find facing angle based on default sprite direction
	public static float Angle (Vector2 defaultDirection, Vector2 direction) {
		float theta = Vector2.Angle(defaultDirection, direction);
		if (defaultDirection == Vector2.down) {
			theta = direction.x < 0f ? -theta : theta;
		} else if (defaultDirection == Vector2.right) {
  		theta = direction.y > 0f ? -theta : theta;
		} else if (defaultDirection == Vector2.up) {
			theta = direction.x > 0f ? -theta : theta;
		} else if (defaultDirection == Vector2.left) {
  		theta = direction.y < 0f ? -theta : theta;
		} 
		return theta;
	}


	// hacky but efficient Fast inverse square root algorithm
	public static float FastSqrt(float z)
	{
		if (z == 0) return 0;
		FloatIntUnion u;
		u.tmp = 0;
		u.f = z;
		u.tmp -= 1 << 23; /* Subtract 2^m. */
		u.tmp >>= 1; /* Divide by 2. */
		u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
		return u.f;
	}
	// C style union what could go wrong?
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int tmp;
	}
}