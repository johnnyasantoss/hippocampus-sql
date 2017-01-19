@echo off
nuget pack Hippocampus.SQL/Hippocampus.SQL.csproj -Symbols -Verbosity detailed -Build -Prop Configuration=Release
exit 0