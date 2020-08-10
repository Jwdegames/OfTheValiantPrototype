using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace Priority_Queue
{
    //Class was originally private and with the SimplePriorityQueue file, 
    //but for purposes of adding DequeueNode method for SimplePriority Queue, 
    //this class was made public and put into its own file
    public class SimpleNode<TItem,TPriority> : GenericPriorityQueueNode<TPriority>
    {
        public TItem Data { get; private set; }

        //Custom attribute for DijakstraCalculator Class
        //public SimpleNode<TItem, TPriority> predecessor { get; set; }
        
        public SimpleNode(TItem data)
        {
            Data = data;
        }


        //Custom equals method
        override
        public bool Equals(object obj)
        {
            SimpleNode<TItem, TPriority> temp = (SimpleNode<TItem, TPriority>)obj;
            if (temp.Data.Equals(Data) && temp.Priority.Equals(Priority))
            {
                return true;
            }
            return false;
        }

        override
        public string ToString()
        {
            return Data.ToString();
        }
    }

}
