#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi

dotnet restore

#copy the Sqlite database script to the test output folder
mkdir -p /home/travis/build/r15h1/heyteam/src/HeyTeam.Tests/bin/Release/netcoreapp2.0
cp ./src/database/SqliteScript.sql /home/travis/build/r15h1/heyteam/src/HeyTeam.Tests/bin/Release/netcoreapp2.0/

#dotnet test ./src/HeyTeam.Tests/HeyTeam.Tests.csproj -c Release -f netcoreapp2.0

revision=${TRAVIS_JOB_ID:=1}  
revision=$(printf "%04d" $revision) 

dotnet pack ./src/HeyTeam.Web/HeyTeam.Web.csproj -c Release -o ./artifacts --version-suffix=$revision  
