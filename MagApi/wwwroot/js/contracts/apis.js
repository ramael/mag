define(["js/contracts/contracts"], function (contracts) {
    this.login = 'api/public/v1/identity/login';
    this.carts = 'api/public/v1/carts?status={status}';
    this.components = 'api/public/v1/components';
    this.component = 'api/public/v1/components/{id}';
    this.loadedcarts = 'api/public/v1/loadedcarts';
    this.loadedcart = 'api/public/v1/loadedcarts/{id}?includedetails={includedetails}';
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
        }).then(function (data) {
            if (data) {
                return new contracts.loginresponse(data.username, data.firstname, data.lastname, data.roles, data.token);
            }
            return null;
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
        }).then(function (data) {
            if (data && data.length > 0) {
                return data.map(w => new contracts.warehouse(w.id, w.name, w.description, w.notes));
            }
            return null;
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
        }).then(function (data) {
            if (data && data.length > 0) {
               return data.map(a => new contracts.area(a.id, a.name, a.description, a.notes));
            }
            return null;
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
        }).then(function (data) {
            if (data && data.length > 0) {
               return data.map(function (cs) {
                    const mcs = new contracts.stock(cs.componentcode, cs.componentdescription, cs.componentqty, null);
                    if (cs.details && cs.details.length > 0) {
                        mcs.details = cs.details.map(csd => new contracts.stockdetail(csd.serialnumber, csd.areaname, csd.locationname));
                    }
                    return mcs;
               });
            }
            return null;
        });
    }.bind(this);

    this.getWarehouseLoadedCarts = function (token, id) {
        return $.ajax({
            url: this.warehouseLoadedCarts.replace("{id}", id).replace("{includedetails}", false),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then(function (data) {
            if (data && data.length > 0) {
                return data.map(function (lc) {
                    const mc = new contracts.cart(lc.cart.id, lc.cart.serialnumber, lc.cart.status);
                    const ma = new contracts.area(lc.location.area.id, lc.location.area.name, lc.location.area.description, null, null, null);
                    const ml = new contracts.location(lc.location.id, lc.location.name, lc.location.description, null, ma);
                    return new contracts.loadedcart(lc.id, lc.year, lc.progressive, lc.description, ma.id, ma, ml.id, ml, mc.id, mc, lc.datein, null, null);
                });
            }
            return null;
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
        }).then(function (data) {
            if (data && data.length > 0) {
               return data.map(l => new contracts.location(l.id, l.name, l.description, l.notes));
            }
            return null;
        });
    }.bind(this);

    this.getCarts = function (token, status) {
        return $.ajax({
            url: this.carts.replace("{status}", status),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then(function (data) {
            if (data && data.length > 0) {
                return data.map(c => new contracts.cart(c.id, c.serialnumber, c.status));
            }
            return null;
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
        }).then(function (data) {
            if (data && data.length > 0) {
                return data.map(c => new contracts.component(c.id, c.code, c.description, c.notes));
            }
            return null;
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
        }).then(function (data) {
            if (data) {
                return new contracts.component(data.id, data.code, data.description, data.notes);
            }
            return null;
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

    this.getLoadedCart = function (token, id) {
        return $.ajax({
            url: this.loadedcart.replace("{id}", id).replace("{includedetails}", true),
            type: 'get',
            contentType: 'application/json',
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then(function (lc) {
            if (lc) {
                let lcDetails = [];
                if (lc.loadedcartdetails && lc.loadedcartdetails.length > 0) {
                    lcDetails = lc.loadedcartdetails.map(function (lcd) {
                        let mc = null;
                        if (lcd.component) {
                            mc = new contracts.component(lcd.component.id, lcd.component.code, lcd.component.description, lcd.component.notes);
                        }
                        return new contracts.loadedcartdetail(lcd.id, lcd.loadedcartid, lcd.componentid, mc, lcd.notes, 0);
                    });
                }
                return new contracts.loadedcart(lc.id, lc.year, lc.progressive, lc.description, lc.areaid, lc.area, lc.locationid, lc.location, lc.cartid, lc.cart, lc.datein, null, lcDetails);

            }
            return null;
        });
    }.bind(this);

    this.createLoadedCart = function (token, loadedcart) {
        return $.ajax({
            url: this.loadedcarts,
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(loadedcart),
            processData: false,
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }.bind(this);

    this.updateLoadedCart = function (token, id, loadedcart) {
        return $.ajax({
            url: this.loadedcart.replace("{id}", id),
            type: 'put',
            contentType: 'application/json',
            data: JSON.stringify(loadedcart),
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
        getLoadedCart: this.getLoadedCart,
        createLoadedCart: this.createLoadedCart,
        updateLoadedCart: this.updateLoadedCart,
        deleteLoadedCart: this.deleteLoadedCart
    }
});
