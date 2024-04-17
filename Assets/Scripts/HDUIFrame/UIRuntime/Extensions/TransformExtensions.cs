using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// ������ȱ���
    /// except��������ָ���ڵ��µı���������а���ָ���Ľڵ㣬���ǲ�����ָ���ڵ������е��ӽڵ㣩
    /// </summary>
    public static IEnumerable<Transform> DepthTraversal(this Transform root, Predicate<Transform> except = null)
    {
        if (root == null) 
        {
            yield break;
        } 

        var stack = new Stack<Transform>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node;
            if (except != null && except.Invoke(node)) 
            {
                continue;
            } 

            for (int i = node.childCount - 1; i >= 0; --i)
            {
                stack.Push(node.GetChild(i));
            }
        }
    }

    /// <summary>
    /// ������ȱ���
    /// except��������ָ���ڵ��µı���������а���ָ���Ľڵ㣬���ǲ�����ָ���ڵ������е��ӽڵ㣩
    /// </summary>
    public static IEnumerable<Transform> BreadthTraversal(this Transform root, Predicate<Transform> except = null)
    {
        if (root == null) 
        {
            yield break;
        } 

        var queue = new Queue<Transform>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node;
            if (except != null && except.Invoke(node)) 
            {
                continue;
            }
           
            for (int i = 0; i < node.childCount; ++i)
            {
                queue.Enqueue(node.GetChild(i));
            }
        }
    }
}
