$RESOURCE_GROUP = "t.hirayama"
$LOCATION = "japaneast"

# Azure Container Apps Create
$ACA_ENVIRONMENT = "thirayama-test-aca-env"
az containerapp env create `
    --name "${ACA_ENVIRONMENT}" `
    --resource-group "${RESOURCE_GROUP}" `
    --location "${LOCATION}"

# Create Storage Account & Queue
$STORAGE_ACCOUNT_NAME = "thirayamateatsa"
az storage account create `
    --name "${STORAGE_ACCOUNT_NAME}" `
    --resource-group "${RESOURCE_GROUP}" `
    --location "${LOCATION}" `
    --sku Standard_LRS `
    --kind StorageV2

$REQUEST_QUEUE_NAME = "request-queue"
# TODO: Create Queue Command
$RESPONSE_QUEUE_NAME = "response-queue"
# TODO: Create Queue Command

$QUEUE_CONNECTION_STRING = az storage account show-connection-string `
    -g $RESOURCE_GROUP `
    --name $STORAGE_ACCOUNT_NAME `
    --query connectionString `
    --output tsv

# Create Azure Container Registry & Push Image

$CONTAINER_REGISTRY_NAME="thirayamatestacr"
$CONTAINER_IMAGE_NAME="agent"
# TODO: Create ACR
# TODO: Build & Push Images


# Create Agent Job
$JOB_NAME="thirayama-test-acajob-sas-agent"
az containerapp job create `
    --name "${JOB_NAME}" `
    --resource-group "${RESOURCE_GROUP}" `
    --environment "${ACA_ENVIRONMENT}" `
    --trigger-type "Event" `
    --replica-timeout "1800" `
    --replica-retry-limit "1" `
    --replica-completion-count "1" `
    --parallelism "1" `
    --min-executions "0" `
    --max-executions "1" `
    --polling-interval "60" `
    --scale-rule-name "queue" `
    --scale-rule-type "azure-queue" `
    --scale-rule-metadata "accountName=${STORAGE_ACCOUNT_NAME}" "queueName=${REQUEST_QUEUE_NAME}" "queueLength=1" `
    --scale-rule-auth "connection=connection-string-secret" `
    --image "${CONTAINER_REGISTRY_NAME}.azurecr.io/${CONTAINER_IMAGE_NAME}" `
    --cpu "0.25" `
    --memory "0.5Gi" `
    --secrets "connection-string-secret=${QUEUE_CONNECTION_STRING}" `
    --registry-server "${CONTAINER_REGISTRY_NAME}.azurecr.io" `
    --env-vars "AZURE_STORAGE_REQUEST_QUEUE_NAME=${REQUEST_QUEUE_NAME}" "AZURE_STORAGE_RESPONSE_QUEUE_NAME=${RESPONSE_QUEUE_NAME}" "AZURE_STORAGE_CONNECTION_STRING=secretref:connection-string-secret"
