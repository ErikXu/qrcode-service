#!/bin/bash

docker run --rm -i \
    -v /root/.nuget/packages:/root/.nuget/packages \
    -v ${PWD}:/workspace \
    mcr.microsoft.com/dotnet/sdk:6.0-alpine \
    sh -c 'cd /workspace && sh publish.sh'