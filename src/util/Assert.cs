
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
    public class Assert
    {
        public void assertTrue(bool s)
        {
            if (!s) 
            {
                throw new AssertTrueException();
            }
        }
        public void assertFalse(bool s)
        {
            if (s)
            {
                throw new AssertFalseException();
            }
        }
        public void assertEquals(string s1, string s2)
        {
            if (!s1.Equals(s2))
            {
                throw new AssertEqualsException();
            }
        }
        public void assertEquals(long s1, long s2)
        {
            if(s1 != s2)
            {
                throw new AssertEqualsException();
            }
        }
    }

    public class AssertTrueException : Exception
    {
        public AssertTrueException()
            : base("Operation returns FALSE")
        { 
        }
    }
    public class AssertFalseException : Exception
    {
        public AssertFalseException()
            : base("Operation returns TRUE")
        {
        }
    }
    public class AssertEqualsException : Exception
    {
        public AssertEqualsException()
            : base("Operation returns NOT EQUAL")
        {
        }
    }
}
