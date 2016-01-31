#!/bin/bash

## Little helper script as MonoDevelop/Xamarin Studio do not have
## built-in NuGet support

## Run this right after checking out the sorces from git to fetch the
## required dependency DLLs if you are using mono on UNIX

set -e
echo "Fetching dependencies via NuGet"
mono tools/NuGet.exe install -o packages GitPusher/packages.config

echo "-------"
echo "OK. You can now open the GitPusher.sln in Xamarin Studio/MonoDevelop"