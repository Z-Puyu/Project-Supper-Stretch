using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommonFrameworks.Extensions {
    public static class EnumExtensions {
        private static class Helper {
            private static bool HasOverlap(sbyte x, sbyte y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(byte x, byte y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(short x, short y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(ushort x, ushort y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(int x, int y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(uint x, uint y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(long x, long y) {
                return (x & y) != 0;
            }

            private static bool HasOverlap(ulong x, ulong y) {
                return (x & y) != 0;
            }

            private static bool HasFlag(sbyte x, sbyte y) {
                return (x & y) == y;
            }
            
            private static bool HasFlag(byte x, byte y) {
                return (x & y) == y;
            }
            
            private static bool HasFlag(short x, short y) {
                return (x & y) == y;
            }

            private static bool HasFlag(ushort x, ushort y) {
                return (x & y) == y;
            }
            
            private static bool HasFlag(int x, int y) {
                return (x & y) == y;
            }

            private static bool HasFlag(uint x, uint y) {
                return (x & y) == y;
            }
            
            private static bool HasFlag(long x, long y) {
                return (x & y) == y;
            }
            
            private static bool HasFlag(ulong x, ulong y) {
                return (x & y) == y;
            }
        }
        
        private static class Helper<E> where E : Enum {
            internal static readonly Func<E, E, bool> HasOverlap =
                    (Func<E, E, bool>)Delegate.CreateDelegate(
                        typeof(Func<E, E, bool>),
                        typeof(EnumExtensions.Helper).GetMethod(
                            name: "HasOverlap", 
                            bindingAttr: BindingFlags.NonPublic | BindingFlags.Static,
                            binder: null, 
                            types: new[] { typeof(E), typeof(E) }, 
                            modifiers: null
                        )!
                    );

            internal static readonly Func<E, E, bool> HasFlag =
                    (Func<E, E, bool>)Delegate.CreateDelegate(
                        typeof(Func<E, E, bool>),
                        typeof(EnumExtensions.Helper).GetMethod(
                            name: "HasFlag",
                            bindingAttr: BindingFlags.NonPublic | BindingFlags.Static,
                            binder: null,
                            types: new[] { typeof(E), typeof(E) },
                            modifiers: null
                        )!
                    );
        }

        public static bool HasFlag<E>(this E e, E flags) where E : struct, Enum {
            return !EqualityComparer<E>.Default.Equals(e, default) && Helper<E>.HasFlag(e, flags);
        }
        
        public static bool Overlaps<E>(this E e, E flags) where E : struct, Enum {
            return !EqualityComparer<E>.Default.Equals(e, default) && Helper<E>.HasOverlap(e, flags);
        }
        
        public static bool HasNone<E>(this E e, E flags) where E : struct, Enum {
            return !e.Overlaps(flags);
        }
    }
}
