using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace FFXIVClientStructs.STD.Helper;

/// <summary>
/// Colors for link to parent. 
/// </summary>
public enum RedBlackTreeNodeColor : byte {
    Red,
    Black,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct RedBlackTree<T, TKey, TKeyExtractor>
    : IEquatable<RedBlackTree<T, TKey, TKeyExtractor>>
    where T : unmanaged
    where TKeyExtractor : IStaticKeyExtractor<T, TKey>
    where TKey : unmanaged {
    public Node* Head;
    public long LongCount;

    public readonly RedBlackTree<T, TKey, TKeyExtractor>* Pointer =>
        (RedBlackTree<T, TKey, TKeyExtractor>*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));

    /// <summary>
    /// Gets the rightmost node in subtree at <paramref name="node"/>.
    /// </summary>
    public static Node* Max(Node* node) {
        while (node is not null && !node->_Right->_Isnil)
            node = node->_Right;
        return node;
    }

    /// <summary>
    /// Gets the leftmost node in subtree at <paramref name="node"/>.
    /// </summary>
    public static Node* Min(Node* node) {
        while (node is not null && !node->_Left->_Isnil)
            node = node->_Left;
        return node;
    }

    public readonly bool Equals(in RedBlackTree<T, TKey, TKeyExtractor> other) => Head == other.Head && LongCount == other.LongCount;
    readonly bool IEquatable<RedBlackTree<T, TKey, TKeyExtractor>>.Equals(RedBlackTree<T, TKey, TKeyExtractor> other) => Equals(other);
    public readonly override bool Equals(object? obj) => obj is RedBlackTree<T, TKey, TKeyExtractor> t && Equals(t);

    public void EraseHead() {
        if (Head is null)
            return;
        EraseTree(Head->_Parent);
        Node.FreeNode(Head);
        Head = null;
        LongCount = 0;
    }

    public void EraseTree(Node* where) {
        while (!where->_Isnil) {
            EraseTree(where->_Right);
            var w = where;
            var wl = where->_Left;
            where->_Left = where;
            where = wl;
            Node.FreeNode(w);
        }
    }

    public Node* Extract(Node* where) {
        var erasedNode = where; // node to erase
        where = where->Next(); // save successor iterator for return

        Node* fixNode; // the node to recolor as needed
        Node* fixNodeParent; // parent of fixNode (which may be nil)
        var pNode = erasedNode;

        if (pNode->_Left->_Isnil) {
            fixNode = pNode->_Right; // stitch up right subtree
        } else if (pNode->_Right->_Isnil) {
            fixNode = pNode->_Left; // stitch up left subtree
        } else {
            // two subtrees, must lift successor node to replace erased
            pNode = where; // _Pnode is successor node
            fixNode = pNode->_Right; // fixNode is only subtree
        }

        if (pNode == erasedNode) {
            // at most one subtree, relink it
            fixNodeParent = erasedNode->_Parent;
            if (!fixNode->_Isnil) {
                fixNode->_Parent = fixNodeParent; // link up
            }

            if (Head->_Parent == erasedNode) {
                Head->_Parent = fixNode; // link down from root
            } else if (fixNodeParent->_Left == erasedNode) {
                fixNodeParent->_Left = fixNode; // link down to left
            } else {
                fixNodeParent->_Right = fixNode; // link down to right
            }

            if (Head->_Left == erasedNode) {
                Head->_Left = fixNode->_Isnil
                    ? fixNodeParent // smallest is parent of erased node
                    : Min(fixNode); // smallest in relinked subtree
            }

            if (Head->_Right == erasedNode) {
                Head->_Right = fixNode->_Isnil
                    ? fixNodeParent // largest is parent of erased node
                    : Max(fixNode); // largest in relinked subtree
            }
        } else {
            // erased has two subtrees, _Pnode is successor to erased
            erasedNode->_Left->_Parent = pNode; // link left up
            pNode->_Left = erasedNode->_Left; // link successor down

            if (pNode == erasedNode->_Right) {
                fixNodeParent = pNode; // successor is next to erased
            } else {
                // successor further down, link in place of erased
                fixNodeParent = pNode->_Parent; // parent is successor's
                if (!fixNode->_Isnil) {
                    fixNode->_Parent = fixNodeParent; // link fix up
                }

                fixNodeParent->_Left = fixNode; // link fix down
                pNode->_Right = erasedNode->_Right; // link next down
                erasedNode->_Right->_Parent = pNode; // right up
            }

            if (Head->_Parent == erasedNode) {
                Head->_Parent = pNode; // link down from root
            } else if (erasedNode->_Parent->_Left == erasedNode) {
                erasedNode->_Parent->_Left = pNode; // link down to left
            } else {
                erasedNode->_Parent->_Right = pNode; // link down to right
            }

            pNode->_Parent = erasedNode->_Parent; // link successor up
            (pNode->_Color, erasedNode->_Color) = (erasedNode->_Color, pNode->_Color); // recolor it
        }

        if (erasedNode->_Color == RedBlackTreeNodeColor.Black) {
            // erasing black link, must recolor/rebalance tree
            for (; fixNode != Head->_Parent && fixNode->_Color == RedBlackTreeNodeColor.Black; fixNodeParent = fixNode->_Parent) {
                if (fixNode == fixNodeParent->_Left) {
                    // fixup left subtree
                    pNode = fixNodeParent->_Right;
                    if (pNode->_Color == RedBlackTreeNodeColor.Red) {
                        // rotate red up from right subtree
                        pNode->_Color = RedBlackTreeNodeColor.Black;
                        fixNodeParent->_Color = RedBlackTreeNodeColor.Red;
                        RotateLeft(fixNodeParent);
                        pNode = fixNodeParent->_Right;
                    }

                    if (pNode->_Isnil) {
                        fixNode = fixNodeParent; // shouldn't happen
                    } else if (pNode->_Left->_Color == RedBlackTreeNodeColor.Black
                               && pNode->_Right->_Color == RedBlackTreeNodeColor.Black) {
                        // redden right subtree with black children
                        pNode->_Color = RedBlackTreeNodeColor.Red;
                        fixNode = fixNodeParent;
                    } else {
                        // must rearrange right subtree
                        if (pNode->_Right->_Color == RedBlackTreeNodeColor.Black) {
                            // rotate red up from left sub-subtree
                            pNode->_Left->_Color = RedBlackTreeNodeColor.Black;
                            pNode->_Color = RedBlackTreeNodeColor.Red;
                            RotateRight(pNode);
                            pNode = fixNodeParent->_Right;
                        }

                        pNode->_Color = fixNodeParent->_Color;
                        fixNodeParent->_Color = RedBlackTreeNodeColor.Black;
                        pNode->_Right->_Color = RedBlackTreeNodeColor.Black;
                        RotateLeft(fixNodeParent);
                        break; // tree now recolored/rebalanced
                    }
                } else {
                    // fixup right subtree
                    pNode = fixNodeParent->_Left;
                    if (pNode->_Color == RedBlackTreeNodeColor.Red) {
                        // rotate red up from left subtree
                        pNode->_Color = RedBlackTreeNodeColor.Black;
                        fixNodeParent->_Color = RedBlackTreeNodeColor.Red;
                        RotateRight(fixNodeParent);
                        pNode = fixNodeParent->_Left;
                    }

                    if (pNode->_Isnil) {
                        fixNode = fixNodeParent; // shouldn't happen
                    } else if (pNode->_Right->_Color == RedBlackTreeNodeColor.Black
                               && pNode->_Left->_Color == RedBlackTreeNodeColor.Black) {
                        // redden left subtree with black children
                        pNode->_Color = RedBlackTreeNodeColor.Red;
                        fixNode = fixNodeParent;
                    } else {
                        // must rearrange left subtree
                        if (pNode->_Left->_Color == RedBlackTreeNodeColor.Black) {
                            // rotate red up from right sub-subtree
                            pNode->_Right->_Color = RedBlackTreeNodeColor.Black;
                            pNode->_Color = RedBlackTreeNodeColor.Red;
                            RotateLeft(pNode);
                            pNode = fixNodeParent->_Left;
                        }

                        pNode->_Color = fixNodeParent->_Color;
                        fixNodeParent->_Color = RedBlackTreeNodeColor.Black;
                        pNode->_Left->_Color = RedBlackTreeNodeColor.Black;
                        RotateRight(fixNodeParent);
                        break; // tree now recolored/rebalanced
                    }
                }
            }

            fixNode->_Color = RedBlackTreeNodeColor.Black; // stopping node is black
        }

        if (0 < LongCount) {
            --LongCount;
        }

        return erasedNode;
    }

    public void ExtractAndErase(Node* node) => Node.FreeNode(Extract(node));

    public readonly FindResult FindUpperBound(in TKey key) {
        if (Head is null)
            return default;
        var result = new FindResult { Location = new TreeId { Parent = Head->_Parent, Child = TreeChild.Right }, Bound = Head };
        var tryNode = result.Location.Parent;
        while (!tryNode->_Isnil) {
            result.Location.Parent = tryNode;
            if (StdOps<TKey>.Compare(key, TKeyExtractor.ExtractKey(tryNode->_Myval)) < 0) {
                result.Location.Child = TreeChild.Left;
                result.Bound = tryNode;
                tryNode = tryNode->_Left;
            } else {
                result.Location.Child = TreeChild.Right;
                tryNode = tryNode->_Right;
            }
        }

        return result;
    }

    public readonly FindResult FindLowerBound(in TKey key) {
        if (Head is null)
            return default;
        var result = new FindResult { Location = new TreeId { Parent = Head->_Parent, Child = TreeChild.Right }, Bound = Head };
        var tryNode = result.Location.Parent;
        while (!tryNode->_Isnil) {
            result.Location.Parent = tryNode;
            if (StdOps<TKey>.Compare(key, TKeyExtractor.ExtractKey(tryNode->_Myval)) <= 0) {
                result.Location.Child = TreeChild.Left;
                result.Bound = tryNode;
                tryNode = tryNode->_Left;
            } else {
                result.Location.Child = TreeChild.Right;
                tryNode = tryNode->_Right;
            }
        }

        return result;
    }

    public readonly override int GetHashCode() => HashCode.Combine((nint)Head, LongCount);

    public Node* GetOrCreateHead<TMemorySpace>()
        where TMemorySpace : IStaticMemorySpace {
        if (Head is null)
            Head = Node.BuyHeadNode<TMemorySpace>();
        return Head;
    }

    public Node* Insert(TreeId loc, Node* newNode) {
        ++LongCount;
        var head = Head;
        newNode->_Parent = loc.Parent;
        if (loc.Parent == head) {
            // first node in tree, just set head values
            head->_Left = newNode;
            head->_Parent = newNode;
            head->_Right = newNode;
            newNode->_Color = RedBlackTreeNodeColor.Black; // the root is black
            return newNode;
        }

        if (loc.Child == TreeChild.Right) {
            // add to right of _Loc._Parent
            loc.Parent->_Right = newNode;
            if (loc.Parent == head->_Right) {
                // remember rightmost node
                head->_Right = newNode;
            }
        } else {
            // add to left of _Loc._Parent
            loc.Parent->_Left = newNode;
            if (loc.Parent == head->_Left) {
                // remember leftmost node
                head->_Left = newNode;
            }
        }

        for (var pNode = newNode; pNode->_Parent->_Color == RedBlackTreeNodeColor.Red;) {
            if (pNode->_Parent == pNode->_Parent->_Parent->_Left) {
                // fixup red-red in left subtree
                var parentSibling = pNode->_Parent->_Parent->_Right;
                if (parentSibling->_Color == RedBlackTreeNodeColor.Red) {
                    // parent's sibling has two red children, blacken both
                    pNode->_Parent->_Color = RedBlackTreeNodeColor.Black;
                    parentSibling->_Color = RedBlackTreeNodeColor.Black;
                    pNode->_Parent->_Parent->_Color = RedBlackTreeNodeColor.Red;
                    pNode = pNode->_Parent->_Parent;
                } else {
                    // parent's sibling has red and black children
                    if (pNode == pNode->_Parent->_Right) {
                        // rotate right child to left
                        pNode = pNode->_Parent;
                        RotateLeft(pNode);
                    }

                    pNode->_Parent->_Color = RedBlackTreeNodeColor.Black; // propagate red up
                    pNode->_Parent->_Parent->_Color = RedBlackTreeNodeColor.Red;
                    RotateRight(pNode->_Parent->_Parent);
                }
            } else {
                // fixup red-red in right subtree
                var parentSibling = pNode->_Parent->_Parent->_Left;
                if (parentSibling->_Color == RedBlackTreeNodeColor.Red) {
                    // parent's sibling has two red children, blacken both
                    pNode->_Parent->_Color = RedBlackTreeNodeColor.Black;
                    parentSibling->_Color = RedBlackTreeNodeColor.Black;
                    pNode->_Parent->_Parent->_Color = RedBlackTreeNodeColor.Red;
                    pNode = pNode->_Parent->_Parent;
                } else {
                    // parent's sibling has red and black children
                    if (pNode == pNode->_Parent->_Left) {
                        // rotate left child to right
                        pNode = pNode->_Parent;
                        RotateRight(pNode);
                    }

                    pNode->_Parent->_Color = RedBlackTreeNodeColor.Black; // propagate red up
                    pNode->_Parent->_Parent->_Color = RedBlackTreeNodeColor.Red;
                    RotateLeft(pNode->_Parent->_Parent);
                }
            }
        }

        head->_Parent->_Color = RedBlackTreeNodeColor.Black; // root is always black
        return newNode;
    }

    public readonly Node* Max() => Max(Head);

    public readonly Node* Min() => Min(Head);

    /// <summary>
    /// Promotes the right node to the root of subtree.
    /// </summary>
    public void RotateLeft(Node* wherenode) {
        var promotingNode = wherenode->_Right;
        wherenode->_Right = promotingNode->_Left;

        if (!promotingNode->_Left->_Isnil)
            promotingNode->_Left->_Parent = wherenode;

        promotingNode->_Parent = wherenode->_Parent;

        if (wherenode == Head->_Parent)
            Head->_Parent = promotingNode;
        else if (wherenode == wherenode->_Parent->_Left)
            wherenode->_Parent->_Left = promotingNode;
        else
            wherenode->_Parent->_Right = promotingNode;

        promotingNode->_Left = wherenode;
        wherenode->_Parent = promotingNode;
    }

    /// <summary>
    /// Promote the left node to the root of subtree.
    /// </summary>
    public void RotateRight(Node* wherenode) {
        var promotingNode = wherenode->_Left;
        wherenode->_Left = promotingNode->_Right;

        if (!promotingNode->_Right->_Isnil)
            promotingNode->_Right->_Parent = wherenode;

        promotingNode->_Parent = wherenode->_Parent;

        if (wherenode == Head->_Parent)
            Head->_Parent = promotingNode;
        else if (wherenode == wherenode->_Parent->_Right)
            wherenode->_Parent->_Right = promotingNode;
        else
            wherenode->_Parent->_Left = promotingNode;

        promotingNode->_Right = wherenode;
        wherenode->_Parent = promotingNode;
    }

    public Node* InsertEmpty<TMemorySpace>(FindResult loc)
        where TMemorySpace : IStaticMemorySpace {
        if (loc.Location.Parent is null)
            loc.Location = new TreeId { Parent = GetOrCreateHead<TMemorySpace>()->_Parent, Child = TreeChild.Right };
        return Insert(loc.Location, Node.BuyNode<TMemorySpace>(Head));
    }

    public bool TryInsertEmpty<TMemorySpace>(in TKey key, out Node* node)
        where TMemorySpace : IStaticMemorySpace {
        var loc = FindLowerBound(key);
        if (loc.KeyEquals(key)) {
            node = loc.Bound;
            return false;
        }

        node = InsertEmpty<TMemorySpace>(loc);
        return true;
    }

    public enum TreeChild {
        Right,
        Left,
        Unused,
    }

    public struct TreeId {
        public Node* Parent;
        public TreeChild Child;
    }

    public struct FindResult {
        public TreeId Location;
        public Node* Bound;

        public readonly bool KeyEquals(in TKey other) {
            if (Bound is null || Bound->_Isnil)
                return false;
            return StdOps<TKey>.ContentEquals(TKeyExtractor.ExtractKey(Bound->_Myval), other);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Node {
        public Node* _Left;
        public Node* _Parent;
        public Node* _Right;
        public RedBlackTreeNodeColor _Color;
        public bool _Isnil;
        public T _Myval;

        public static Node* BuyHeadNode<TMemorySpace>()
            where TMemorySpace : IStaticMemorySpace {
            var n = (Node*)TMemorySpace.Allocate((nuint)sizeof(Node), 0x10);
            if (n == null)
                throw new OutOfMemoryException();
            *n = default;
            n->_Color = RedBlackTreeNodeColor.Black;
            n->_Left = n->_Parent = n->_Right = n;
            n->_Isnil = true;
            return n;
        }

        public static Node* BuyNode<TMemorySpace>(Node* head)
            where TMemorySpace : IStaticMemorySpace {
            var n = (Node*)TMemorySpace.Allocate((nuint)sizeof(Node), 0x10);
            if (n == null)
                throw new OutOfMemoryException();
            *n = default;
            n->_Color = RedBlackTreeNodeColor.Red;
            n->_Left = n->_Parent = n->_Right = head;
            n->_Isnil = false;

            return n;
        }

        public readonly Node* Next() {
            Debug.Assert(!_Isnil, "Tried to increment a head node.");
            if (_Right->_Isnil) {
                fixed (Node* thisPtr = &this) {
                    var ptr = thisPtr;
                    Node* node;
                    while (!(node = ptr->_Parent)->_Isnil && ptr == node->_Right)
                        ptr = node;

                    return node;
                }
            }

            return Min(_Right);
        }

        public readonly Node* Prev() {
            if (_Isnil) return _Right;

            if (_Left->_Isnil) {
                fixed (Node* thisPtr = &this) {
                    var ptr = thisPtr;
                    Node* node;
                    while (!(node = ptr->_Parent)->_Isnil && ptr == node->_Left)
                        ptr = node;

                    return ptr->_Isnil ? ptr : node;
                }
            }

            return Max(_Left);
        }

        public static void FreeNode(Node* node) {
            if (!node->_Isnil)
                StdOps<T>.StaticDispose(ref node->_Myval);
            IMemorySpace.Free(node);
        }
    }

    public struct Enumerator : IEnumerable<T>, IEnumerator<T> {
        private readonly RedBlackTree<T, TKey, TKeyExtractor>* _owner;
        private readonly bool _ltr;
        private Node* _current;

        internal Enumerator(RedBlackTree<T, TKey, TKeyExtractor>* owner, bool ltr) {
            _owner = owner;
            _ltr = ltr;
            Reset();
        }

        public readonly ref T Current => ref _current->_Myval;

        readonly object IEnumerator.Current => Current;

        readonly T IEnumerator<T>.Current => Current;

        public bool MoveNext() {
            if (_owner->Head == null || (_current is not null && _current == _owner->Head))
                return false;

            if (_ltr)
                _current = _current == null ? _owner->Head->_Left : _current->Next();
            else
                _current = _current == null ? _owner->Head->_Right : _current->Prev();

            return !_current->_Isnil;
        }

        public bool DeleteAndMoveNext() {
            var what = _current;
            if (what is null || what->_Isnil)
                return false;

            var next = MoveNext();
            _owner->ExtractAndErase(what);
            return next;
        }

        public void Reset() => _current = null;

        public void Dispose() {
        }

        public readonly Enumerator GetEnumerator() => new(_owner, _ltr);

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    }
}
