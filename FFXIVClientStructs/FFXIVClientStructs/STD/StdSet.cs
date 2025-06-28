using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.STD.ContainerInterface;
using FFXIVClientStructs.STD.Helper;

namespace FFXIVClientStructs.STD;

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public unsafe struct StdSet<T, TMemorySpace>
    : IStdSet<T>
        , IEquatable<StdSet<T, TMemorySpace>>
        , IStaticNativeObjectOperation<StdSet<T, TMemorySpace>>
    where T : unmanaged
    where TMemorySpace : IStaticMemorySpace {
    public RedBlackTree<T, T, DefaultKeyExtractor<T>> Tree;

    public static bool HasDefault => true;
    public static bool IsDisposable => true;
    public static bool IsCopyable => StdOps<T>.IsCopyable;
    public static bool IsMovable => true;

    /// <inheritdoc/>
    public readonly long LongCount => Tree.LongCount;

    /// <inheritdoc cref="IStdSet{T}.Count"/>
    public readonly int Count => checked((int)Tree.LongCount);

    /// <inheritdoc/>
    public readonly RedBlackTree<T, T, DefaultKeyExtractor<T>>.Enumerator GetEnumerator() => new(Tree.Pointer, true);

    /// <inheritdoc/>
    public readonly RedBlackTree<T, T, DefaultKeyExtractor<T>>.Enumerator Reverse() => new(Tree.Pointer, false);

    /// <inheritdoc/>
    public bool AddCopy(in T value) {
        if (!StdOps<T>.IsCopyable)
            throw new InvalidOperationException("Copying is not supported");
        if (!Tree.TryInsertEmpty<TMemorySpace>(value, out var node))
            return false;

        StdOps<T>.ConstructCopyInPlace(value, out node->_Myval);
        return true;
    }

    /// <inheritdoc/>
    public bool AddMove(ref T value) {
        if (!StdOps<T>.IsMovable)
            throw new InvalidOperationException("Copying is not supported");
        if (!Tree.TryInsertEmpty<TMemorySpace>(value, out var node))
            return false;

        StdOps<T>.ConstructMoveInPlace(ref value, out node->_Myval);
        return true;
    }

    /// <inheritdoc/>
    public void Clear() => Tree.EraseHead();

    /// <inheritdoc/>
    public readonly bool Contains(in T item) {
        var b = Tree.FindLowerBound(item).Bound;
        return b is not null && StdOps<T>.ContentEquals(b->_Myval, item);
    }

    /// <inheritdoc/>
    public readonly void CopyTo(T[] array, int arrayIndex) {
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, null);
        if (array.Length - arrayIndex < LongCount)
            throw new ArgumentException(null, nameof(array));

        var i = (long)arrayIndex;
        foreach (ref var p in this)
            array[i++] = p;
    }

    /// <inheritdoc/>
    public void Dispose() => Tree.EraseHead();

    /// <inheritdoc cref="object.Equals(object?)"/>
    public readonly bool Equals(in StdSet<T, TMemorySpace> other) => Tree.Equals(other.Tree);

    /// <inheritdoc/>
    readonly bool IEquatable<StdSet<T, TMemorySpace>>.Equals(StdSet<T, TMemorySpace> other) => Tree.Equals(other.Tree);

    /// <inheritdoc/>
    public readonly override bool Equals(object? obj) => obj is StdSet<T, TMemorySpace> t && Equals(t);

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other) {
        var n = Tree.Min();
        if (n is null)
            return;

        if (other is not IReadOnlySet<T> otherSet)
            otherSet = other.ToHashSet();

        using var enumerator = GetEnumerator();
        for (var state = enumerator.MoveNext(); state;) {
            state = otherSet.Contains(enumerator.Current)
                ? enumerator.DeleteAndMoveNext()
                : enumerator.MoveNext();
        }
    }

    /// <inheritdoc/>
    public readonly override int GetHashCode() => Tree.GetHashCode();

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other) {
        var n = Tree.Min();
        if (n is null)
            return;

        if (other is not IReadOnlySet<T> otherSet)
            otherSet = other.ToHashSet();

        using var enumerator = GetEnumerator();
        for (var state = enumerator.MoveNext(); state;) {
            state = !otherSet.Contains(enumerator.Current)
                ? enumerator.DeleteAndMoveNext()
                : enumerator.MoveNext();
        }
    }

    /// <inheritdoc cref="IReadOnlySet{T}.IsProperSubsetOf"/>
    public readonly bool IsProperSubsetOf(IEnumerable<T> other) {
        CountOverlaps(other, out var notFoundInOther, out var notFoundInSelf);
        return notFoundInOther == 0 && notFoundInSelf > 0;
    }

    /// <inheritdoc cref="IReadOnlySet{T}.IsProperSupersetOf"/>
    public readonly bool IsProperSupersetOf(IEnumerable<T> other) {
        CountOverlaps(other, out var notFoundInOther, out var notFoundInSelf);
        return notFoundInOther > 0 && notFoundInSelf == 0;
    }

    /// <inheritdoc cref="IReadOnlySet{T}.IsSubsetOf"/>
    public readonly bool IsSubsetOf(IEnumerable<T> other) {
        CountOverlaps(other, out var notFoundInOther, out _);
        return notFoundInOther == 0;
    }

    /// <inheritdoc cref="IReadOnlySet{T}.IsSupersetOf"/>
    public readonly bool IsSupersetOf(IEnumerable<T> other) {
        CountOverlaps(other, out _, out var notFoundInSelf);
        return notFoundInSelf == 0;
    }

    /// <inheritdoc/>
    public readonly ref readonly T Min() => ref Tree.Min()->_Myval;

    /// <inheritdoc/>
    public readonly ref readonly T Max() => ref Tree.Min()->_Myval;

    /// <inheritdoc cref="IReadOnlySet{T}.Overlaps"/>
    public readonly bool Overlaps(IEnumerable<T> other) {
        CountOverlaps(other, out _, out var notFoundInSelf);
        return notFoundInSelf < Tree.LongCount;
    }

    /// <inheritdoc/>
    public bool Remove(in T item) {
        var b = Tree.FindLowerBound(item).Bound;
        if (b is null || !StdOps<T>.ContentEquals(b->_Myval, item))
            return false;
        Tree.ExtractAndErase(b);
        return true;
    }

    /// <inheritdoc cref="IReadOnlySet{T}.SetEquals"/>
    public readonly bool SetEquals(IEnumerable<T> other) {
        CountOverlaps(other, out var notFoundInOther, out var notFoundInSelf);
        return notFoundInOther == 0 && notFoundInSelf == 0;
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other) {
        var n = Tree.Min();
        if (n is null)
            return;

        var otherSet = other.ToHashSet();

        using var enumerator = GetEnumerator();
        for (var state = enumerator.MoveNext(); state;) {
            state = otherSet.Remove(enumerator.Current)
                ? enumerator.DeleteAndMoveNext()
                : enumerator.MoveNext();
        }

        foreach (var o in otherSet)
            AddCopy(o);
    }

    /// <inheritdoc/>
    public readonly T[] ToArray() {
        var res = new T[LongCount];
        var i = 0L;
        foreach (ref var v in this)
            res[i++] = v;
        return res;
    }

    /// <inheritdoc/>
    public readonly T[] ToArrayReverse() {
        var res = new T[LongCount];
        var i = 0L;
        foreach (ref var v in Reverse())
            res[i++] = v;
        return res;
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other) {
        foreach (var o in other)
            AddCopy(o);
    }

    private readonly void CountOverlaps(IEnumerable<T> other, out long notFoundInOther, out long notFoundInSelf) {
        if (other is not IReadOnlySet<T>)
            other = other.ToHashSet();

        notFoundInOther = Tree.LongCount;
        notFoundInSelf = 0L;
        foreach (var r in other) {
            if (Contains(r))
                notFoundInOther--;
            else
                notFoundInSelf++;
        }
    }

    public static int Compare(in StdSet<T, TMemorySpace> left, in StdSet<T, TMemorySpace> right) {
        using var e1 = left.GetEnumerator();
        using var e2 = right.GetEnumerator();
        while (true) {
            var b1 = e1.MoveNext();
            var b2 = e2.MoveNext();
            if (!b1 && !b2)
                return 0;
            if (b1 && !b2)
                return 1;
            if (!b1 && b2)
                return -1;
            var c = StdOps<T>.Compare(e1.Current, e2.Current);
            if (c != 0)
                return c;
        }
    }

    public static bool ContentEquals(in StdSet<T, TMemorySpace> left, in StdSet<T, TMemorySpace> right) {
        if (left.LongCount != right.LongCount)
            return false;

        using var e1 = left.GetEnumerator();
        using var e2 = right.GetEnumerator();
        while (true) {
            var b1 = e1.MoveNext();
            var b2 = e2.MoveNext();
            if (!b1 && !b2)
                return true;
            if (b1 != b2)
                return false;
            if (!StdOps<T>.ContentEquals(e1.Current, e2.Current))
                return false;
        }
    }

    public static void ConstructDefaultInPlace(out StdSet<T, TMemorySpace> item) {
        item = default;
        item.Tree.GetOrCreateHead<TMemorySpace>();
    }
    public static void StaticDispose(ref StdSet<T, TMemorySpace> item) => item.Dispose();
    public static void ConstructCopyInPlace(in StdSet<T, TMemorySpace> source, out StdSet<T, TMemorySpace> target) {
        if (!IsCopyable)
            throw new InvalidOperationException("Copying is not supported");
        ConstructDefaultInPlace(out target);
        foreach (ref var v in source)
            target.AddCopy(v);
    }
    public static void ConstructMoveInPlace(ref StdSet<T, TMemorySpace> source, out StdSet<T, TMemorySpace> target) => (target, source) = (source, default);
    public static void Swap(ref StdSet<T, TMemorySpace> item1, ref StdSet<T, TMemorySpace> item2) => (item1, item2) = (item2, item1);
}
