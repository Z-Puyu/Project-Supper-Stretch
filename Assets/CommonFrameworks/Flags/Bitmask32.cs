using System;
using System.Collections.Generic;

namespace CommonFrameworks.Flags {
    public struct Bitmask32 : IFlag<int> {
        public static Bitmask32 AllSet { get; } = new Bitmask32(~0);
        public static Bitmask32 NoneSet { get; } = new Bitmask32(0);
        
        private int Value { get; set; }

        public Bitmask32(int value) {
            this.Value = value;
        }

        public bool Has(int mask) {
            return (this.Value & mask) == mask;
        }

        public void Set(int index) {
            if (index >= 32) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.Value |= 1 << index;
        }

        public void Unset(int index) {
            if (index >= 32) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value &= ~(1 << index);
        }

        public void Toggle(int index) {
            if (index is < 0 or >= 32) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value ^= 1 << index;
        }

        public IEnumerable<int> GetAllPresent() {
            List<int> indices = new List<int>();
            for (int i = 0; i < 32; i += 1) {
                if (this.Has(1 << i)) {
                    indices.Add(i);
                }
            }

            return indices;
        }

        public bool HasAnyPresent(out int first) {
            for (int i = 0; i < 32; i += 1) {
                if (((this.Value >> i) & 1) == 0) {
                    continue;
                }

                first = i;
                return true;
            }

            first = -1;
            return false;
        }

        public static Bitmask32 operator |(Bitmask32 left, Bitmask32 right) {
            return new Bitmask32(left.Value | right.Value);
        }

        public static Bitmask32 operator &(Bitmask32 left, Bitmask32 right) {
            return new Bitmask32(left.Value & right.Value);
        }

        public static Bitmask32 operator ~(Bitmask32 value) {
            return new Bitmask32(~value.Value);
        }

        public static Bitmask32 operator ^(Bitmask32 left, Bitmask32 right) {
            return new Bitmask32(left.Value ^ right.Value);
        }

        public static implicit operator int(Bitmask32 value) {
            return value.Value;
        }

        public static implicit operator Bitmask32(int value) {
            return new Bitmask32(value);
        }

        public static implicit operator long(Bitmask32 value) {
            return value.Value;
        }
    }
}
