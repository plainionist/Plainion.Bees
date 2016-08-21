using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Plainion.GatedCheckIn
{
    static class SecureStringExtensions
    {
        public static string ToUnsecureString( this SecureString source )
        {
            var returnValue = IntPtr.Zero;
            try
            {
                returnValue = Marshal.SecureStringToGlobalAllocUnicode( source );
                return Marshal.PtrToStringUni( returnValue );
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode( returnValue );
            }
        }

    }
}
