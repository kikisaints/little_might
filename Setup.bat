echo off
dotnet tool install --global dotnet-mgcb-editor
mgcb-editor --register

dotnet new --install MonoGame.Templates.CSharp

pushd .\src
dotnet add package MonoGame.Extended

dotnet add package MonoGame.Extended.Content.Pipeline
popd

echo
echo =================================================================================================
echo Once the MGCB tool opens up, press F6 to build. If you run into any issues, please file an issue.
echo =================================================================================================
.\src\Content\Content.mgcb
