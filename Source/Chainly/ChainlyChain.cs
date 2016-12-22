using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chainly
{
	public class ChainlyChain<TObject>
	{
		public TObject Result { get; private set; }

		public ChainlyChain(TObject item)
		{
			Result = item;
		}

		public ChainlyChain<TObject> Then(Action<TObject> chainedMethod)
		{
			chainedMethod(Result);

			return this;
		}
	}
}
