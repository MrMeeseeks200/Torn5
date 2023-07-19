
source .env
# version=$(<version)
deployedVersion=$(curl ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/version --ssl)
mkdir temp

if [ "$version" == "$deployedVersion" ]
  then
    echo "No changes to deploy"
    echo "Bump version and try again"
    exit 1
fi

mkdir workrepo
cd workrepo
git init
cp -r "../bin/Debug/." .
git add .
git commit -m commit
mkdir -p ../Releases/$version/
git archive -o "../Releases/$version/Torn5.zip" @
cd ..
rm -rf workrepo

# download old version
curl ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/Torn5.zip --ssl -o ./temp/Torn5.zip

# upload old version to archive folder
curl -T temp/Torn5.zip ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Archive/Torn5_$deployedVersion.zip --ssl

# upload new version to current folder
curl -T Releases/$version/Torn5.zip ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/Torn5.zip --ssl

# update version file
curl -T version ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/version --ssl

rm -rf temp

echo deployed Torn5 v$deployedVersion


