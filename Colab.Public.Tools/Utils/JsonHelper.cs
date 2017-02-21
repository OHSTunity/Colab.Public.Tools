using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using Concepts.Ring1;
using Concepts.Ring2;
using Concepts.Ring3;
using Concepts.Ring8.Tunity;
using System.Web;

namespace Colab.Public
{
    public static class ColabJsonHelper
    {

        public static T GetParent<T>(Json child) where T : Json
        {
            while (child != null)
            {
                if (child is T)
                    return child as T;
                child = child.Parent;
            }
            return null;
        }

        public static T GetParentClass<T>(Json child) where T : class
        {
            while (child != null)
            {
                if (child is T)
                    return child as T;
                child = child.Parent;
            }
            return null;
        }

        public static T GetParentData<T>(Json j)
        {
            if (j.Data != null && j.Data is T)
            {
                return (T)j.Data;
            }
            if (j.Parent != null)
            {
                return GetParentData<T>(j.Parent);
            }
            return default(T);
        }
    }
}
