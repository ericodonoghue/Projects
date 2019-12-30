using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AVL
{
    /// <summary>
    /// Represents a self-balancing binary search tree using the AVL method.
    /// </summary>
    public class AVLBinarySearchTree<T>
    {
        /// <summary> Root node in the tree </summary>
        private AVLNode root;

        public delegate int Compare(T value1, T value2);
        private readonly Compare compare = Comparer<T>.Default.Compare;

        /// <summary>
        /// Creates a AVL Binary Search Tree with no nodes and uses the default Compare delegate
        /// </summary>
        public AVLBinarySearchTree()
        {
            root = null;
        }

        /// <summary>
        /// Creates a AVL Binary Search Tree with no nodes using the passed Compare delegate
        /// </summary>
        /// <param name="compare">Delegate to compare nodes</param>
        public AVLBinarySearchTree(Compare compare) : this()
        {
            this.compare = compare;
        }

        /// <summary>
        /// Adds a new node.
        /// </summary>
        /// <param name="item">element of the node to be added</param>
        /// <returns>true if the item was added, otherwise false</returns>
        public bool Add(T item)
        {
            int temp = Size(root);
            root = AddRecursion(root, item);

            // Checks if the size of the tree has changed
            return ((Size(root) - temp) != 0);
        }

        /// <summary>
        /// Recursive helper method for Add. Adds the specified item into the correct location in the tree and rebalances the tree.
        /// </summary>
        /// <param name="node">node being tested for placement</param>
        /// <param name="item">element of the node to be added</param>
        /// <returns>node to be set as the node passed into the method</returns>
        private AVLNode AddRecursion(AVLNode node, T item)
        {
            // Returns a new node to be set as the node passed into the method
            if (node == null)
                return new AVLNode(item, 0, 1);

            //  Recursively iterates through the tree until the correct place is found
            if (compare(item, node.element) < 0)
                node.left = AddRecursion(node.left, item);

            else if (compare(item, node.element) > 0)
                node.right = AddRecursion(node.right, item);

            else
                return node;

            // Updates the height and size of the current node
            node.height = (Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1);
            node.size = Size(node.left) + (Size(node.right) + 1);
            return BalanceTree(node);
        }

        /// <summary>
        /// Adds all items from the given collection
        /// </summary>
        /// <param name="items">Collection of elements to be added</param>
        /// <returns>true if all the elements from the collection were added, otherwise false</returns>
        public bool AddAll(Collection<T> items)
        {
            bool result = true;
            foreach (T current in items)
            {
                if (!result)
                    Add(current);
                else
                    result = Add(current);
            }

            return result;
        }

        /// <summary>
        /// Checks if the tree contains the given item
        /// </summary>
        /// <param name="item">element whose presence is to checked for</param>
        /// <returns>true if the element is present, otherwise false</returns>
        public bool Contains(T item)
        {
            return ContainsRecur(root, item);
        }

        /// <summary>
        /// Recursive helper method for contains.
        /// </summary>
        /// <param name="node">node to be tested for the element</param>
        /// <param name="item">element whose presence is to be tested for</param>
        /// <returns>true if the element is present, otherwise false</returns>
        private bool ContainsRecur(AVLNode node, T item)
        {
            // Iterates through the tree until the element is found or a null node is reached
            if (node == null)
                return false;

            else if (compare(item, node.element) < 0)
                return ContainsRecur(node.left, item);

            else if (compare(item, node.element) > 0)
                return ContainsRecur(node.right, item);

            else
                return true;
        }

        public bool ContainsAll(Collection<T> items)
        {
            if (Size(root) != items.Count)
                return false;

            foreach (T current in items)
            {
                if (!Contains(current))
                    return false;
            }

            return true;
        }

        public bool Remove(T item)
        {
            if (root == null)
                throw new Exception("ERROR: BinarySearchTree is empty.");

            int temp = Size(root);
            root = RemoveRecursion(root, item);
            return (Size(root) - temp) != 0;
        }

        private AVLNode RemoveRecursion(AVLNode node, T item)
        {
            if (node == null)
                throw new Exception("The given item does not exist");

            else if (compare(item, node.element) < 0)
                node.left = RemoveRecursion(node.left, item);
            
            else if (compare(item, node.element) > 0)
                node.right = RemoveRecursion(node.right, item);
            
            else if ((node.left == null))
                return node.right;
            
            else if ((node.right == null))
                return node.left;
            
            else
            {
                AVLNode temp = node;
                node = Minimum(temp.right);
                node.right = RemoveMinimum(temp.right);
                node.left = temp.left;
            }

            // Updates the height and size of the current node
            node.height = (Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1);
            node.size = Size(node.left) + (Size(node.right) + 1);
            return BalanceTree(node);
        }

        public T Minimum()
        {
            return Minimum(root).element;
        }

        private AVLNode Minimum(AVLNode node)
        {
            if (root == null)
                throw new Exception("ERROR: BinarySearchTree is empty.");

            while (node.left != null)
            {
                node = node.left;
            }

            return node;
        }

        private AVLNode RemoveMinimum(AVLNode node)
        {
            if (node.left == null)
                return node.right;
            else
                node.left = RemoveMinimum(node.left);

            node.height = Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1;
            return BalanceTree(node);
        }

        public T Maximum()
        {
            return Maximum(root).element;
        }

        private AVLNode Maximum(AVLNode node)
        {
            if (root == null)
                throw new Exception("ERROR: BinarySearchTree is empty.");

            while (node.right != null)
            {
                node = node.right;
            }

            return node;
        }

        public bool IsBalanced()
        {
            return IsBalancedRecursion(root);
        }

        private bool IsBalancedRecursion(AVLNode node)
        {
            if (node == null)         
                return true;
            
            else if ((GetBalanceFactor(node) > 1) || (GetBalanceFactor(node) < -1))
                return false;

            else
                return IsBalancedRecursion(node.left) && IsBalancedRecursion(node.right);           
        }

        public void Clear()
        {
            root = null;
        }

        public bool IsEmpty()
        {
            return root == null;
        }

        public int Size()
        {
            return Size(root);
        }

        private int Size(AVLNode node)
        {
            if (node == null)
                return 0;
            else
                return node.size;
        }

        public ArrayList ToArrayList()
        {
            ArrayList arrList = new ArrayList();
            ToArrayListRecur(root, arrList);
            return arrList;
        }

        private void ToArrayListRecur(AVLNode node, ArrayList arrList)
        {
            if (node == null)
                return;         

            ToArrayListRecur(node.left, arrList);
            arrList.Add(node.element);
            ToArrayListRecur(node.right, arrList);
        }

        private AVLNode BalanceTree(AVLNode node)
        {
            if (GetBalanceFactor(node) > 1)
            {
                if (GetBalanceFactor(node.left) < 0)
                    node.left = LeftRotate(node.left);               

                node = RightRotate(node);
            }
            else if (GetBalanceFactor(node) < -1)
            {
                if (GetBalanceFactor(node.right) > 0)
                    node.right = RightRotate(node.right);

                node = LeftRotate(node);
            }

            return node;
        }

        private AVLNode LeftRotate(AVLNode node)
        {
            AVLNode temp = node.right;
            node.right = temp.left;
            temp.left = node;
            node.height = Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1;
            temp.height = Math.Max(GetHeight(temp.left), GetHeight(temp.right)) + 1;
            temp.size = node.size;
            node.size = Size(node.left) + (Size(node.right) + 1);
            return temp;
        }

        private AVLNode RightRotate(AVLNode node)
        {
            AVLNode temp = node.left;
            node.left = temp.right;
            temp.right = node;
            node.height = Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1;
            temp.height = Math.Max(GetHeight(temp.left), GetHeight(temp.right)) + 1;
            temp.size = node.size;
            node.size = Size(node.left) + (Size(node.right) + 1);
            return temp;
        }

        private int GetHeight(AVLNode node)
        {
            if (node == null)
                return -1;

            return node.height;
        }

        private int GetBalanceFactor(AVLNode node)
        {
            return GetHeight(node.left) - GetHeight(node.right);
        }

        internal class AVLNode
        {
            internal T element;

            internal int height;

            internal int size;

            internal AVLNode left;

            internal AVLNode right;

            public AVLNode(T element, int height, int size)
            {
                this.element = element;
                this.height = height;
                this.size = size;
            }
        }
    }    
}
