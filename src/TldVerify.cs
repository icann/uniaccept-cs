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
 * @author Simon Raveh (Java), Don Meyer (Java to C#)
 * @version 0.1
 */

using System;
using System.Net;
using System.IO;
using System.Threading;

namespace org.icann.tld
{
    public class TldVerify 
    { 
        public static readonly string TLD_CACHE_FILE = "tlds-alpha-by-domain.txt";
        private static readonly string TLDS_BY_DOMAIN_TXT = "http://data.iana.org/TLD/tlds-alpha-by-domain.txt";
        private static readonly string MD5_URL = "http://data.iana.org/TLD/tlds-alpha-by-domain.txt.md5";
        private TldCache cache;

        /*
         * Verifies a top-level domain exists.
         * This overloaded method takes from one to three arguments and verifies the TLD using the DNS protocol.
         *
         * @param domainName -- a domain name or a TLD.
         * @param QTypes -- the type of query (SOA, NS, etc.).
         * @param SendBy -- the method of query. Either UDP or TCP.
         * @return <code>true</code> if the top-level domain exists, <code>false</code> if it does not exist.
         */
        public bool verifyTld(string domainName) 
        {
            try
            {
                string topLevelDomain = new DomainNameUtil().getTopLevelDomain(domainName);
                Lookup lookup = new Lookup(topLevelDomain, QTypes.SOA, SendBy.UDP);
                return lookup.Result();
            }
            catch (Exception e)
            {
                throw new TLDVerifyException(e.Message, e);
            }
        }

        /*
         * Verifies a top-level domain exists against a fixed database.
         * This methods takes a single argument, which can either be a
         * domain name, or a TLD and verified for validity against a fixed database that
         * has been previously obtained with the {@link #refreshTldDB()} method.
         *
         * @param domainName the domain name or a TLD to validate
         * @return <code>true</code> if the top-leve domain exist <code>false</code>
         * @throws TLDVerifyException
         */

        public bool verifyTldOffline(string domainName) 
        {
            try
            {
                string topLevelDomain = new DomainNameUtil().getTopLevelDomain(domainName);
                if (cache != null)
                {
                    return cache.exist(topLevelDomain);
                }
                else
                {
                    refreshTldDB(TLD_CACHE_FILE);
                    return verifyTldOffline(topLevelDomain, TLD_CACHE_FILE);
                }
            }
            catch (Exception e)
            {
                throw new TLDVerifyException(e);
            }
        }

         /*
         * Verifies a top-level domain exists against a fixed database.
         * The first argument supplies is either a domain name, or a TLD,
         * which is verified for validity against a fixed database that
         * has been previously obtained with the {@link #refreshTldDB()} method.
         * The second argument is the filename where the cache is stored.
         * The file format should be the same as  the one IANA publish at (http://data.iana.org/TLD/tlds-alpha-by-domain.txt).
         * The version in the first line will be check against the current cache version to prevent loading of old data.
         *
         * @param domainName    The domain name or a TLD to validate
         * @param cacheFilePath The full path to where the cache is stored.
         * @return <code>true</code> if the top-leve domain exist <code>false</code>
         * @throws TLDVerifyException
         */
        public bool verifyTldOffline(string domainName, string cacheFilePath) 
        {
            try 
            {
                string topLevelDomain = new DomainNameUtil().getTopLevelDomain(domainName);
                readTld(new FileInfo(cacheFilePath));
                return cache.exist(topLevelDomain);
            } 
            catch (IOException e) 
            {
                throw new TLDVerifyException(e);
            }
        }

        /*
         * Updates the copy of the fixed database of valid top-level
         * domains.
         * Downloads the official list of valid TLDs from the IANA website,
         * and performs consistency checking to ensure it was downloaded
         * correctly. Store the data in the default location.
         *
         * @throws TLDVerifyException
         */
        public void refreshTldDB() 
        {
            try
            {
                refreshTldDB(TLD_CACHE_FILE);
            }
            catch (IOException e)
            {
                throw new TLDVerifyException(e);
            }
        }

        /*
         * Updates the copy of the fixed database of valid top-level
         * domains.
         * Downloads the official list of valid TLDs from the IANA website,
         * and performs consistency checking to ensure it was downloaded
         * correctly. The parameter is the filename to store the cache in.
         *
         * @param cacheStoreFileName The full path to the file to store the cache in.
         * @throws TLDVerifyException
         */
        public void refreshTldDB(string cacheStoreFileName)
        {
            try 
            {
                string outputCacheName = TLD_CACHE_FILE;
                string tempFile = "temp_md5 " + DateTime.Now.Ticks;
                FileInfo tFile = new FileInfo(tempFile);
                WebClient toFile = new WebClient();
                toFile.DownloadFile(TLDS_BY_DOMAIN_TXT, tempFile);

                string digest = getDigestInfo(MD5_URL);
                string digest1 = new FileBasedMD5Generator().createDigest(tempFile);

                if (!digest.Equals(digest1)) {
                    throw new TLDVerifyException("Could not download TLD data from IANA web site");
                }

                readTld(tFile);
                tFile.Delete();

                if (cacheStoreFileName != null) {
                    outputCacheName = cacheStoreFileName;
                }

                writeTlds(outputCacheName);
            } 
            catch (Exception e) 
            {
                throw new TLDVerifyException(e.Message, e);
            }
        }

        private string getDigestInfo(string URL)
        {
            try
            {
                WebClient url = new WebClient();
                using (Stream URLStream = url.OpenRead(URL))
                {
                    using (StreamReader sr = new StreamReader(URLStream))
                    {
                        string header = sr.ReadLine();
                        return header.Trim().Substring(0, 32);
                    }
                }
            }
            catch (IOException e)
            {
                throw new TLDVerifyException(e.Message, e);
            }
        }

        private void readTld(FileInfo tFile) 
        {
            try 
            {
                string line;
                using (StreamReader sr = tFile.OpenText())
                {
                    string header = sr.ReadLine();
                    long version = parseVersion(header);
                    TldCache newCache = new TldCache(version, header);
                    if (cache != null && cache.getVersion >= version)
                    {
                        return;
                    }
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }
                        newCache.addTld(line.Trim());
                    }
                    cache = newCache;
                }
            }
            catch (Exception e) 
            {
                throw new TLDVerifyException(e);
            }
        }

        private long parseVersion(string line) 
        {
            return new VersionParser().parse(line);
        }

        private void writeTlds(string filePath) 
        {
            try
            {
                cache.print(filePath);
            }
            catch (IOException e) 
            {
                throw new TLDVerifyException(e);
            }
        }
    }
}

