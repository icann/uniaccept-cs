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
using System.Collections;
using System.IO;

namespace org.icann.tld
{
    /*
    * In memory cache that holds all the TLDs that were fetch using
    * {@link TldVerify#refreshTldDB}
    *
    * @author Simon Raveh (Java), Don Meyer (Java to C#)
    * @version 0.1
    */
    class TldCache 
    {
        private ArrayList tlds;
        private string header;
        private long version;

        public TldCache(long version, string header) 
        {
            this.version = version;
            this.header = header;
            tlds = new ArrayList();
        }

        /*
         * Return True if the TLD exist in the cache
         * @param tld - the top level domain to check
         * @return  <code>true</code> if the top level domain exist <code>false</code> otherwise
         */
        public bool exist(string tld) 
        {
            for (int i=0; i<tlds.Count; i++)
            { 
                if(tlds[i].ToString().ToUpper().Equals(tld.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Return a list of all TLDs currently in the cache
         * @return  a list of all TLDs
         */
         public ArrayList TLDs
         {
             get
             {
                 return tlds;
             }
         }

        /*
         * Write out all TLDs and the header
         *
         * @param out The writer to use
         * @throws IOException
         */
        public void print(string pFile) 
        {
            try
            {
                using (FileStream fs = new FileStream(pFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(header);
                        for (int i = 0; i < tlds.Count; i++)
                        {
                            sw.WriteLine(tlds[i].ToString());
                        }
                    }
                }
            }
            catch (IOException e)
            {
                throw new TLDVerifyException(e);
            }
        }

        /*
         * Add new TLD to the cache.
         *
         * @param tld The top level domain to add
         */
        public void addTld(string tld) 
        {
            tlds.Add(tld);
        }

        public long getVersion
        {
            get
            {
                return version;
            }
        }
    }
}