# build
docker build --pull -t cross-build-arm64 -f Dockerfile.cross-build-x64-arm64 .
docker run --rm -v ${pwd}:/source -w /source cross-build-arm64 dotnet publish -a arm64 -o app-arm64 -p:SysRoot=/crossrootfs/arm64 -p:LinkerFlavor=lld
ls app-arm64
scp -r app-arm64 root@10.11.99.1:~/remarkable-net    
ssh root@10.11.99.1

# sanity test
# docker run -it --rm -v ${pwd}:/source -w /app-arm64 cross-build-arm64
# docker run --rm -v ${pwd}:/source -w /app-arm64 cross-build-arm64 /source/app-arm64/rm_cloud_sync
