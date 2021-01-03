define(["js/contracts/contracts"], function (contracts) {
    this.login = 'api/public/v1/identity/login';
    this.carts = 'api/public/v1/carts?status={status}';
    this.components = 'api/public/v1/components';
    this.component = 'api/public/v1/components/{id}';
    this.loadedcarts = 'api/public/v1/loadedcarts';
    this.loadedcart = 'api/public/v1/loadedcarts/{id}';
    this.loadedcartcheckout = 'api/public/v1/loadedcarts/{id}/checkout';
    this.warehouses = 'api/public/v1/warehouses';
    this.warehouseAreas = 'api/public/v1/warehouses/{id}/areas';
    this.warehouseCurrentStock = 'api/public/v1/warehouses/{id}/currentstock';
    this.warehouseLoadedCarts = 'api/public/v1/warehouses/{id}/loadedcarts';
    this.areaLocations = 'api/public/v1/areas/{id}/locations';

    this.login = function (login) {
        return $.ajax({
            url: this.login,
            dataType: 'json',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(login),
            processData: false
        });
    }.bind(this);

    this.getWarehouses = function (token) {
        return $.ajax({
            url: this.warehouses,
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getWarehouseAreas = function (token, id) {
        return $.ajax({
            url: this.warehouseAreas.replace("{id}", id),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getWarehouseCurrentStock = function (token, id) {
        return $.ajax({
            url: this.warehouseCurrentStock.replace("{id}", id),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getWarehouseLoadedCarts = function (token, id) {
        return $.ajax({
            url: this.warehouseLoadedCarts.replace("{id}", id),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getAreaLocations = function (token, id) {
        return $.ajax({
            url: this.areaLocations.replace("{id}", id),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getComponents = function (token) {
        return $.ajax({
            url: this.components,
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getCarts= function (token, status) {
        return $.ajax({
            url: this.carts.replace("{status}", status),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.getComponent = function(token, id) {
        return $.ajax({
            url: this.component.replace("{id}", id),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.createComponent = function (token, component) {
        return $.ajax({
            url: this.components,
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(component),
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.updateComponent = function (token, id, component) {
        return $.ajax({
            url: this.component.replace("{id}", id),
            type: 'put',
            contentType: 'application/json',
            data: JSON.stringify(component),
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.deleteComponent = function (token, id) {
        return $.ajax({
            url: this.component.replace("{id}", id),
            type: 'delete',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.deleteLoadedCart = function (token, id) {
        return $.ajax({
            url: this.loadedcartcheckout.replace("{id}", id),
            type: 'put',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    return {
        login: this.login,
        getWarehouses: this.getWarehouses,
        getWarehouseAreas: this.getWarehouseAreas,
        getWarehouseCurrentStock: this.getWarehouseCurrentStock,
        getWarehouseLoadedCarts: this.getWarehouseLoadedCarts,
        getAreaLocations: this.getAreaLocations,
        getCarts: this.getCarts,
        getComponents: this.getComponents,
        getComponent: this.getComponent,
        createComponent: this.createComponent,
        updateComponent: this.updateComponent,
        deleteComponent: this.deleteComponent,
        deleteLoadedCart: this.deleteLoadedCart
    }
});
