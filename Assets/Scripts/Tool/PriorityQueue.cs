using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace Algorithm
{
    public class PriorityQueue<T>{
        
        private int _size;
        public int Size
        {
            get{
                if (_size < 0)  _size = 0;
                return _size;
            }
            set => _size = value;
        }
 
        private int _capacity;
        public int Capacity { 
            get{
                if (_capacity < 0) _capacity = 0;
                return _capacity;
            }
            set => _capacity = value;
        }
        
        public List<T> _elements;
        
        public bool IsEmpty => _size == 0;
        private T Top => _elements[0];
        
        private readonly IComparer<T> _comparator;
        
        public PriorityQueue(IComparer<T> comparator, int capacity = 1) {
            Size = 0;
            Capacity = capacity;
            _elements = new List<T>();
            _comparator = comparator;
        }
 
        public void Enqueue(T element)
        {
            if (Size == Capacity) {
                ExpandCapacity();
            }
 
            _elements[Size] = element;
            HeapInsert(_elements, Size);
            Size++;
        }
 
        public T Dequeue()
        {
            if (Size == 0) {
                return default(T);
            }
            
            T element = _elements[0];
            //删除堆顶元素，将堆顶元素交换到最后端
            Swap(_elements, 0, Size - 1);
            Size--;
            Heapify(_elements, 0, Size);
            return element;
        }
 
        public T Peek()
        {
            return Top;
        }
        
        private void HeapInsert(List<T> elements, int index)
        {
            //比较当前节点与父节点之间的大小，从插入位置向上处理。
            while (_comparator.Compare(elements[index], elements[(index - 1) / 2]) > 0)
            {
                Swap(elements, index, (index - 1) / 2);
                index = (index - 1) / 2;
            }
        }
 
        public void Clear()
        {
            Size = 0;
        }
 
        private void Heapify(List<T> elements, int index, int size)
        {
            int left = index * 2 + 1;
            while (left < size)
            {
                int comparatorNum = left + 1 < size && _comparator.Compare(elements[left + 1], elements[left]) > 0 ? left + 1 : left;
                comparatorNum = _comparator.Compare(elements[comparatorNum], elements[index]) > 0 ? comparatorNum : index;
                if (comparatorNum == index) {
                    break;
                }
                Swap(elements, comparatorNum, index);
                index = comparatorNum;
                left = index * 2 + 1;
            }
        }
 
        // 这里借鉴CSDN博客:
        // https://blog.csdn.net/Ilovewolves/article/details/119453154
        private void ExpandCapacity() {
            Capacity = Mathf.CeilToInt(Capacity * 1.5f);
            T[] newElements = new T[Capacity];
            for (int i = 0; i < _elements.Count; i++) {
                newElements[i] = _elements[i];
            }
            _elements = new List<T>(newElements);
        }
        
        private void Swap(List<T> elements, int i, int j){
            (elements[i], elements[j]) = (elements[j], elements[i]);
        }
    }
}