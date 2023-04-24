// ReSharper disable UnusedType.Global
// ReSharper disable UnusedType.Local

namespace Com.InnoGames
{
	using System;
	using UnityEngine;
	using UnityEngine.Assertions;

	[Flags]
	public enum MyPublic
	{
	}

	[Flags]
	internal enum MyInternal
	{
		A = 0b_0001,
		B = 0b_0010,
		C = 0b_0100,
		D = 0b_1000,
	}

	public class Sample : MonoBehaviour
	{
		[Flags]
		public enum MyNestedPublic
		{
			Fire
		}

		[Flags]
		internal enum MyNestedInternal
		{
			Hello
		}

		[Flags]
		protected enum MyNestedProtected
		{
		}

		[Flags]
		private enum MyNestedPrivate
		{
		}

		private static class NestedInnerClass
		{
			[Flags]
			public enum MyInnerNestedPublic
			{
			}
		}

		private const MyInternal myEnum = MyInternal.A | MyInternal.C;

		private void Update()
		{
			bool hasFlagSystem = HasFlagSystem();
			bool hasFlagCustom = HasFlagCustom();
			Assert.AreEqual(hasFlagSystem, hasFlagCustom);

			Assert.IsTrue(myEnum.HasFlagNonAlloc(MyInternal.A));
			Assert.IsFalse(myEnum.HasFlagNonAlloc(MyInternal.B));
			Assert.IsTrue(myEnum.HasFlagNonAlloc(MyInternal.C));
			Assert.IsFalse(myEnum.HasFlagNonAlloc(MyInternal.D));
		}

		private static bool HasFlagSystem()
		{
			// This will allocate due to boxing.
			return myEnum.HasFlag(MyInternal.C);
		}

		private static bool HasFlagCustom()
		{
			// This avoids the allocation due to the concrete type being used.
			return myEnum.HasFlagNonAlloc(MyInternal.C);
		}
	}
}
