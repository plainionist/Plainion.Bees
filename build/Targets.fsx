// load dependencies from source folder to allow bootstrapping
#r "/bin/Plainion.CI/Fake.Core.Target.dll"
#r "/bin/Plainion.CI/Fake.IO.FileSystem.dll"
#r "/bin/Plainion.CI/Fake.IO.Zip.dll"
#r "/bin/Plainion.CI/Plainion.CI.Tasks.dll"

open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Plainion.CI

Target "CreatePackage" (fun _ ->
    !! ( outputPath </> "*.*Tests.*" )
    ++ ( outputPath </> "*nunit*" )
    ++ ( outputPath </> "TestResult.xml" )
    ++ ( outputPath </> "**/*.pdb" )
    |> DeleteFiles

    PZip.PackRelease()
)

Target "Deploy" (fun _ ->
    let releaseDir = @"\bin\Plainion.Bees"

    CleanDir releaseDir

    let zip = PZip.GetReleaseFile()
    Unzip releaseDir zip
)

Target "Publish" (fun _ ->
    let zip = PZip.GetReleaseFile()
    PGitHub.Release [ zip ]
)

RunTarget()
