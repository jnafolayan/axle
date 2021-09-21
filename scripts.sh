call_get () {
  local output="$(curl -X GET -s -i http://localhost:5000/$1)"
  echo "$output"
}

help () {
  echo "Available commands are:"
  echo "  > help"
  echo "  > build-server"
  echo "  > build-docker-dev"
  echo "  > build-docker-prod"
  echo "  > run-dev"
  echo "  > run-prod"
  echo "  > index-documents"
}

if [ $1 = "help" ]
then
  help
elif [ $1 = "index-documents" ] 
then
  echo "Indexing documents..."
  echo ""
  call_get "indexdocument"
elif [ $1 = "build-server" ]
then
  echo "Build server..."
  echo ""
  dotnet publish -c Release
elif [ $1 = "build-docker-dev" ]
then
  sudo docker-compose -f docker-compose.yml build
elif [ $1 = "build-docker-prod" ]
then
  sudo docker-compose -f docker-compose.prod.yml build
elif [ $1 = "run-dev" ]
then
  echo "Spinning dev services up..."
  echo ""
  sudo docker-compose -f docker-compose.yml up
elif [ $1 = "run-prod" ]
then
  echo "Spinning prod services up..."
  echo ""
  sudo docker-compose -f docker-compose.prod.yml up
fi