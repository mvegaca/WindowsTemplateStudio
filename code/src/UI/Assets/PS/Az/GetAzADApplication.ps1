$data = Get-AzADApplication -First 100
$resultApps = New-Object "System.Collections.Generic.List[PSObject]"
foreach ($app in $data) {
	$result = New-Object -TypeName PSObject
	Add-Member -InputObject $result -MemberType NoteProperty -Name DisplayName -Value $app.DisplayName
	Add-Member -InputObject $result -MemberType NoteProperty -Name ObjectId -Value $app.ObjectId
	Add-Member -InputObject $result -MemberType NoteProperty -Name HomePage -Value $app.HomePage
	Add-Member -InputObject $result -MemberType NoteProperty -Name Type -Value $app.Type
	Add-Member -InputObject $result -MemberType NoteProperty -Name ApplicationId -Value $app.ApplicationId
	Add-Member -InputObject $result -MemberType NoteProperty -Name AvailableToOtherTenants -Value $app.AvailableToOtherTenants

	Add-Member -InputObject $result -MemberType NoteProperty -Name IdentifierUris -Value $app.IdentifierUris
	Add-Member -InputObject $result -MemberType NoteProperty -Name AppPermissions -Value $app.AppPermissions
	Add-Member -InputObject $result -MemberType NoteProperty -Name ReplyUrls -Value $app.ReplyUrls
	$resultApps.Add($result)
 }

return $resultApps