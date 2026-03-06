using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace ExpressionTrees {
    public static class ExpressionTree {
        public static readonly Type[] NoArgs = Type.EmptyTypes;
        
        public delegate R Accessor<in T, out R>(T target);
        
        public delegate R Method<in T, in A, out R>(T target, A argument);
        
        public delegate R Method<in T, out R>(T target, params object[] arguments);
        
        public delegate void ConsumerMethod<in T, in A>(T target, A argument);
        
        public delegate void ConsumerMethod<in T>(T target, params object[] arguments);
        
        public delegate R ProducerMethod<in T, out R>(T target);

        /// <summary>
        /// Creates a getter expression for a property or field.
        /// </summary>
        /// <param name="name">The name of the property or field to access.</param>
        /// <typeparam name="T">The type of the object containing the property or field.</typeparam>
        /// <typeparam name="R">The type of the property or field.</typeparam>
        /// <returns>A compiled lambda expression that retrieves the property or field value.</returns>
        public static Accessor<T, R> Getter<T, R>(string name) {
            ParameterExpression target = Expression.Parameter(typeof(T), "target");
            MemberExpression getter = Expression.PropertyOrField(target, name);
            return Expression.Lambda<Accessor<T, R>>(getter, target).Compile();
        }

        /// <summary>
        /// Creates a function call expression for a method, without arguments.
        /// </summary>
        /// <param name="name">The name of the method to call.</param>
        /// <typeparam name="T">The type of the object containing the method.</typeparam>
        /// <typeparam name="R">The return type of the method.</typeparam>
        /// <returns>A compiled delegate that calls the method on a runtime target.</returns>
        public static ProducerMethod<T, R> Function<T, R>(string name) {
            ParameterExpression target = Expression.Parameter(typeof(T), "target");
            MethodInfo? method = typeof(T).FindClosestMatchCallableFunction(typeof(R), name);
            Debug.Assert(method is not null, $"Method {name} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(target, method);
            return Expression.Lambda<ProducerMethod<T, R>>(call, target).Compile();
        }

        /// <summary>
        /// Creates a function call expression for a method, involving a single argument.
        /// </summary>
        /// <param name="name">The name of the method to call.</param>
        /// <typeparam name="T">The type of the object containing the method.</typeparam>
        /// <typeparam name="A">The type of the argument to the method.</typeparam>
        /// <typeparam name="R">The return type of the method.</typeparam>
        /// <returns>A compiled delegate that calls the method with runtime target and argument.</returns>
        public static Method<T, A, R> Function<T, A, R>(string name) {
            ParameterExpression target = Expression.Parameter(typeof(T), "target");
            ParameterExpression argument = Expression.Parameter(typeof(A), "argument");
            MethodInfo? method = typeof(T).FindClosestMatchCallableFunction(typeof(R), name, typeof(A));
            Debug.Assert(method is not null, $"Method {name} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(target, method, argument);
            return Expression.Lambda<Method<T, A, R>>(call, target, argument).Compile();
        }
        
        /// <summary>
        /// Creates a function call expression for a method, involving multiple arguments.
        /// </summary>
        /// <param name="name">The name of the method to call.</param>
        /// <param name="arguments">The types of the arguments to the method.</param>
        /// <typeparam name="T">The type of the object on which the method is called.</typeparam>
        /// <typeparam name="R">The return type of the method.</typeparam>
        /// <returns>A compiled delegate that calls the specified method with runtime target and arguments.</returns>
        public static Method<T, R> Function<T, R>(string name, params Type[] arguments) {
            ParameterExpression target = Expression.Parameter(typeof(T), "target");
            ParameterExpression[] args = arguments.Select((type, i) => Expression.Parameter(type, $"arg[{i}]"))
                                                  .ToArray();
            MethodInfo? method = typeof(T).FindClosestMatchCallableFunction(typeof(R), name, arguments);
            Debug.Assert(method is not null, $"Method {name} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(
                target, method, args.Select(arg => Expression.Convert(arg, arg.Type))
            );
            
            return Expression.Lambda<Method<T, R>>(call, args.Prepend(target)).Compile();
        }
        
        /// <summary>
        /// Creates a function call expression for a void-returning method, involving a single argument.
        /// </summary>
        /// <param name="name">The name of the method to call.</param>
        /// <typeparam name="T">The type on which the method is defined.</typeparam>
        /// <typeparam name="A">The type of the single argument.</typeparam>
        /// <returns>A compiled delegate that calls the method with runtime target and argument.</returns>
        public static ConsumerMethod<T, A> Action<T, A>(string name) {
            ParameterExpression target = Expression.Parameter(typeof(T), "target");
            ParameterExpression argument = Expression.Parameter(typeof(A), "argument");
            MethodInfo? method = typeof(T).FindClosestMatchCallableFunction(typeof(void), name, typeof(A));
            Debug.Assert(method is not null, $"Method {name} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(target, method, argument);
            return Expression.Lambda<ConsumerMethod<T, A>>(call, target, argument).Compile();
        }

        /// <summary>
        /// Creates a function call expression for a void-returning method, involving multiple arguments.
        /// </summary>
        /// <param name="name">The name of the method to call.</param>
        /// <param name="arguments">The types of the arguments to the method.</param>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <returns>A compiled delegate that calls the method with runtime target and arguments.</returns>
        public static ConsumerMethod<T> Action<T>(string name, params Type[] arguments) {
            ParameterExpression target = Expression.Parameter(typeof(T), "target");
            ParameterExpression[] args = arguments.Select((type, i) => Expression.Parameter(type, $"arg[{i}]"))
                                                  .ToArray();
            MethodInfo? method = typeof(T).FindClosestMatchCallableFunction(typeof(void), name, arguments);
            Debug.Assert(method is not null, $"Method {name} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(
                target, method, args.Select(arg => Expression.Convert(arg, arg.Type))
            );
            
            return Expression.Lambda<ConsumerMethod<T>>(call, args.Prepend(target)).Compile();
        }
    }
}
