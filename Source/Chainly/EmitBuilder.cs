using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable once CheckNamespace
namespace Chainly
{
	internal static class EmitBuilder
	{
		/// <summary>
		/// Module builder only needs to be created once.
		/// </summary>
		private static ModuleBuilder _moduleBuilder;

		/// <summary>
		/// Type cache for faster lookups.
		/// </summary>
		private static readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();

		/// <summary>
		/// If a type fails to be created it cannot be re-created until he app domain unloads.
		/// This cache is for keeping exceptions for a specific interface + object combo and returning it
		/// every time that exception would have happend.
		/// </summary>
		private static readonly Dictionary<string, Exception> BrokenTypeCache = new Dictionary<string, Exception>();

		/// <summary>
		/// Constants for hardcoded strings.
		/// </summary>
		private const string ValueMethod = "Value";
		private const string ItemField = "_item";
		private const string DynamicAssemblyName = "ChainlyDynamicClasses";
		private const string DynamicClassSuffix = "ChainlyProxy";

		/// <summary>
		/// Build a dynamic type for the specified interface and object types.
		/// A cached copy will be returned if the type has already been built.
		/// </summary>
		/// <param name="interfaceType">The interface type to implement</param>
		/// <param name="objectType">The object type to wrap in the interface </param>
		/// <returns>A dynamic type for the specified interface and object types</returns>
		internal static Type Build(Type interfaceType, Type objectType)
		{
			var typeId = $"{interfaceType.FullName}_{objectType.FullName}";
			lock (Cache)
			{
				try
				{
					if (BrokenTypeCache.ContainsKey(typeId))
					{
						throw BrokenTypeCache[typeId];
					}

					if (Cache.ContainsKey(typeId))
					{
						return Cache[typeId];
					}


					AssertPublic(interfaceType);
					AssertPublic(objectType);

					if (_moduleBuilder == null)
					{
						CreateModuleBuilder();
					}

					var builder = CreateType(typeId, interfaceType);

					var itemField = builder.DefineField(ItemField, objectType, FieldAttributes.Private | FieldAttributes.InitOnly);

					AddConstructor(builder, objectType, itemField);

					// Create methods from interface
					foreach (var fluentMethod in interfaceType.CrossGetMethods().Where(f => f.ReturnType == interfaceType))
					{
						AddFluentMethod(interfaceType, objectType, builder, itemField, fluentMethod);
					}

					var valueMethod = interfaceType.CrossGetMethod(ValueMethod, new Type[0]);
					if (valueMethod != null && valueMethod.ReturnType == objectType)
					{
						AddValueMethod(builder, objectType, itemField);
					}

#if NETSTANDARD || NETCORE
					var type = builder.CreateTypeInfo().AsType();
#else
					var type = builder.CreateType();
#endif

					Cache.Add(typeId, type);
					return type;
				}
				catch (Exception ex)
				{
					if (!BrokenTypeCache.ContainsKey(typeId))
					{
						BrokenTypeCache.Add(typeId, ex);
					}
					throw;
				}
			}
		}

		private static void AssertPublic(Type type)
		{
			if (!type.CrossIsPublic())
			{
				throw new ChainlyException(
					$"{(type.CrossIsInterface() ? "Interface" : "Object")} \"{type.FullName}\" must be public");
			}
		}

		private static void AddConstructor(TypeBuilder builder, Type objectType, FieldInfo itemField)
		{
			var constructor = builder.DefineConstructor(
				MethodAttributes.Public |
				MethodAttributes.SpecialName |
				MethodAttributes.RTSpecialName,
				CallingConventions.Standard,
				new[] { objectType });

			var generator = constructor.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, itemField);
			generator.Emit(OpCodes.Ret);
		}

		private static void AddValueMethod(TypeBuilder builder, Type objectType, FieldInfo itemField)
		{
			var methodBuilder = builder.DefineMethod(ValueMethod,
				MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.NewSlot
				| MethodAttributes.Virtual
				| MethodAttributes.Final,
				objectType, new Type[0]);

			var generator = methodBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, itemField);
			generator.Emit(OpCodes.Ret);
		}

		private static void AddFluentMethod(Type interfaceType, Type objectType, TypeBuilder builder, FieldInfo field,
			MethodBase fluentMethod)
		{
			var parameters = fluentMethod.GetParameters().Select(f => f.ParameterType).ToArray();
			var realMethodToRun = objectType.CrossGetMethod(fluentMethod.Name, parameters);

			if (realMethodToRun == null)
			{
				throw new ChainlyException($"Found no method to override with the same arguments for method \"{fluentMethod.Name}\"");
			}

			var methodBuilder = builder.DefineMethod(fluentMethod.Name,
				MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.NewSlot
				| MethodAttributes.Virtual
				| MethodAttributes.Final,
				interfaceType,
				parameters);

			var generator = methodBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, field);

			for (var i = 0; i < parameters.Length; i++)
			{
				generator.Emit(OpCodes.Ldarg, (i + 1));
			}

			generator.Emit(OpCodes.Callvirt, realMethodToRun);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ret);
		}

		private static TypeBuilder CreateType(string typeId, Type interfaceType)
		{
			var typeName = $"{DynamicAssemblyName}.{typeId}{DynamicClassSuffix}";

			var typeBuilder = _moduleBuilder.DefineType(typeName,
				TypeAttributes.Public |
				TypeAttributes.Class |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoLayout,
				typeof(object),
				new[] { interfaceType });

			return typeBuilder;
		}

		private static void CreateModuleBuilder()
		{
			var assemblyName = new AssemblyName { Name = DynamicAssemblyName };

#if NETSTANDARD || NETCORE

			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			_moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name);

#else

			var thisDomain = System.Threading.Thread.GetDomain();
			var assemblyBuilder = thisDomain.DefineDynamicAssembly(assemblyName,
				AssemblyBuilderAccess.Run);

			_moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, false);

#endif
		}
	}
}
