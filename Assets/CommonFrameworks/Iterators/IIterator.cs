using System;

namespace CommonFrameworks.Iterators;

public interface IIterator<T> {
    public void Iterate(ITraversable<T> map, Predicate<T>? until = null, int count = -1);
    public void Iterate(ITraversable<T> map, T source, Predicate<T>? until = null, int count = -1);
}