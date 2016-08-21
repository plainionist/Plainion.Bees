using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Plainion.Serialization;

namespace Plainion.GatedCheckIn.Model
{
    [Serializable]
    [DataContract( Namespace = "http://github.com/ronin4net/plainion/GatedCheckIn", Name = "BuildDefinition" )]
    class BuildDefinition : SerializableBindableBase
    {
        private string myRepositoryRoot;
        private string mySolution;
        private bool myRunTests;
        private bool myCheckIn;
        private bool myPush;
        private string myConfiguration;
        private string myPlatform;
        private string myTestRunnerExecutable;
        private string myTestAssemblyPattern;
        private string myUserName;
        private string myUserEMail;
        [NonSerialized]
        private SecureString myUserPassword;
        private string myDiffTool;

        [DataMember( Name = "UserPassword" )]
        private byte[] mySerializablePassword;

        public string RepositoryRoot
        {
            get { return myRepositoryRoot; }
            set { SetProperty( ref myRepositoryRoot, value != null ? Path.GetFullPath( value ) : null ); }
        }

        [DataMember]
        public string Solution
        {
            get { return mySolution; }
            set { SetProperty( ref mySolution, value ); }
        }

        [DataMember]
        public bool RunTests
        {
            get { return myRunTests; }
            set { SetProperty( ref myRunTests, value ); }
        }

        [DataMember]
        public bool CheckIn
        {
            get { return myCheckIn; }
            set { SetProperty( ref myCheckIn, value ); }
        }

        [DataMember]
        public bool Push
        {
            get { return myPush; }
            set { SetProperty( ref myPush, value ); }
        }

        [DataMember]
        public string Configuration
        {
            get { return myConfiguration; }
            set { SetProperty( ref myConfiguration, value ); }
        }

        [DataMember]
        public string Platform
        {
            get { return myPlatform; }
            set { SetProperty( ref myPlatform, value ); }
        }

        [DataMember]
        public string TestRunnerExecutable
        {
            get { return myTestRunnerExecutable; }
            set { SetProperty( ref myTestRunnerExecutable, value ); }
        }

        [DataMember]
        public string TestAssemblyPattern
        {
            get { return myTestAssemblyPattern; }
            set { SetProperty( ref myTestAssemblyPattern, value ); }
        }

        [DataMember]
        public string UserName
        {
            get { return myUserName; }
            set { SetProperty( ref myUserName, value ); }
        }

        [DataMember]
        public string UserEMail
        {
            get { return myUserEMail; }
            set { SetProperty( ref myUserEMail, value ); }
        }

        public SecureString UserPassword
        {
            get { return myUserPassword; }
            set
            {
                if ( SetProperty( ref myUserPassword, value ) )
                {
                    // we do serialization as "update-on-write" because we also want to support cloning at any time
                    if ( myUserPassword == null )
                    {
                        mySerializablePassword = null;
                    }
                    else
                    {
                        var bytes = Encoding.UTF8.GetBytes( myUserPassword.ToUnsecureString() );

                        mySerializablePassword = ProtectedData.Protect( bytes, null, DataProtectionScope.CurrentUser );
                    }
                }
            }
        }

        [DataMember]
        public string DiffTool
        {
            get { return myDiffTool; }
            set { SetProperty( ref myDiffTool, value ); }
        }

        [OnDeserialized]
        private void OnDeserialized( StreamingContext context )
        {
            if ( mySerializablePassword != null )
            {
                var bytes = ProtectedData.Unprotect( mySerializablePassword, null, DataProtectionScope.CurrentUser );

                myUserPassword = Encoding.UTF8.GetString( bytes ).ToSecureString();
            }
        }
    }
}
