using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Colab.Public
{
    public static class Enum<T>  
    {  
        public static T Parse(string value)  
        {  
            return (T) Enum.Parse(typeof (T), value, true);  
        }

        public static String GetName(T value)
        {
            return Enum.GetName(typeof(T), value);
        }
      
        public static IList<T> GetValues()  
        {  
           IList<T> list = new List<T>();  
           foreach (object value in Enum.GetValues(typeof (T)))  
           {  
               list.Add((T) value);  
           }  
           return list;  
       }  
    }  
}