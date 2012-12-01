using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class GameObjectEx
	{
		public static GameObject IncreaseSize(this GameObject @this, float x, float y)
		{
			var scale = @this.transform.localScale;
			scale.x += x;
			scale.y += y;
			@this.transform.localScale = scale;
			return @this;
		}
	}
}

