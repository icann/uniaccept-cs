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
    public class TldVerifyTest //extends TestCase 
    {
        Assert assert = new Assert();

        public void testRefreshTldDB()
        {
            try
            {
                new TldVerify().refreshTldDB("testDB.txt");
                FileInfo file = new FileInfo("testDB.txt");
                assert.assertTrue(file.Exists);
                string s = string.Empty;
                using (StreamReader sr = file.OpenText())
                {
                    s = sr.ReadToEnd();
                }
                assert.assertTrue(s.IndexOf("AERO") > -1);
            }
            catch (Exception e)
            {
                throw new TLDVerifyException(e);
            }
        }

        public void testVerifyTldOffline()
        {
            try
            {
                TldVerify tldVerify = new TldVerify();
                //assert.assertTrue(tldVerify.verifyTldOffline("AERO."));
                assert.assertTrue(tldVerify.verifyTldOffline("AERO", "testDB.txt"));
                assert.assertFalse(tldVerify.verifyTldOffline("sss"));
                //assert.assertTrue(tldVerify.verifyTldOffline(".AERO."));
            }
            catch (Exception e)
            {
                throw new TLDVerifyException(e);
            }
        }

        public void testVerifyTld()
        {
            try
            {
                TldVerify tldVerify = new TldVerify();
                assert.assertTrue(tldVerify.verifyTld("AERO."));
                assert.assertTrue(tldVerify.verifyTld("AERO"));
                assert.assertTrue(tldVerify.verifyTld("ICANN.ORG"));
                assert.assertTrue(tldVerify.verifyTld("icann.org"));
                //assert.assertFalse(tldVerify.verifyTld("SS"));
            }
            catch (Exception e)
            {
                throw new TLDVerifyException(e);
            }
        }
    }
}