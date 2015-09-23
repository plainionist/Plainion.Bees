﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Plainion.Serialization;

namespace Plainion.GatedCheckIn.Model
{
    [DataContract(Namespace = "http://github.com/ronin4net/plainion/GatedCheckIn", Name = "BuildDefinition")]
    class BuildDefinition : SerializableBindableBase
    {
        private string myRepositoryRoot;
        private string mySolution;
        private bool myRunTests;
        private bool myCheckIn;
        private string myConfiguration;
        private string myPlatform;
        private string myTestRunnerExecutable;
        private string myTestAssemblyPattern;

        [DataMember]
        public string RepositoryRoot
        {
            get { return myRepositoryRoot; }
            set { SetProperty(ref myRepositoryRoot, value); }
        }

        [DataMember]
        public string Solution
        {
            get { return mySolution; }
            set { SetProperty(ref mySolution, value); }
        }

        [DataMember]
        public bool RunTests
        {
            get { return myRunTests; }
            set { SetProperty(ref myRunTests, value); }
        }

        [DataMember]
        public bool CheckIn
        {
            get { return myCheckIn; }
            set { SetProperty(ref myCheckIn, value); }
        }

        [DataMember]
        public string Configuration
        {
            get { return myConfiguration; }
            set { SetProperty(ref myConfiguration, value); }
        }

        [DataMember]
        public string Platform
        {
            get { return myPlatform; }
            set { SetProperty(ref myPlatform, value); }
        }

        [DataMember]
        public string TestRunnerExecutable
        {
            get { return myTestRunnerExecutable; }
            set { SetProperty(ref myTestRunnerExecutable, value); }
        }

        [DataMember]
        public string TestAssemblyPattern
        {
            get { return myTestAssemblyPattern; }
            set { SetProperty(ref myTestAssemblyPattern, value); }
        }
    }
}
