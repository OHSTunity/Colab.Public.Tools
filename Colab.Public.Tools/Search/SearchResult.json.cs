using Starcounter;
using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter.Advanced;
using Starcounter.Templates;

namespace Colab.Public
{
    [SearchResult_json]
    partial class SearchResult: Page
    {
    }

    [SearchResult_json.Types]
    public partial class SearchType : Json
    {
    }
}
