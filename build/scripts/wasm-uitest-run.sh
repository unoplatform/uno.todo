#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

export UNO_UITEST_TARGETURI=http://localhost:5000
export UNO_UITEST_DRIVERPATH_CHROME=$BUILD_SOURCESDIRECTORY/build/node_modules/chromedriver/lib/chromedriver
export UNO_UITEST_CHROME_BINARY_PATH=$BUILD_SOURCESDIRECTORY/build/node_modules/puppeteer/.local-chromium/linux-800071/chrome-linux/chrome
export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/wasm
export UNO_UITEST_PLATFORM=Browser
export UNO_UITEST_CHROME_CONTAINER_MODE=true
export UNO_UITEST_PROJECT_PATH=$BUILD_SOURCESDIRECTORY/src/ToDo.UITests
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/src/ToDo.UITests/bin/Release/net7.0/ToDo.UITests.dll
export UNO_UITEST_LOGFILE=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/wasm/nunit-log.xml
export UNO_UITEST_WASM_PROJECT=$BUILD_SOURCESDIRECTORY/src/ToDo.Wasm/ToDo.Wasm.csproj
export UNO_UITEST_WASM_OUTPUT_PATH=$BUILD_SOURCESDIRECTORY/src/ToDo.Wasm/bin/Release/net7.0/dist/
export UNO_UITEST_NUNIT_VERSION=3.11.1
export UNO_UITEST_DOTNETSERVE_VERSION=1.10.112
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe

cd $BUILD_SOURCESDIRECTORY

dotnet build /p:Configuration=Release $UNO_UITEST_WASM_PROJECT /p:IsUiAutomationMappingEnabled=True /p:DisableNet7=True

# install dotnet serve / Remove as needed
dotnet tool uninstall dotnet-serve -g || true
dotnet tool uninstall dotnet-serve --tool-path $BUILD_SOURCESDIRECTORY/build/tools || true
dotnet tool install dotnet-serve --version $UNO_UITEST_DOTNETSERVE_VERSION --tool-path $BUILD_SOURCESDIRECTORY/build/tools || true
export PATH="$PATH:$BUILD_SOURCESDIRECTORY/build/tools"

mkdir -p "$BUILD_ARTIFACTSTAGINGDIRECTORY/wasm-dist"
cp -R "$UNO_UITEST_WASM_OUTPUT_PATH" "$BUILD_ARTIFACTSTAGINGDIRECTORY/wasm-dist"

## The python server serves the current working directory, and may be changed by the nunit runner
dotnet-serve -p 5000 -d "$UNO_UITEST_WASM_OUTPUT_PATH" &

cd $BUILD_SOURCESDIRECTORY/build

npm i chromedriver@86.0.0
npm i puppeteer@5.3.1

mkdir -p $UNO_UITEST_SCREENSHOT_PATH


cd $UNO_UITEST_PROJECT_PATH
dotnet test \
	-c Release \
	-l:"console;verbosity=normal" \
	--logger "nunit;LogFileName=$UNO_UITEST_LOGFILE" \
	|| true
