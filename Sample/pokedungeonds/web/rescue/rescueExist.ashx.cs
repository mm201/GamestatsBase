using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sample.pokedungeonds.web.rescue
{
    /// <summary>
    /// Summary description for rescueExist
    /// </summary>
    public class rescueExist : GamestatsHandler
    {
        public rescueExist() : base("TXqjDDOLhPySKSztgBHY", 114069, 32153, 512, 1631340900, "pokedungeonds",
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version1)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {

        }
    }
}
