using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sample.pokedungeonds.web.common
{
    /// <summary>
    /// Summary description for setProfile
    /// </summary>
    public class setProfile : GamestatsHandler
    {
        public setProfile() : base("TXqjDDOLhPySKSztgBHY", 114069, 32153, 512, 1631340900, "pokedungeonds", 
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version1)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                0, 8);
            
        }
    }
}