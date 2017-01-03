using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chainly
{
	/// <summary>
	/// Handles extensions for different version of .NET
	/// </summary>
	public static class TypeExtensions
	{

#if NETSTANDARD
		public static IEnumerable<MethodInfo> CrossGetMethods(this Type type)
		{
			return type.GetRuntimeMethods();
		}

		public static MethodInfo CrossGetMethod(this Type type, string name, Type[] parameters)
		{
			return type.GetRuntimeMethod(name, parameters);
		}

		public static bool CrossIsPublic(this Type type)
		{
			return type.GetTypeInfo().IsPublic;
		}

		public static bool CrossIsInterface(this Type type)
		{
			return type.GetTypeInfo().IsInterface;
		}

#elif NETCORE

		public static IEnumerable<MethodInfo> CrossGetMethods(this Type type)
		{
			return type.GetRuntimeMethods();
		}

		public static MethodInfo CrossGetMethod(this Type type, string name, Type[] parameters)
		{
			return type.GetRuntimeMethod(name, parameters);
		}

		public static bool CrossIsPublic(this Type type)
		{
			return type.GetTypeInfo().IsPublic;
		}

		public static bool CrossIsInterface(this Type type)
		{
			return type.GetTypeInfo().IsInterface;
		}

#else
		public static IEnumerable<MethodInfo> CrossGetMethods(this Type type)
		{
			return type.GetMethods();
		}

		public static MethodInfo CrossGetMethod(this Type type, string name, Type[] parameters)
		{
			return type.GetMethod(name, parameters);
		}

		public static bool CrossIsPublic(this Type type)
		{
			return type.IsPublic;
		}

		public static bool CrossIsInterface(this Type type)
		{
			return type.IsInterface;
		}
#endif

	}
}
