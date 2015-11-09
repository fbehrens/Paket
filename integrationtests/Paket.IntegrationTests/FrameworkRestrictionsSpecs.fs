﻿module Paket.IntegrationTests.FrameworkRestrictionsSpecs

open Fake
open Paket
open System
open NUnit.Framework
open FsUnit
open System
open System.IO
open Paket.Domain
open Paket.Requirements

[<Test>]
let ``#140 windsor should resolve framework dependent dependencies``() =
    let lockFile = update "i000140-resolve-framework-restrictions"
    lockFile.Groups.[Constants.MainDependencyGroup].Resolution.[PackageName "TaskParallelLibrary"].Settings.FrameworkRestrictions
    |> shouldEqual [FrameworkRestriction.Exactly(DotNetFramework(FrameworkVersion.V3_5))]

[<Test>]
let ``#1182 framework restrictions overwrite each other``() =
    let lockFile = update "i001182-framework-restrictions"
    let lockFile = lockFile.ToString()
    lockFile.Contains("Microsoft.Data.OData (>= 5.6.2)") |> shouldEqual true
    lockFile.Contains("framework: winv4.5") |> shouldEqual false

[<Test>]
let ``#1190 paket add nuget should handle transitive dependencies``() = 
    paket "add nuget xunit version 2.1.0" "i001190-transitive-dependencies-with-restrictions" |> ignore
    
    let lockFile = LockFile.LoadFrom(Path.Combine(scenarioTempPath "i001190-transitive-dependencies-with-restrictions","paket.lock"))
    lockFile.Groups.[Constants.MainDependencyGroup].Resolution.[PackageName "xunit.abstractions"].Settings.FrameworkRestrictions
    |> shouldContain (FrameworkRestriction.AtLeast(DotNetFramework(FrameworkVersion.V4_5)))
    
[<Test>]
let ``#1190 paket add nuget should handle transitive dependencies with restrictions``() = 
    paket "add nuget xunit version 2.1.0" "i001190-transitive-deps" |> ignore
    
    let lockFile = LockFile.LoadFrom(Path.Combine(scenarioTempPath "i001190-transitive-deps","paket.lock"))
    lockFile.Groups.[Constants.MainDependencyGroup].Resolution.[PackageName "xunit.abstractions"].Settings.FrameworkRestrictions
    |> shouldEqual []
    