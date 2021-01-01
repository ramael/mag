define([], function () {
    return {
        login: 'api/public/v1/identity/login',
        warehouses: 'api/public/v1/warehouses',
        warehouseCurrentStock: 'api/public/v1/warehouses/{id}/currentstock'
    }
});
