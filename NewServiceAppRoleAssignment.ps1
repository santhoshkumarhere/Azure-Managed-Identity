# Replace with your managed identity object ID
$miObjectID = "29634640-3e5e-497a-bf92-99595792e2dc"
# Replace with the permissions required by your app
$permissionToAdd = "MIFuncAppReadAccessRole"

#FunctionAPP EnterPrise Application Client Id
$appId = "c04bdd78-1323-4f2a-a218-07223049e72f"

Connect-AzureAD

$app = Get-AzureADServicePrincipal -Filter "AppId eq '$appId'"
$role = $app.AppRoles | where Value -Like $permissionToAdd | Select-Object -First 1
New-AzureADServiceAppRoleAssignment -Id $role.Id -ObjectId $miObjectID -PrincipalId $miObjectID -ResourceId $app.ObjectId
 