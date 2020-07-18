#!/bin/sh

cd ./build
dotnet Solver.dll "$@" || echo "run error code: $?"
