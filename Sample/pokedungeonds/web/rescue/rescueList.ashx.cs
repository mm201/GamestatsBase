using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sample.pokedungeonds.web.rescue
{
    /// <summary>
    /// Summary description for rescueList
    /// </summary>
    public class rescueList : GamestatsHandler
    {
        public rescueList() : base("TXqjDDOLhPySKSztgBHY", 114069, 32153, 512, 1631340900, "pokedungeonds",
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version1)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            
        }
    }
}
