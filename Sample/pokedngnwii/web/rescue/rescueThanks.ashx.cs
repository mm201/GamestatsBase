using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Sample.pokedngnwii.web.rescue
{
    /// <summary>
    /// Summary description for rescueThanks
    /// </summary>
    public class rescueThanks : GamestatsHandler
    {
        public rescueThanks() : base("zjzrhOVXZKLHNspYpGoR0001c7850000620b0000000820556356pokedngnwii",
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version2)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            // replay response from Receiving a Random A-OK Mail And Sending a Random Thank You Mail.pcapng
            // request data:
            // 0000: 8b9ca41c7c000000 1ca49c8b00000000
            // 0010: 0001b14b0007000a 306f306e3067306e
            // 0020: 30673092306e306b 0000000000000000
            // 0030: 0000000000000000 0000000030923092
            // 0040: 3092309230923092 3092309230923092
            // 0050: 3092309230923092 3092309230923092
            // 0060: 3092309230923092 3092309230923092
            // 0070: 3092309230923092 3092309230923092
            // 0080: 30923092

            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                0, 6);
        }
    }
}