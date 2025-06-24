using TwoThreeFourTree;

namespace TwoThreeFourTree
{
    using BSTforRBTree;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Linq;

    public class TwoThreeFourTree<T> where T : IComparable
    {
        private class Node
        {
            public List<T> Keys = new List<T>();
            public List<Node> Children = new List<Node>();

            public bool IsLeaf => Children.Count == 0;
            public bool IsFull => Keys.Count == 3;
            public bool CanLend => Keys.Count > 1;
        }

        private Node root;

        public TwoThreeFourTree()
        {
            root = null;
        }

        public bool Insert(T k)
        {
            // If there's no root, make one
            if (root == null)
            {
                root = new Node { Keys = new List<T> { k } };
                return true;
            }

            Node curr = root;
            Node parent = null;

            while (curr != null)
            {
                // It is unlikely for curr.Keys to be null given the structure of the Node class,
                // but we add a check here to prevent the NullReferenceException.
                if (curr.Keys == null)
                {
                    throw new InvalidOperationException("Current node's keys are null.");
                }

                // Split the node if it's full
                if (curr.IsFull)
                {
                    Split(parent, curr);
                    // After splitting, determine where to go next.
                    // If the key to insert is greater than the last key of the parent (after split), go right; otherwise, go left.
                    if (parent != null)
                    {
                        // Assuming the last key of the parent is the one that has been moved up from the middle of the current node
                        if (k.CompareTo(parent.Keys.Last()) > 0)
                        {
                            // If the key is greater than the last key of the parent, go to the right child of the parent
                            curr = parent.Children[parent.Children.FindIndex(x => x == curr) + 1];
                        }
                        else
                        {
                            // Otherwise, continue with the left child (which is the current node after the split)
                            curr = parent.Children[parent.Children.FindIndex(x => x == curr)];
                        }
                    }
                    else
                    {
                        // If the split node was the root, then the new root is now the current node
                        curr = root;
                    }
                    // Continue to next iteration of the loop
                    continue;
                }

                int pos = curr.Keys.FindIndex(x => k.CompareTo(x) < 0);

                // When we've reached a leaf node
                if (curr.IsLeaf)
                {
                    if (pos == -1)
                    {
                        curr.Keys.Add(k);
                    }
                    else
                    {
                        curr.Keys.Insert(pos, k);
                    }
                    return true;
                }

                // Not a leaf node, move to the appropriate child
                parent = curr;
                curr = (pos == -1) ? curr.Children[curr.Children.Count - 1] : curr.Children[pos];
            }
            return false; // Should never reach here if the tree is correctly maintained
        }

        // Split logic for 2-3-4 tree node
        private void Split(Node parent, Node child)
        {
            Node sibling = new Node();

            // Split the keys
            int midIndex = child.Keys.Count / 2;
            T midKey = child.Keys[midIndex];

            sibling.Keys.AddRange(child.Keys.GetRange(midIndex + 1, child.Keys.Count - midIndex - 1));
            child.Keys.RemoveRange(midIndex, child.Keys.Count - midIndex);

            // Split the children if the current node is not a leaf
            if (!child.IsLeaf)
            {
                sibling.Children.AddRange(child.Children.GetRange(midIndex + 1, child.Children.Count - midIndex - 1));
                child.Children.RemoveRange(midIndex + 1, child.Children.Count - midIndex - 1);
            }

            // If there's a parent, insert the median key into it
            if (parent != null)
            {
                int pos = parent.Keys.FindIndex(x => midKey.CompareTo(x) < 0);
                if (pos == -1)
                    parent.Keys.Add(midKey);
                else
                    parent.Keys.Insert(pos, midKey);

                // Insert the sibling into the parent's children list
                pos = parent.Children.FindIndex(x => child == x);
                parent.Children.Insert(pos + 1, sibling);
            }
            else // If the current node is the root, create a new root
            {
                Node newRoot = new Node();
                newRoot.Keys.Add(midKey);
                newRoot.Children.Add(child);
                newRoot.Children.Add(sibling);
                root = newRoot;
            }

        }

        public bool Delete(T k)
        {
            if (root == null)
                return false; // Tree is empty

            Node parent = null;
            Node curr = root;
            bool found = false;

            while (curr != null)
            {
                int pos = curr.Keys.FindIndex(x => k.CompareTo(x) == 0);

                if (pos != -1) // Key found in the current node
                {
                    found = true;

                    if (curr.IsLeaf)
                    {
                        curr.Keys.RemoveAt(pos);
                        return true;
                    }
                    else
                    {
                        // Replace the key with its predecessor in the subtree rooted at curr
                        Node predParent = curr;
                        Node pred = curr.Children[pos];
                        while (!pred.IsLeaf)
                        {
                            predParent = pred;
                            pred = pred.Children.Last();
                        }
                        curr.Keys[pos] = pred.Keys.Last();

                        // Now remove the predecessor key from its leaf node
                        return Remove(pred.Keys.Last(), predParent);
                    }
                }
                else // Key not found in the current node
                {
                    parent = curr;
                    pos = curr.Keys.FindIndex(x => k.CompareTo(x) < 0);

                    if (pos == -1)
                        curr = curr.Children.Last();
                    else
                        curr = curr.Children[pos];
                }
            }

            return found;
        }

        private bool Remove(T k, Node parent)
        {
            int pos = parent.Keys.FindIndex(x => k.CompareTo(x) == 0);
            if (pos != -1)
            {
                parent.Keys.RemoveAt(pos);
                return true;
            }
            else
            {
                pos = parent.Keys.FindIndex(x => k.CompareTo(x) < 0);
                Node child = parent.Children[pos];
                return Remove(k, child);
            }
        }

        public bool Search(T k)
        {
            try
            {
                Node curr = root;
                //while the pointer is not null
                while (curr != null)
                {
                    int pos = curr.Keys.FindIndex(x => k.CompareTo(x) == 0);
                    if (pos != -1)
                        return true; // Found

                    pos = curr.Keys.FindIndex(x => k.CompareTo(x) < 0);
                    curr = (pos == -1) ? curr.Children[curr.Children.Count - 1] : curr.Children[pos];
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine(k + " was not found");
            }
            return false; // Not found
        }



        public BSTforRBTree<T> Convert()
        {
            BSTforRBTree<T> rbTree = new BSTforRBTree<T>();
            if (root != null)
            {
                ConvertNode(root, rbTree, Color.BLACK, null, true);
            }
            return rbTree;
        }


        private void ConvertNode(Node node, BSTforRBTree<T> rbTree, Color parentColor, Node parentNode, bool isLeftChild)
        {
            if (node == null) return;

            Color nodeColor = parentNode == null ? Color.BLACK : parentColor;

            rbTree.Add(node.Keys[0], nodeColor);

            if (node.Keys.Count > 1)
            {
                rbTree.Add(node.Keys[1], parentColor == Color.RED ? Color.BLACK : Color.RED);
            }

            if (node.Keys.Count > 2)
            {
                rbTree.Add(node.Keys[2], parentColor == Color.RED ? Color.BLACK : Color.RED);
            }

            if (node.Children.Count > 0)
            {
                ConvertNode(node.Children.ElementAtOrDefault(0), rbTree, nodeColor, node, true);
            }
            if (node.Children.Count > 1)
            {
                ConvertNode(node.Children.ElementAtOrDefault(1), rbTree, Color.BLACK, node, false);
            }
            if (node.Children.Count > 2)
            {
                ConvertNode(node.Children.ElementAtOrDefault(2), rbTree, nodeColor, node, false);
            }
            if (node.Children.Count > 3)
            {
                ConvertNode(node.Children.ElementAtOrDefault(3), rbTree, Color.BLACK, node, false);
            }
        }



        public void Print()
        {
            Print(root, 0);
            Console.WriteLine();
        }

        private void Print(Node node, int space)
        {
            // Base case
            if (node == null)
                return;

            // Increase distance between levels
            space += 5;

            // Process right children first
            if (node.Children.Count > 2)
                Print(node.Children[2], space); // Child 3 for a 4-node

            if (node.Children.Count > 1)
                Print(node.Children[1], space); // Child 2 for a 3-node or 4-node

            // Print current node after space count
            Console.Write("\n");
            for (int i = 5; i < space; i++)
                Console.Write(" ");
            for (int i = 0; i < node.Keys.Count; i++)
                Console.Write(node.Keys[i] + " ");
            Console.Write("\n");

            // Process left children
            Print(node.Children.FirstOrDefault(), space); // Child 1 for 2-node, 3-node, or 4-node

            if (node.Children.Count > 3)
                Print(node.Children[3], space); // Child 4 for a 4-node
        }


    }
}
    namespace BSTforRBTree
{
    public enum Color { RED, BLACK };       // Colors of the red-black tree

    public interface ISearchable<T>
    {
        void Add(T item, Color rb);
        void Print();
    }

    //-------------------------------------------------------------------------

    // Implementation:  BSTforRBTree

    public class BSTforRBTree<T> : ISearchable<T> where T : IComparable
    {

        // Common generic node class for a BSTforRBTree

        private class Node
        {
            // Read/write properties

            public T Item;
            public Color RB;
            public Node Left;
            public Node Right;

            public Node(T item, Color rb)
            {
                Item = item;
                RB = rb;
                Left = Right = null;
            }
        }

        private Node root;

        public BSTforRBTree()
        {
            root = null;    // Empty BSTforRBTree
        }

        // Add 
        // Insert an item into a BSTforRBTRee
        // Duplicate items are not inserted
        // Worst case time complexity:  O(log n) 
        // since the maximum depth of a red-black tree is O(log n)

        public void Add(T item, Color rb)
        {
            Node curr;
            bool inserted = false;

            if (root == null)
                root = new Node(item, rb);   // Create a root
            else
            {
                curr = root;
                while (!inserted)
                {
                    if (item.CompareTo(curr.Item) < 0)
                    {
                        if (curr.Left == null)              // Empty spot
                        {
                            curr.Left = new Node(item, rb);
                            inserted = true;
                        }
                        else
                            curr = curr.Left;               // Move left
                    }
                    else
                        if (item.CompareTo(curr.Item) > 0)
                    {
                        if (curr.Right == null)         // Empty spot
                        {
                            curr.Right = new Node(item, rb);
                            inserted = true;
                        }
                        else
                            curr = curr.Right;          // Move right
                    }
                    else
                        inserted = true;                // Already inserted
                }
            }
        }

        public void Print()
        {
            Print(root, 0);                // Call private, recursive Print
            Console.WriteLine();
        }

        // Print
        // Inorder traversal of the BSTforRBTree
        // Time complexity:  O(n)

        private void Print(Node node, int k)
        {
            string s;
            string t = new string(' ', k);

            if (node != null)
            {
                Print(node.Right, k + 4);
                s = node.RB == Color.RED ? "R" : "B";
                Console.WriteLine(t + node.Item.ToString() + s);
                Print(node.Left, k + 4);
            }
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        var tree = new TwoThreeFourTree<int>();

        int[] keysToInsert = { 50, 40, 60, 30, 70, 20, 80 };
        foreach (var key in keysToInsert)
        {
            Console.Write(key + " ");
            tree.Insert(key);
        }
        Console.WriteLine("\nTree after insertions:");
        tree.Print();

        Console.WriteLine("\nSearching for 60 and 15 in the tree:");
        Console.WriteLine("Search 60: " + tree.Search(60));
        Console.WriteLine("Search 15: " + tree.Search(15));

        Console.WriteLine("\nDeleting 20 and 60 from the tree.");
        tree.Delete(20);
        tree.Delete(60);
        Console.WriteLine("Tree after deletions:");
        tree.Print();

        Console.WriteLine("\nConverting 2-3-4 tree to Red-Black tree:");
        var rbTree = tree.Convert();
        Console.WriteLine("Red-Black tree:");
        rbTree.Print();

    }
}