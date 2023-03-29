using System.Reflection;

namespace ShimmysAdminTools.Models
{
	public struct FieldAccessor<C, T> where C : class
	{
		public FieldInfo Field { get; }

		public C Instance { get; }

		public T Value
		{
			get => (T)Field.GetValue(Instance);
			set => Field.SetValue(Instance, value);
		}

		public FieldAccessor(string name, C instance = null)
		{
			Field = typeof(C).GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			Instance = instance;
		}
	}
}
