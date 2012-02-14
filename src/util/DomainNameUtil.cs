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

namespace org.icann.tld
{

    /*
     * A utitility class to manipulate domain names
     *
     * @author Simon Raveh (Java), Don Meyer (Java to C#)
     * @version 0.1
     */
    public class DomainNameUtil
    {
        /*
         * Find the top level domain of a given domain name.
         *
         * @param domainName a fully qualified domain name
         * @return return the top level domain for this domain name
         */
        public string getTopLevelDomain(string domainName)
        {
            domainName = stripTrailingDot(domainName);
            string[] labels = domainName.Split('.');
            if (labels.Length > 1)
            {
                return labels[labels.Length - 1];
            }
            return labels[0];
        }

        /*
         * <p>Strip the leading "." from a domain name. </p>
         * <pre>
         * DomainNameUtil.stripLeadingDot(.abc.com)  = abc.com
         * DomainNameUtil.stripLeadingDot(.abc)  = abc
         * DomainNameUtil.stripLeadingDot(abc)  = abc
         *
         *
         * @param domainName the domain name to strip the dot from, may be null
         * @return the domain name without the leading dot
         */
        public string stripLeadingDot(string domainName)
        {
            if (domainName == string.Empty)
            {
                return domainName;
            }

            if (domainName.StartsWith(".") && domainName.Length > 1)
            {
                return domainName.Substring(1, domainName.Length - 1);
            }
            return domainName;
        }

        /*
         * <p>Strip the trailing "." from a domain name. </p>
         * <pre>
         * DomainNameUtil.stripLeadingDot(.abc.com.)  = .abc.com
         * DomainNameUtil.stripLeadingDot(abc.)  = abc
         * DomainNameUtil.stripLeadingDot(abc)  = abc
         *
         *
         * @param domainName the domain name to strip the  trailing dot from, may be null
         * @return the domain name without the trailing dot
         */
        public string stripTrailingDot(string domainName)
        {
            if (domainName.EndsWith("."))
            {
                return domainName.Substring(0, domainName.LastIndexOf("."));
            }
            return domainName;
        }
    }
}