using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Aveva.Gdp.TokenVault.Api.Helpers
{
    public static class SecureStringHelper
    {
        #region Public Methods

        public static bool Equals(this SecureString s1, SecureString s2)
        {
            if (s1 is null || s2 is null)
                throw new ArgumentNullException();

            if (s1.Length != s2.Length)
                return false;

            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;

            RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                bstr1 = Marshal.SecureStringToBSTR(s1);
                bstr2 = Marshal.SecureStringToBSTR(s2);

                unsafe
                {
                    for (Char* ptr1 = (Char*)bstr1.ToPointer(), ptr2 = (Char*)bstr2.ToPointer();
                        *ptr1 != 0 && *ptr2 != 0;
                         ++ptr1, ++ptr2)
                    {
                        if (*ptr1 != *ptr2)
                            return false;
                    }
                }

                return true;
            }
            finally
            {
                if (bstr1 != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstr1);

                if (bstr2 != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstr2);
            }
        }

        public static T ProcessBytes<T>(this SecureString src, Func<byte[], T> func)
        {
            IntPtr bstr = IntPtr.Zero;
            byte[] workArray = null;
            GCHandle? handle = null;
            try
            {
                /*** PLAINTEXT EXPOSURE BEGINS HERE ***/
                bstr = Marshal.SecureStringToBSTR(src);
                unsafe
                {
                    byte* bstrBytes = (byte*)bstr;
                    workArray = new byte[src.Length * 2];
                    handle = GCHandle.Alloc(workArray, GCHandleType.Pinned);
                    for (int i = 0; i < workArray.Length; i++)
                        workArray[i] = *bstrBytes++;
                }

                return func(workArray);
            }
            finally
            {
                if (workArray != null)
                    for (int i = 0; i < workArray.Length; i++)
                        workArray[i] = 0;
                handle?.Free();
                if (bstr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstr);
                /*** PLAINTEXT EXPOSURE ENDS HERE ***/
            }
        }

        #endregion
    }
}