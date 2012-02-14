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
using System.IO;

namespace org.icann.tld
{
 /*
 * An example class to demonstrate library usage
 */
    public class Main
    {
        public static readonly string DEFAULT_DOMAIN = "icann.com";

        public void main(string[] args) 
        {
            string domainName = args.Length > 0 ? args[0] : DEFAULT_DOMAIN;
            Console.WriteLine("Verifying domain " + domainName + " using DNS protocol ");
            TldVerify verify = new TldVerify();
            bool b = verify.verifyTld(domainName);
            Console.WriteLine("The result is " + b);
            Console.WriteLine("Verifying domain " + domainName + " using Offline method");

            try 
            {
                bool b1 = verify.verifyTldOffline(domainName);
                Console.WriteLine("The result is " + b1);
            } 
            catch (TLDVerifyException e) 
            {
                Console.WriteLine("Exception " + e.Message);
            }            
            Console.WriteLine("\nEnter test to execute test routines");
        }

        public void test()
        {
            TldVerifyTest tldVerifyTest = new TldVerifyTest();
            DomainNameUtilTest domainNameUtilTest = new DomainNameUtilTest();
            VersionParserTest versionParserTest = new VersionParserTest();

            tldVerifyTest.testRefreshTldDB();
            Console.WriteLine("testRefreshTldDB tested.");
            tldVerifyTest.testVerifyTld();
            Console.WriteLine("testVerifyTld tested.");
            tldVerifyTest.testVerifyTldOffline();
            Console.WriteLine("testVerifyTldOffline tested.");
            domainNameUtilTest.testGetTopLevelDomain();
            Console.WriteLine("testGetTopLevelDomain tested.");
            versionParserTest.testParse();
            Console.WriteLine("testParse tested.");
        }
    }
}