# CIBridge

CIBridge is a small app intended to bridge between Source Control Hooks and Continuous integration tools such as Cruise Control

Currently implemented for CodebaseHq and CCNet

## Installation

* Pull from git
** Download the nuget command line tool http://nuget.codeplex.com/releases/58939/download/222685
** From the solution root run 'nuget i Cargowire.CIBridge\packages.config -o packages' to pull/update dependencies
* Ensure that the configuration AppSettings are correct for the CCNet server
* Run as a public website
* Create a hook within CodebaseHq
* Push a commit

## Notes

Two key objects are at play.  A HookParser and a BuildEngine.  HookParsers implement IHookParser and return a small object tree
containing repository information.  BuildEngines implement BuildEngine and provide the ability to retrieve project build statuses
as well as to force a build.

It is intended that the appropriate HookParser will be identified based on the request received.  The beginnings of a GitHubHookParser
has been created for this purpose.

## Future Developments

* Currently the CCNet BuildEngine using the ThoughtWorks dlls fails when attempting to retrieve project statuses
* The ManualCCNet build engine performs posts to the WebDashboard - this works for both force build and retrieving project statuses
* Ideally after receiving a notification the system should hold for a timelimit to see if another quick push is made, to ensure
that it is included in the build and does not generate unnnecessary builds
* Additionally on receiving a build command the current activity should be checked and the build put 'on hold' until it returns to sleep