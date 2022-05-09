#!/bin/bash

cd src/QRCodeService

rm -rf ../../publish

dotnet publish -c Release -o ../../publish -r alpine-x64 /p:PublishSingleFile=true /p:DebugType=None /p:DebugSymbols=false --self-contained true