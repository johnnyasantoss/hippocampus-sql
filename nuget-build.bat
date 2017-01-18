@echo off
nuget pack Hippocampus.SQL.csproj -Symbols -Verbosity detailed -Build -Prop Configuration=Release
exit 0