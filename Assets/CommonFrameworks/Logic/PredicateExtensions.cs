using System;
using System.Collections.Generic;

namespace CommonFrameworks.Logic {
    public static class PredicateExtensions {
        public static bool All<T>(this IEnumerable<Predicate<T>> predicates, T args) {
            foreach (Predicate<T> predicate in predicates) {
                if (!predicate.Invoke(args)) {
                    return false;
                }
            }
            
            return true;
        }
        
        public static bool Any<T>(this IEnumerable<Predicate<T>> predicates, T args) {
            foreach (Predicate<T> predicate in predicates) {
                if (predicate.Invoke(args)) {
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool None<T>(this IEnumerable<Predicate<T>> predicates, T args) {
            foreach (Predicate<T> predicate in predicates) {
                if (predicate.Invoke(args)) {
                    return false;
                }
            }
            
            return true;
        }
        
        public static bool NotAll<T>(this IEnumerable<Predicate<T>> predicates, T args) {
            foreach (Predicate<T> predicate in predicates) {
                if (!predicate.Invoke(args)) {
                    return true;
                }
            }
            
            return false;
        }
    }
}
