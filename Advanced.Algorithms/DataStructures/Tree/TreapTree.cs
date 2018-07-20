﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Advanced.Algorithms.DataStructures
{
    internal class TreapTreeNode<T> : BSTNodeBase<T> where T : IComparable
    {
        internal new TreapTreeNode<T> Parent
        {
            get { return (TreapTreeNode<T>)base.Parent; }
            set { base.Parent = value; }
        }

        internal new TreapTreeNode<T> Left
        {
            get { return (TreapTreeNode<T>)base.Left; }
            set { base.Left = value; }
        }

        internal new TreapTreeNode<T> Right
        {
            get { return (TreapTreeNode<T>)base.Right; }
            set { base.Right = value; }
        }

        internal int Priority { get; set; }

        internal TreapTreeNode(TreapTreeNode<T> parent, T value, int priority)
        {
            Parent = parent;
            Value = value;
            Priority = priority;
        }
    }

    //TODO support initial  bulk loading if possible
    //TODO make sure duplicates are handled correctly if its not already
    public class TreapTree<T> : IEnumerable<T> where T : IComparable
    {
        private Random rndGenerator = new Random();
        internal TreapTreeNode<T> Root { get; set; }
        public int Count { get; private set; }

        //O(log(n)) worst O(n) for unbalanced tree
        public bool HasItem(T value)
        {
            if (Root == null)
            {
                return false;
            }

            return find(Root, value) != null;
        }

        //O(log(n)) worst O(n) for unbalanced tree
        public int GetHeight()
        {
            return getHeight(Root);
        }

        //O(log(n)) worst O(n) for unbalanced tree
        private int getHeight(TreapTreeNode<T> node)
        {
            if (node == null)
            {
                return -1;
            }

            return Math.Max(getHeight(node.Left), getHeight(node.Right)) + 1;
        }

        //O(log(n)) worst O(n) for unbalanced tree
        public void Insert(T value)
        {
            if (Root == null)
            {
                Root = new TreapTreeNode<T>(null, value, rndGenerator.Next());
                Count++;
                return;
            }

            var newNode = insert(Root, value);

            heapify(newNode);
            Count++;
        }

        //O(log(n)) always
        private TreapTreeNode<T> insert(TreapTreeNode<T> currentNode, T newNodeValue)
        {
            while (true)
            {
                var compareResult = currentNode.Value.CompareTo(newNodeValue);

                //current node is less than new item
                if (compareResult < 0)
                {
                    //no right child
                    if (currentNode.Right == null)
                    {
                        //insert
                        currentNode.Right = new TreapTreeNode<T>(currentNode, newNodeValue, rndGenerator.Next());
                        return currentNode.Right;
                    }

                    currentNode = currentNode.Right;
                }
                //current node is greater than new node
                else if (compareResult > 0)
                {
                    if (currentNode.Left == null)
                    {
                        //insert
                        currentNode.Left = new TreapTreeNode<T>(currentNode, newNodeValue, rndGenerator.Next());
                        return currentNode.Left;
                    }

                    currentNode = currentNode.Left;
                }
                else
                {
                    throw new Exception("Item exists");
                }
            }
        }

        //remove the node with the given identifier from the descendants 
        //O(log(n)) worst O(n) for unbalanced tree
        public void Delete(T value)
        {
            if (Root == null)
            {
                throw new Exception("Empty TreapTree");
            }

            delete(Root, value);
            Count--;
        }

        //O(log(n)) worst O(n) for unbalanced tree
        private void delete(TreapTreeNode<T> node, T value)
        {
            while (true)
            {
                if (node != null)
                {
                    var compareResult = node.Value.CompareTo(value);

                    //node is less than the search value so move right to find the deletion node
                    if (compareResult < 0)
                    {
                        node = node.Right ?? throw new Exception("Item do not exist");
                        continue;
                    }
                    //node is less than the search value so move left to find the deletion node

                    if (compareResult > 0)
                    {
                        node = node.Left ?? throw new Exception("Item do not exist");
                        continue;
                    }
                }

                //node is a leaf node
                if (node != null && node.IsLeaf)
                {
                    deleteLeaf(node);
                }
                else
                {
                    //case one - right tree is null (move sub tree up)
                    if (node?.Left != null && node.Right == null)
                    {
                        deleteLeftNode(node);
                    }
                    //case two - left tree is null  (move sub tree up)
                    else if (node?.Right != null && node.Left == null)
                    {
                        deleteRightNode(node);
                    }
                    //case three - two child trees 
                    //replace the node value with maximum element of left subtree (left max node)
                    //and then delete the left max node
                    else
                    {
                        if (node != null)
                        {
                            var maxLeftNode = findMax(node.Left);

                            node.Value = maxLeftNode.Value;

                            //delete left max node
                            node = node.Left;
                            value = maxLeftNode.Value;
                        }

                        continue;
                    }
                }

                break;
            }
        }

        private void deleteLeaf(TreapTreeNode<T> node)
        {
            //if node is root
            if (node.Parent == null)
            {
                Root = null;
            }
            //assign nodes parent.left/right to null
            else if (node.IsLeftChild)
            {
                node.Parent.Left = null;
            }
            else
            {
                node.Parent.Right = null;
            }
        }

        private void deleteRightNode(TreapTreeNode<T> node)
        {
            //root
            if (node.Parent == null)
            {
                Root.Right.Parent = null;
                Root = Root.Right;
                return;
            }

            //node is left child of parent
            if (node.IsLeftChild)
            {
                node.Parent.Left = node.Right;
            }
            //node is right child of parent
            else
            {
                node.Parent.Right = node.Right;
            }

            node.Right.Parent = node.Parent;
        }

        private void deleteLeftNode(TreapTreeNode<T> node)
        {
            //root
            if (node.Parent == null)
            {
                Root.Left.Parent = null;
                Root = Root.Left;
                return;
            }

            //node is left child of parent
            if (node.IsLeftChild)
            {
                node.Parent.Left = node.Left;
            }
            //node is right child of parent
            else
            {
                node.Parent.Right = node.Left;
            }

            node.Left.Parent = node.Parent;
        }

        public T FindMax()
        {
            return findMax(Root).Value;
        }


        private TreapTreeNode<T> findMax(TreapTreeNode<T> node)
        {
            while (true)
            {
                if (node.Right == null) return node;
                node = node.Right;
            }
        }

        public T FindMin()
        {
            return findMin(Root).Value;
        }

        private TreapTreeNode<T> findMin(TreapTreeNode<T> node)
        {
            while (true)
            {
                if (node.Left == null)
                {
                    return node;
                }

                node = node.Left;
            }
        }


        //find the node with the given identifier among descendants of parent and parent
        //uses pre-order traversal
        //O(log(n)) worst O(n) for unbalanced tree
        private TreapTreeNode<T> find(TreapTreeNode<T> parent, T value)
        {
            while (true)
            {
                if (parent == null)
                {
                    return null;
                }

                if (parent.Value.CompareTo(value) == 0)
                {
                    return parent;
                }

                var left = find(parent.Left, value);

                if (left != null)
                {
                    return left;
                }

                parent = parent.Right;
            }
        }

        //reorder the tree node so that heap property is valid
        private void heapify(TreapTreeNode<T> node)
        {
            while (node.Parent != null)
            {
                if (node.Priority < node.Parent.Priority)
                {
                    node = node.IsLeftChild ? rightRotate(node.Parent) : leftRotate(node.Parent);
                }
                else
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Rotates current root right and returns the new root node
        /// </summary>
        /// <param name="currentRoot"></param>
        /// <returns></returns>
        private TreapTreeNode<T> rightRotate(TreapTreeNode<T> currentRoot)
        {
            var prevRoot = currentRoot;
            var leftRightChild = prevRoot.Left.Right;

            var newRoot = currentRoot.Left;

            //make left child as root
            prevRoot.Left.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Left;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Left;
                }
            }

            //move prev root as right child of current root
            newRoot.Right = prevRoot;
            prevRoot.Parent = newRoot;

            //move right child of left child of prev root to left child of right child of new root
            newRoot.Right.Left = leftRightChild;
            if (newRoot.Right.Left != null)
            {
                newRoot.Right.Left.Parent = newRoot.Right;
            }

            if (prevRoot == Root)
            {
                Root = newRoot;
            }

            return newRoot;

        }

        /// <summary>
        /// Rotates the current root left and returns new root
        /// </summary>
        /// <param name="currentRoot"></param>
        /// <returns></returns>
        private TreapTreeNode<T> leftRotate(TreapTreeNode<T> currentRoot)
        {
            var prevRoot = currentRoot;
            var rightLeftChild = prevRoot.Right.Left;

            var newRoot = currentRoot.Right;

            //make right child as root
            prevRoot.Right.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Right;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Right;
                }
            }


            //move prev root as left child of current root
            newRoot.Left = prevRoot;
            prevRoot.Parent = newRoot;

            //move left child of right child of prev root to right child of left child of new root
            newRoot.Left.Right = rightLeftChild;
            if (newRoot.Left.Right != null)
            {
                newRoot.Left.Right.Parent = newRoot.Left;
            }

            if (prevRoot == Root)
            {
                Root = newRoot;
            }

            return newRoot;
        }

        //Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new BSTEnumerator<T>(Root);
        }
    }
}
