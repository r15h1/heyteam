#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi

dotnet restore

# Ideally we would use the 'dotnet test' command to test netcoreapp and net451 so restrict for now 
# but this currently doesn't work due to https://github.com/dotnet/cli/issues/3073 so restrict to netcoreapp
cp ./src/database/SqliteScript.sql .src/HeyTeam.Tests/bin/Debug/netcoreapp2.0/SqliteScript.sql
dotnet test ./src/HeyTeam.Tests/HeyTeam.Tests.csproj -c Release -f netcoreapp2.0

revision=${TRAVIS_JOB_ID:=1}  
revision=$(printf "%04d" $revision) 

dotnet pack ./src/HeyTeam.Web/HeyTeam.Web.csproj -c Release -o ./artifacts --version-suffix=$revision  
