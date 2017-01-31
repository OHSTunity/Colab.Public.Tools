using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Starcounter;
using Concepts.Ring1;
using Concepts.Ring8.Tunity;

namespace Colab.Public
{
    public class SearchQuery
    {
        private List<KeyValuePair<String, String>> _filters = new List<KeyValuePair<string, string>>();
        private string _freetext;
        private string _type;


        public SearchQuery(String query)
        {
            query = HttpUtility.UrlDecode(query);
            var parts = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String part in parts)
            {
                if (part.IndexOf(':') >= 0)
                {
                    var opt = part.Split(':');
                    var a = opt[0].ToLower();
                    Filters.Add(new KeyValuePair<string, string>(a, (opt.Length > 1)? opt[1]: ""));
                }
                else
                {
                    if (!String.IsNullOrEmpty(_freetext))
                        _type = _freetext;
                    _freetext = part;
                }
            }
        }


        public string Freetext
        {
            get
            {
                return _freetext;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public List<KeyValuePair<string, string>> Filters
        {
            get
            {
                return _filters;
            }
        }
    }
}
