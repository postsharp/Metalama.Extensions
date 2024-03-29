﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using Spectre.Console.Cli;
using MetalamaDependencies = PostSharp.Engineering.BuildTools.Dependencies.Definitions.MetalamaDependencies.V2024_0;

var product = new Product( MetalamaDependencies.MetalamaExtensions )
{
    Solutions = new Solution[] { new DotNetSolution( "Metalama.Extensions.sln" ) { CanFormatCode = true } },
    PublicArtifacts = Pattern.Create(
        "Metalama.Extensions.DependencyInjection.$(PackageVersion).nupkg",
        "Metalama.Extensions.DependencyInjection.ServiceLocator.$(PackageVersion).nupkg",
        "Metalama.Extensions.Multicast.$(PackageVersion).nupkg",
        "Metalama.Extensions.Metrics.$(PackageVersion).nupkg",
        "Metalama.Extensions.Architecture.$(PackageVersion).nupkg" ),
    Dependencies = new[] { DevelopmentDependencies.PostSharpEngineering, MetalamaDependencies.Metalama },
    MainVersionDependency = MetalamaDependencies.Metalama
};

var commandApp = new CommandApp();

commandApp.AddProductCommands(product);

return commandApp.Run(args);