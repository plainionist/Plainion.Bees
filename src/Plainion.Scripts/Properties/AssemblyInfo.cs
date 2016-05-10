using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Security;

[assembly: AssemblyTitle( "Plainion.Scripts" )]
[assembly: CLSCompliant( true )]

// required since .NET 4.0 to allow calls to other assemblies
[assembly: AllowPartiallyTrustedCallersAttribute()]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "2b0131cb-d5a7-4343-a841-ac8f7fa38dcb" )]

[assembly: AssemblyVersion("1.11.0.0")]
