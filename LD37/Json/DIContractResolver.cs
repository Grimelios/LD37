using System;
using Newtonsoft.Json.Serialization;
using Ninject;

namespace LD37.Json
{
	public class DIContractResolver : DefaultContractResolver
	{
		private IKernel kernel;

		public DIContractResolver(IKernel kernel)
		{
			this.kernel = kernel;
		}

		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract contract = base.CreateObjectContract(objectType);
			contract.DefaultCreator = () => kernel.Get(objectType);

			return contract;
		}
	}
}
