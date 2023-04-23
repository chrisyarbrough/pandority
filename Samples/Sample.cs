using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Com.InnoGames
{
	[Flags]
	internal enum MyEnum
	{
		A = 0b_0001,
		B = 0b_0010,
		C = 0b_0100,
		D = 0b_1000,
	}

	internal class Sample : MonoBehaviour
	{
		private const MyEnum myEnum = MyEnum.A | MyEnum.C;

		private void Update()
		{
			bool hasFlagSystem = HasFlagSystem();
			bool hasFlagCustom = HasFlagCustom();
			Assert.AreEqual(hasFlagSystem, hasFlagCustom);

			Assert.IsTrue(myEnum.HasFlagNonAlloc(MyEnum.A));
			Assert.IsFalse(myEnum.HasFlagNonAlloc(MyEnum.B));
			Assert.IsTrue(myEnum.HasFlagNonAlloc(MyEnum.C));
			Assert.IsFalse(myEnum.HasFlagNonAlloc(MyEnum.D));
		}

		private static bool HasFlagSystem()
		{
			// This will allocate due to boxing.
			return myEnum.HasFlag(MyEnum.C);
		}

		private static bool HasFlagCustom()
		{
			// This avoids the allocation due to the concrete type being used.
			return myEnum.HasFlagNonAlloc(MyEnum.C);
		}
	}
}
