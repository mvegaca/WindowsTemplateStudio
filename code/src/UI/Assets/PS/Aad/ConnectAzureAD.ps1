# Connect to Azure with a browser sign in token
$data = Connect-AzureAD
$result = New-Object -TypeName PSObject
Add-Member -InputObject $result -MemberType NoteProperty -Name Account -Value $data.Context.Account.Id
Add-Member -InputObject $result -MemberType NoteProperty -Name TenantId -Value $data.Context.Tenant.TenantId
return $result