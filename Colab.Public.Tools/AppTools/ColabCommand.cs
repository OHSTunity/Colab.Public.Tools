using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colab.Public
{
    public enum ColabCommand
        {
            REREQUEST_URL, //makes a new request on the current url
            MORPH_URL, //Morph to the given url (push it to history and make a request)
            PUSH_TO_HISTORY_URL, //(just push it to history)
            MODIFY_URL, //modifies current url without reload and history changes
            OPEN_URL, //Open up an url in a new window/tab
            DOWNLOAD_URL, //download the file given with the url
            END_SESSION, //logout
            ADD_CLASS_TO_ELEMENT, //T
            GO_BACK,
        }
    
}
