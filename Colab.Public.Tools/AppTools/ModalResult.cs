using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Colab.Public
{
    public class ModalResult
    {
        public Json Modal;
        public T GetModal<T>() where T:Json
        {
            return Modal as T;
        }
        public ModalResultType Value;
        public String Msg;
        public Object Data;
    }

    public enum ModalResultType
    {
        SUCCESS,
        FAILURE,
        YES,
        NO,
        OK,
        CANCEL,
        NORESULT,
        REMOVE
    }
}
