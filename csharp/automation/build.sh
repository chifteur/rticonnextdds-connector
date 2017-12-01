#!/bin/bash
REPO_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/..

pushd "${REPO_DIR}"
command -v dotnet >/dev/null 2>&1 && dotnet restore "${REPO_DIR}"/Connector.sln || nuget restore "${REPO_DIR}"/Connector.sln
popd

msbuild /v:minimal "${REPO_DIR}"/Connector.sln