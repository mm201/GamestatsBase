using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GamestatsBase;

namespace Sample
{
    /// <summary>
    /// Summary description for dummy
    /// </summary>
    public class dummy : GamestatsHandler
    {
        public dummy()
            : base("00000000000000000000", 0, 0, 0, 0, "dummy",
                GamestatsRequestVersions.Version1, GamestatsResponseVersions.Version1, false)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, System.IO.MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            
        }
    }
}