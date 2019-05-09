using System;

namespace PokyPoker.Domain.Helpers
{
    public static class ArrayExtensions
    {
        public static ArraySegment<TValue> Slice<TValue>(this TValue[] values) =>
            new ArraySegment<TValue>(values);

        public static ArraySegment<TValue> Slice<TValue>(this TValue[] values, int offset) =>
            new ArraySegment<TValue>(values, offset, values.Length - offset);

        public static ArraySegment<TValue> Slice<TValue>(this TValue[] values, int offset, int count) =>
            new ArraySegment<TValue>(values, offset, count);
    }
}
