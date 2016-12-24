using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chainly
{
	public class ChainlyChain<TObject>
	{
		private readonly TObject _item;

		public ChainlyChain(TObject item)
		{
			_item = item;
		}

		public ChainlyChain<TObject> Then(Action<TObject> chainedMethod)
		{
			chainedMethod(_item);

			return this;
		}

		public static ChainlyChain<TObject> operator +(ChainlyChain<TObject> chain, Action<TObject> chainedMethod)
		{
			return chain.Then(chainedMethod);
		}

		public TObject Value()
		{
			return _item;
		}
	}
}
