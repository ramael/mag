define([], function () {
    return {
        login: 'api/public/v1/identity/login',
        components: 'api/public/v1/components',
        component: 'api/public/v1/components/{id}',
        warehouses: 'api/public/v1/warehouses',
        warehouseCurrentStock: 'api/public/v1/warehouses/{id}/currentstock'
    }
});
