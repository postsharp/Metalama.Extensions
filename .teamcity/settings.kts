// This file is automatically generated by `Build.ps1 generate-scripts`.

// Both Swabra and swabra need to be imported
import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildFeatures.sshAgent
import jetbrains.buildServer.configs.kotlin.buildFeatures.Swabra
import jetbrains.buildServer.configs.kotlin.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.failureConditions.*
import jetbrains.buildServer.configs.kotlin.triggers.*

version = "2024.03"

project {

    buildType(DebugBuild)
    buildType(ReleaseBuild)
    buildType(PublicBuild)
    buildType(PublicDeployment)
    buildType(VersionBump)
    buildType(DownstreamMerge)

    buildTypesOrder = arrayListOf(DebugBuild,ReleaseBuild,PublicBuild,PublicDeployment,VersionBump,DownstreamMerge)

}

object DebugBuild : BuildType({

    name = "Build [Debug]"

    artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults\n"

    params {
        text("BuildArguments", "", label = "Build Arguments", description = "Arguments to append to the 'Build' build step.", allowEmpty = true)
        text("TimeOut", "300", label = "Time-Out Threshold", description = "Seconds after the duration of the last successful build.", regex = """\d+""", validationMessage = "The timeout has to be an integer number.")
    }
    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Kill background processes before cleanup"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
        }
        powerShell {
            name = "Build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "test --configuration Debug --buildNumber %build.number% --buildType %system.teamcity.buildType.id% %BuildArguments%")
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
            param("metricThreshold", "%TimeOut%")
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
    }

    triggers {
        vcs {
            watchChangesInDependencies = true
            branchFilter = "+:<default>"
            // Build will not trigger automatically if the commit message contains comment value.
            triggerRules = "-:comment=<<VERSION_BUMP>>|<<DEPENDENCIES_UPDATED>>:**"
        }
    }

    dependencies {
        dependency(AbsoluteId("Metalama_Metalama20241_Metalama_DebugBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaBackstage_DebugBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaCompiler_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaFrameworkRunTime_DebugBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Framework.RunTime"
            }
        }

     }

})

object ReleaseBuild : BuildType({

    name = "Build [Release]"

    artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults\n"

    params {
        text("BuildArguments", "", label = "Build Arguments", description = "Arguments to append to the 'Build' build step.", allowEmpty = true)
        text("TimeOut", "300", label = "Time-Out Threshold", description = "Seconds after the duration of the last successful build.", regex = """\d+""", validationMessage = "The timeout has to be an integer number.")
    }
    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Kill background processes before cleanup"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
        }
        powerShell {
            name = "Build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "test --configuration Release --buildNumber %build.number% --buildType %system.teamcity.buildType.id% %BuildArguments%")
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
            param("metricThreshold", "%TimeOut%")
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
    }

    dependencies {
        dependency(AbsoluteId("Metalama_Metalama20241_Metalama_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaBackstage_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaCompiler_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaFrameworkRunTime_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Framework.RunTime"
            }
        }

     }

})

object PublicBuild : BuildType({

    name = "Build [Public]"

    artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults\n"

    params {
        text("UpstreamCheckArguments", "", label = "Check pending upstream changes Arguments", description = "Arguments to append to the 'Check pending upstream changes' build step.", allowEmpty = true)
        text("BuildArguments", "", label = "Build Arguments", description = "Arguments to append to the 'Build' build step.", allowEmpty = true)
        text("TimeOut", "300", label = "Time-Out Threshold", description = "Seconds after the duration of the last successful build.", regex = """\d+""", validationMessage = "The timeout has to be an integer number.")
    }
    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Kill background processes before cleanup"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
        }
        powerShell {
            name = "Check pending upstream changes"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools git check-upstream %UpstreamCheckArguments%")
        }
        powerShell {
            name = "Build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "test --configuration Public --buildNumber %build.number% --buildType %system.teamcity.buildType.id% %BuildArguments%")
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
            param("metricThreshold", "%TimeOut%")
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
        sshAgent {
            // By convention, the SSH key name is always PostSharp.Engineering for all repositories using SSH to connect.
            teamcitySshKey = "PostSharp.Engineering"
        }
    }

    dependencies {
        dependency(AbsoluteId("Metalama_Metalama20241_Metalama_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaBackstage_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaCompiler_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaFrameworkRunTime_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Framework.RunTime"
            }
        }

     }

})

object PublicDeployment : BuildType({

    name = "Deploy [Public]"

    type = Type.DEPLOYMENT

    params {
        text("PublishArguments", "", label = "Publish Arguments", description = "Arguments to append to the 'Publish' build step.", allowEmpty = true)
        text("TimeOut", "300", label = "Time-Out Threshold", description = "Seconds after the duration of the last successful build.", regex = """\d+""", validationMessage = "The timeout has to be an integer number.")
    }
    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Publish"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "publish --configuration Public %PublishArguments%")
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
            param("metricThreshold", "%TimeOut%")
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
        sshAgent {
            // By convention, the SSH key name is always PostSharp.Engineering for all repositories using SSH to connect.
            teamcitySshKey = "PostSharp.Engineering"
        }
    }

    dependencies {
        dependency(AbsoluteId("Metalama_Metalama20241_Metalama_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_Metalama_PublicDeployment")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaBackstage_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaCompiler_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }
        }
        dependency(AbsoluteId("Metalama_Metalama20241_MetalamaFrameworkRunTime_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Framework.RunTime"
            }
        }
        dependency(PublicBuild) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults"
            }
        }

     }

})

object VersionBump : BuildType({

    name = "Version Bump"

    params {
        text("BumpArguments", "", label = "Bump Arguments", description = "Arguments to append to the 'Bump' build step.", allowEmpty = true)
        text("TimeOut", "300", label = "Time-Out Threshold", description = "Seconds after the duration of the last successful build.", regex = """\d+""", validationMessage = "The timeout has to be an integer number.")
    }
    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Bump"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "bump %BumpArguments%")
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
            param("metricThreshold", "%TimeOut%")
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
        sshAgent {
            // By convention, the SSH key name is always PostSharp.Engineering for all repositories using SSH to connect.
            teamcitySshKey = "PostSharp.Engineering"
        }
    }

})

object DownstreamMerge : BuildType({

    name = "Downstream Merge"

    params {
        text("DownstreamMergeArguments", "", label = "Merge downstream Arguments", description = "Arguments to append to the 'Merge downstream' build step.", allowEmpty = true)
        text("TimeOut", "300", label = "Time-Out Threshold", description = "Seconds after the duration of the last successful build.", regex = """\d+""", validationMessage = "The timeout has to be an integer number.")
    }
    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Merge downstream"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools git merge-downstream %DownstreamMergeArguments%")
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
            param("metricThreshold", "%TimeOut%")
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
        sshAgent {
            // By convention, the SSH key name is always PostSharp.Engineering for all repositories using SSH to connect.
            teamcitySshKey = "PostSharp.Engineering"
        }
    }

    triggers {
        vcs {
            watchChangesInDependencies = true
            branchFilter = "+:<default>"
            // Build will not trigger automatically if the commit message contains comment value.
            triggerRules = "-:comment=<<VERSION_BUMP>>|<<DEPENDENCIES_UPDATED>>:**"
        }
    }

    dependencies {
        dependency(DebugBuild) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }
        }

     }

})

