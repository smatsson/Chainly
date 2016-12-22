using System;

namespace Chainly
{
	public static class ObjectExtensions
	{
		public static TInterface Chain<TInterface>(this object item)
		where TInterface : class
		{
			var objectType = item.GetType();
			var interfaceType = typeof(TInterface);
			var type = EmitBuilder.Build(interfaceType, objectType);
			return (TInterface)Activator.CreateInstance(type, args: item);
		}

		public static ChainlyChain<TObject> CreateChain<TObject>(this TObject item)
		{
			return new ChainlyChain<TObject>(item);
		}
	}
}
