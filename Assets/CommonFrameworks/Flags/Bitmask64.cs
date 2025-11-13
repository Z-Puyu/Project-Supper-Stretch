using System;
using System.Collections.Generic;

namespace CommonFrameworks.Flags {
    public struct Bitmask64 : IFlag<int> {
        public static Bitmask64 AllSet { get; } = new Bitmask64(~0L);
        public static Bitmask64 NoneSet { get; } = new Bitmask64(0L);
        
        private long Value { get; set; }

        public Bitmask64(long value) {
            this.Value = value;
        }

        public bool Has(int flag) {
            return (this.Value & flag) == flag;
        }

        public void Set(int index) {
            if (index is < 0 or >= 64) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.Value |= 1L << index;
        }

        public void Unset(int index) {
            if (index is < 0 or >= 64) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value &= ~(1 << index);
        }

        public void Toggle(int index) {
            if (index is < 0 or >= 64) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            this.Value ^= 1 << index;
        }
        
        public IEnumerable<int> GetAllPresent() {
            List<int> indices = new List<int>();
            for (int i = 0; i < 64; i += 1) {
                if (this.Has(1 << i)) {
                    indices.Add(i);
                }
            }

            return indices;
        }
        
        public bool HasAnyPresent(out int first) {
            for (int i = 0; i < 63; i += 1) {
                if (((this.Value >> i) & 1) == 0) {
                    continue;
                }

                first = i;
                return true;
            }

            first = -1;
            return false;
        }
        
        public static Bitmask64 operator |(Bitmask64 left, Bitmask64 right) {
            return new Bitmask64(left.Value | right.Value);
        }

        public static Bitmask64 operator &(Bitmask64 left, Bitmask64 right) {
            return new Bitmask64(left.Value & right.Value);
        }

        public static Bitmask64 operator ~(Bitmask64 value) {
            return new Bitmask64(~value.Value);
        }

        public static Bitmask64 operator ^(Bitmask64 left, Bitmask64 right) {
            return new Bitmask64(left.Value ^ right.Value);
        }

        public static implicit operator long(Bitmask64 value) {
            return value.Value;
        }

        public static implicit operator Bitmask64(long value) {
            return new Bitmask64(value);
        }
    }
}
