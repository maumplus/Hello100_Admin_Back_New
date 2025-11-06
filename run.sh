#!/bin/bash

# Hello100Admin API 실행 스크립트

echo "🚀 Starting Hello100Admin API..."
echo ""

cd "$(dirname "$0")/src/API" || exit
dotnet run
