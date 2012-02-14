// Copyright 2007 ICANN. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//    1. Redistributions of source code must retain the above copyright notice, this list of conditions
//       and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions
//       and the following disclaimer in the documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY ICANN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE FREEBSD PROJECT OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY,
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// The views and conclusions contained in the software and documentation are those of the authors
// and should not be interpreted as representing official policies,
// either expressed or implied, of ICANN.

/*
 * A class that lets you verify the existence
 * of a domain
 *  
 * @author Don Meyer
 * @version 0.1
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;

namespace org.icann.tld
{
    internal class Lookup
    {
        private string domainName = string.Empty;
        private string dnsServer = string.Empty;
        private int port = 53; 
        private QTypes type = QTypes.SOA;
        private SendBy sendBy = SendBy.UDP;
        private int results = -100;

        public Lookup()
        { }

        /// <summary>
        /// Searches for TLD.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="type">QType</param>
        /// <param name="sendBy"></param>
        public Lookup(string domainName, QTypes type, SendBy sendBy)
        {
            this.domainName = domainName;
            this.type = type;
            this.sendBy = sendBy;
            send();
        }

        /// <summary>
        /// Searches for TLD.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="sendBy">UDP or TCP</param>
        public Lookup(string domainName, SendBy sendBy)
        {
            this.domainName = domainName;
            this.sendBy = sendBy;
            send();
        }

        /// <summary>
        /// Searches for TLD.
        /// </summary>
        /// <param name="domainName"></param>
        public Lookup(string domainName)
        {
            this.domainName = domainName;
            send();
        }

        private void send()
        {
            FindDNSServers getDNS = new FindDNSServers();
            ArrayList dnsServerArray = getDNS.GetDNS();
            for (int c = 0; c < dnsServerArray.Count; c++)
            {
                try
                {                     
                    //wait up to ~6 seconds for results from the DNS server before killing the process. If more DNS addresses exist loop through the list and keep trying.
                    dnsServer = dnsServerArray[c].ToString();
                    ThreadStart entryPoint = new ThreadStart(completeSend);
                    Thread sendThread = new Thread(entryPoint);
                    int ctr = 60;
                    sendThread.Start();
                    while (ctr > 0)
                    {
                        sendThread.Join(100);   //suspend loop execution for interval.
                        if (results > -100)     //worker thread has completed.
                        {
                            break;
                        }
                        ctr -= 1;
                    }
                    if (results == -100)        //worker thread has not completed.
                    {
                        sendThread.Abort();
                    }
                    else                        //DNS server has returned data.
                    {       
                        break;                  
                    }
                }
                catch (ThreadStateException e)
                {
                    throw new TLDVerifyException(e);
                }
            }
        }

        private void completeSend()
        {
            try
            {
                if (sendBy == SendBy.TCP)
                {
                    String responseData = String.Empty;
                    TcpClient client = new TcpClient(dnsServer, port);
                    Byte[] dGram = dataGram(domainName, sendBy);
                    NetworkStream stream = client.GetStream();
                    stream.Write(dGram, 0, dGram.Length);
                    Byte[] receiveBytes = new Byte[4096];
                    Int32 bytes = stream.Read(receiveBytes, 0, receiveBytes.Length);
                    stream.Close();
                    client.Close();
                    results = getResults(receiveBytes, sendBy);
                }
                else
                {
                    UdpClient udpClient = new UdpClient(dnsServer, port);
                    Byte[] dGram = dataGram(domainName, SendBy.UDP);
                    udpClient.Send(dGram, dGram.Length);
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    results = getResults(receiveBytes, sendBy);
                }
            }
            catch (Exception e)
            {
                throw new TLDVerifyException(e);
            }
        }

        /// <summary>
        /// Returns TLD query results.
        /// </summary>
        /// <returns></returns>
        public bool Result()
        {
            bool OK = false;
            if (results == 0 || results == -3)
            {
                OK = false;
            }
            else if (results > 0)
            {
                OK = true;
            }
            else if (results < 0)
            {
                string errMsg = string.Empty;
                if (results == -1)
                {
                    errMsg = "The name server was unable to interpret the query.";
                }
                if (results == -2)
                {
                    errMsg = "The name server encountered an error condition.";
                }
                if (results == -5)
                {
                    errMsg = "The query was refused by name server.";
                }
                if (results == -8)
                {
                    errMsg = "Invalid response from name server.";
                }
                if (results == -10)
                {
                    errMsg = "Invalid query. No parameters were set.";
                }
                if (results == -100)
                {
                    errMsg = "Name server not responding. Try again.";
                }
                OK = false;
                throw new TLDVerifyException(errMsg);
            }
            return OK;
        }

        private byte[] dataGram(string domainName,SendBy sendBy)
        {
            string[] dName = domainName.Split('.');
            uint dgLength = (uint)sendBy * 2;
            uint bodyLength;
            dGramHeader dHead = new dGramHeader();
            dHead.ID = 0;
            dHead.QR = 0;
            dHead.OpCode = OpCodes.QUERY;
            dHead.AA = 0;
            dHead.TC = 0;
            dHead.RD = 1;
            dHead.RA = 0;
            dHead.Z = 0;
            dHead.RCode = RCodes.Success;
            dHead.QDCount = 1;
            dHead.ANCount = 0;
            dHead.NSCount = 0;
            dHead.ARCount = 0;

            byte[] header = formHeader(dHead);
            byte[] question = formQuestion(dName);
            bodyLength = (uint)(header.Length + question.Length);

            byte[] dGram = new byte[dgLength + bodyLength];
            int ptr = 0;
            if (sendBy == SendBy.TCP)
            {
                dGram[0] = (byte)((bodyLength >> 8) & 0xFF);
                dGram[1] = (byte)(bodyLength & 0xFF); 
                ptr = 2;
            }
            for (int n = 0; n < header.Length; n++)
            {
                dGram[ptr] = header[n];
                ptr += 1;
            }
            for (int m = 0; m < question.Length; m++)
            {
                dGram[ptr] = question[m];
                ptr += 1;
            }
            return dGram;
        }

        private byte[] formHeader(dGramHeader dHead)
        {
            byte[] result = new byte[12];
            result[0] = (byte)((dHead.ID >> 8) & 0xFF);
            result[1] = (byte)(dHead.ID & 0xFF);
            result[2] = (byte)((dHead.QR << 7) + (((byte)dHead.OpCode << 3) & 0xF) + (dHead.AA << 2) + (dHead.TC << 1) + dHead.RD);
            result[3] = (byte)((dHead.RA << 7) + (dHead.Z << 4) + ((byte)dHead.RCode & 0xF));
            result[4] = (byte)((dHead.QDCount >> 8) & 0xFF);
            result[5] = (byte)(dHead.QDCount & 0xFF);
            result[6] = (byte)((dHead.ANCount >> 8) & 0xFF);
            result[7] = (byte)(dHead.ANCount & 0xFF);
            result[8] = (byte)((dHead.NSCount >> 8) & 0xFF);
            result[9] = (byte)(dHead.NSCount & 0xFF);
            result[10] = (byte)((dHead.ARCount >> 8) & 0xFF);
            result[11] = (byte)(dHead.ARCount & 0xFF);
            return result;
        }

        private byte[] formQuestion(string[] domainName)
        {
            string question = string.Empty;
            int qLength = 0;
            int QType = (int)QTypes.SOA;
            int QClass = 1;
            for (int n=0; n<domainName.Length; n++)
            {
                question += (char)domainName[n].Length;
                question += domainName[n];
            }
            qLength = question.Length + 5;
            byte[] result = new byte[qLength];
            for (int m = 0; m < question.Length; m++)
            {
                result[m] = (byte)question[m];
            }
            result[qLength - 5] = 0;
            result[qLength - 4] = (byte)((((int)QType) >> 8) & 0xFF);
            result[qLength - 3] = (byte)((int)QType & 0xFF);			
            result[qLength - 2] = (byte)((QClass >> 8) & 0xFF);
            result[qLength - 1] = (byte)(QClass & 0xFF);
            return result;
        }

        private int getResults(Byte[] receiveBytes, SendBy sendBy)
        {
            int ptr = ((int)sendBy * 2);
            int response = (int)receiveBytes[ptr + 2] & 0x80;
            int authoritativeAnswer = (int)receiveBytes[ptr + 2] & 0x04;
            int truncation = (int)receiveBytes[ptr + 2] & 0x02;
            int rcode = (int)receiveBytes[ptr + 3] & 0x0f;
            int exists = ((int)receiveBytes[ptr + 6] << 8) + (int)receiveBytes[ptr + 7];
            int result;
            if (response == 0)
            {
                result = -8;
            }
            else if (rcode == 1)
            {
                result = -1;
            }
            else if (rcode == 2)
            {
                result = -2;
            }
            else if (rcode == 3)
            {
                result = -3;
            }
            else if (rcode == 5)
            {
                result = -5;
            }
            else
            {
                result = exists;
            }
            return result;
        }
    }

    internal struct dGramHeader
    {
        public uint ID;         // WORD		MessageID
        public byte QR;			// 1 BIT	0 = question, 1 = response
        public OpCodes OpCode;	// 4 BIT	Type of request/response
        public byte AA;			// 1 BIT	Authorative answer
        public byte TC;			// 1 BIT	Truncation
        public byte RD;			// 1 BIT	Recursion desired/supported
        public byte RA;			// 1 BIT	Recursion Available
        public byte Z;			// 3 BIT	Reserved for future use
        public RCodes RCode;	// 4 BIT	Response code
        public uint QDCount;    // WORD     Number of entries in question
        public uint ANCount;    // WORD     Number of recs in answer
        public uint NSCount;    // WORD     Number of recs in authority section
        public uint ARCount;    // WORD     Number of recs in additional records section
    }

    internal enum QTypes : int
    {
        A = 1,			// Host address
        NS = 2,			// Authorative name server
        //MD = 3,		// OBSOLETE: Mail destination
        //MF = 4,		// OBSOLETE: Mail forwarder
        CNAME = 5,		// Canonical name
        SOA = 6,		// Start of Authority
        MB = 7,			// EXPERIMENTAL: Mailbox domain name
        MG = 8,			// EXPERIMENTAL: Mail group member
        MR = 9,			// EXPERIMENTAL: Mail rename domain name
        NULL = 10,		// ?
        WKS = 11,		// Well known service description
        PTR = 12,		// Domain name pointer
        HINFO = 13,		// Host information
        MINFO = 14,		// Mailbox or mail list information
        MX = 15,		// Mail exchange
        TXT = 16,		// Text strings
        AXFR = 252,		// Request for a transfer of an entire zone
        MAILB = 253,	// Request for all mailbox-related records (MB,MG or MR)
        //MAILA = 253,	// OBSOLETE: Request for mail agent RRs 
        All = 255,		// Request for all records
    }

    internal enum OpCodes : int
    {
        QUERY = 0,		// standard query
        IQUERY = 1,		// inverse query
        STATUS = 2		// server status request
    }

    internal enum RCodes : int
    {
        Success = 0,
        Format_Error = 1,
        Server_Failure = 2,
        Name_Error = 3,	
        Not_Implemented = 4,
        Refused = 5,
    }

    internal enum SendBy : int
    {
        UDP = 0,		// UDP search
        TCP = 1,		// TCP search
    }
}

