echo 'start'

# variables
echo 'variables:'
GITHASH=`git rev-parse --short HEAD`
IMGNAME=netcore-demo
CONTAINER=netcore-demo-web
# publish
echo 'publish:'
rm -rf ./publish
dotnet publish -o ./publish
# image
echo 'image:'
docker build -t $IMGNAME:$GITHASH .
docker tag $IMGNAME:$GITHASH $IMGNAME:latest
docker rmi -f $(docker images -q -f dangling=true)
# container
echo 'container:'
docker stop $CONTAINER || true && docker rm -f $CONTAINER || true
docker run -d --name $CONTAINER $IMGNAME

echo 'done!'
