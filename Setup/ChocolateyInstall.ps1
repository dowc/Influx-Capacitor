$packageName = 'windirstat'
$fileType = 'msi'
$url = 'http://thargelion.net/resources/InfluxDB.Net.Collector/InfluxDB.Net.Collector.Service.1.0.3.msi'
$silentArgs = ''

Install-ChocolateyPackage $packageName $fileType "$silentArgs" "$url"