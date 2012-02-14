//C# Network Programming 
//FindDNS() Registry routine:
//by Richard Blum 
//Publisher: Sybex 
//ISBN: 0782141765
//
//GetDynamicDnsAddress() routine:
//by Don Meyer
//ICANN
//
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

using System;
using Microsoft.Win32;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections;

namespace org.icann.tld
{
    public class FindDNSServers
    {
        public ArrayList GetDNS()
        {
            string dnsAddress = string.Empty;
            ArrayList serverList = new ArrayList();
            RegistryKey start = Registry.LocalMachine;
            string DNSservers = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";
            RegistryKey DNSserverKey = start.OpenSubKey(DNSservers);
            if (DNSserverKey != null)
            {
                dnsAddress = DNSserverKey.GetValue("NameServer").ToString().Trim();
                DNSserverKey.Close();
                start.Close();
            }
            if(dnsAddress != string.Empty)
            {
                serverList.Add(dnsAddress);
            }
            else
            {
                serverList = GetDynamicDnsAddress();
            }
            return serverList;
        }

        private ArrayList GetDynamicDnsAddress()
        {
            ArrayList serverList = new ArrayList();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                if (dnsServers.Count > 0)
                {
                    foreach (IPAddress dns in dnsServers)
                    {
                        byte[] dbyte = dns.GetAddressBytes();
                        serverList.Add(dbyte[0].ToString() + "." + dbyte[1].ToString() + "." + dbyte[2].ToString() + "." + dbyte[3].ToString());
                    }
                }
            }
            return serverList;
        }
    }
}