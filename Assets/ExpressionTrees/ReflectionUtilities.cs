using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ExpressionTrees {
    public static class ReflectionUtilities {
        private static Type? ResolveParamArrayType(Type?[] args, int parameterCount) {
            Type? type = args.Skip(parameterCount - 1).FirstOrDefault(arg => arg is not null) ??
                         args.Take(parameterCount).LastOrDefault(arg => arg is not null);
            return type is not null &&
                   args.Skip(parameterCount - 1).Where(arg => arg is not null).All(type.IsAssignableFrom)
                    ? type
                    : null;
        }

        private static Type?[] Compress(this IEnumerable<Type?> args, ParameterInfo[] parameters) {
            Type?[] arguments = args.ToArray();
            if (parameters.Length >= arguments.Length) {
                return arguments;
            }

            Array.Resize(ref arguments, parameters.Length);
            arguments[^1] = ReflectionUtilities.ResolveParamArrayType(arguments, parameters.Length);
            return arguments;       
        }

        private static int DistanceFrom(this Type type, Type? other) {
            if (other is null) {
                return 0;
            }
            
            Type? child = type;
            Type parent = other;
            if (child.IsAssignableFrom(parent)) {
                (child, parent) = (parent, child);
            }
            
            int distance = 0;
            while (child is not null && child != parent) {
                child = child.BaseType;
                distance += 1;
            }
            
            return distance;
        }

        /// <summary>
        /// Checks if an argument of a given type can be assigned to a parameter.
        /// If the type is <c>null</c>, it checks if the parameter is optional.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="type">The type to check against the parameter.</param>
        /// <returns><c>true</c> if the type can be assigned to the parameter, <c>false</c> otherwise.</returns>
        public static bool IsAssignableWith(this ParameterInfo parameter, Type? type) {
            return type is null ? parameter.HasDefaultValue : parameter.ParameterType.IsAssignableFrom(type);
        }
        
        /// <summary>
        /// Checks if the return value from a method is assignable to the given type.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="type">The expected return type of the method.
        /// <c>null</c> means that the method should return <c>void</c>.</param>
        /// <returns><c>true</c> if the method returns the given type, <c>false</c> otherwise.</returns>
        public static bool Returns(this MethodInfo method, Type? type) {
            return (type ?? typeof(void)).IsAssignableFrom(method.ReturnType);
        }

        /// <summary>
        /// Checks if the method accepts the given arguments.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="arguments">The types of the arguments. A <c>null</c> type means that
        /// no argument is provided for that parameter.</param>
        /// <returns><c>true</c> if the method accepts the given arguments, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Constraint: the method cannot contain any pass-by-reference parameters
        /// (i.e., <c>ref</c>, <c>in</c>, or <c>out</c>).
        /// </remarks>
        public static bool Accepts(this MethodInfo method, params Type?[] arguments) {
            ParameterInfo[] parameters = method.GetParameters();
            Debug.Assert(Array.TrueForAll(parameters, parameter => !parameter.ParameterType.IsByRef));
            arguments = arguments.Compress(parameters);
            Debug.Assert(Array.TrueForAll(arguments, argument => argument is null || !argument.IsByRef));
            Debug.Assert(parameters.Length == arguments.Length);
            for (int i = 0; i < parameters.Length; i += 1) {
                if (!parameters[i].IsAssignableWith(i < arguments.Length ? arguments[i] : null)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds all callable functions in the given object which have the given name, return type, and arguments.
        /// </summary>
        /// <param name="obj">The object on which to search for callable functions.</param>
        /// <param name="name">The name of the functions to find.</param>
        /// <param name="returns">The return type of the functions.</param>
        /// <param name="arguments">The types of the arguments used to call the functions.</param>
        /// <returns>A collection of <see cref="MethodInfo"/> objects representing the callable functions.</returns>
        /// <remarks>
        /// Constraint: the method cannot contain any pass-by-reference parameters
        /// (i.e., <c>ref</c>, <c>in</c>, or <c>out</c>).
        /// </remarks>
        public static IEnumerable<MethodInfo> FindCallableFunctions(
            this object obj, string name, Type returns, params Type?[] arguments
        ) {
            return obj.FindCallableFunctions(returns, arguments).Where(method => method.Name == name);
        }

        /// <summary>
        /// Finds all callable functions in the given type which conforms to the given return type and arguments.
        /// </summary>
        /// <param name="obj">The object on which to search for callable functions.</param>
        /// <param name="returns">The return type of the functions.</param>
        /// <param name="arguments">The types of the arguments used to call the functions.</param>
        /// <returns>A collection of <see cref="MethodInfo"/> objects representing the callable functions.</returns>
        /// <remarks>
        /// Constraint: the method cannot contain any pass-by-reference parameters
        /// (i.e., <c>ref</c>, <c>in</c>, or <c>out</c>).
        /// </remarks>
        public static IEnumerable<MethodInfo> FindCallableFunctions(
            this object obj, Type returns, params Type?[] arguments
        ) {
            return obj.GetType().GetMethods(ReflectionFlags.Everything)
                      .Where(method => method.Returns(returns) && method.Accepts(arguments));
        }

        /// <summary>
        /// Finds the closest matching function in the give type that is
        /// callable with the given return type and arguments.
        /// </summary>
        /// <param name="obj">The object on which to search for callable functions.</param>
        /// <param name="returns">The return type of the function.</param>
        /// <param name="args">The types of the arguments used to call the function.</param>
        /// <returns>The closest matching <see cref="MethodInfo"/> object representing the callable function.</returns>
        /// <remarks>
        /// Constraint: the method cannot contain any pass-by-reference parameters
        /// (i.e., <c>ref</c>, <c>in</c>, or <c>out</c>).
        /// </remarks>
        public static MethodInfo? FindClosestMatchCallableFunction(this object obj, Type returns, params Type?[] args) {
            return ReflectionUtilities.FindClosestMatch(obj.FindCallableFunctions(returns, args), returns, args);
        }

        /// <summary>
        /// Finds the closest matching function in the give type that is
        /// callable with the given return type and arguments.
        /// </summary>
        /// <param name="obj">The object on which to search for callable functions.</param>
        /// <param name="returns">The return type of the function.</param>
        /// <param name="name">The name of the function to find.</param>
        /// <param name="args">The types of the arguments used to call the function.</param>
        /// <returns>The closest matching <see cref="MethodInfo"/> object representing the callable function.</returns>
        /// <remarks>
        /// Constraint: the method cannot contain any pass-by-reference parameters
        /// (i.e., <c>ref</c>, <c>in</c>, or <c>out</c>).
        /// </remarks>
        public static MethodInfo? FindClosestMatchCallableFunction(
            this object obj, Type returns, string name, params Type?[] args
        ) {
            return ReflectionUtilities.FindClosestMatch(obj.FindCallableFunctions(name, returns, args), returns, args);
        }

        private static MethodInfo? FindClosestMatch(
            IEnumerable<MethodInfo> methods, Type returns, params Type?[] arguments
        ) {
            MethodInfo? method = null;
            double closest = double.MaxValue;
            foreach (MethodInfo candidate in methods) {
                Type?[] args = arguments.Compress(candidate.GetParameters());
                double score = candidate.GetParameters()
                                        .Select(parameter => parameter.ParameterType)
                                        .Prepend(candidate.ReturnType)
                                        .Zip(args.Prepend(returns), (param, arg) => param.DistanceFrom(arg))
                                        .Average();
                if (score >= closest) {
                    continue;
                }

                closest = score;
                method = candidate;
            }
            
            return method;       
        }
    }
}
