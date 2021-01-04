define(['knockout', 'text!./m-loadedcarts.html'], function (ko, tplString) {

    function LoadedCartsModel(params) {
        const self = this;
        self.root = params.value;
        self.itemid = params.itemid * 1;
        self.warehouses = ko.observableArray();
        self.warehouseLoadedCarts= ko.observableArray();

        self.selectedWarehouse = ko.observable();
        self.selectedWarehouse.subscribe(function (warehouseid) {
            self.warehouseLoadedCarts.removeAll();
            if (warehouseid) {
                self.getWarehouseLoadedCarts(warehouseid);
            }
        });

        // Events
        self.addLoadedCart= function (e) {
            console.log("addLoadedCart", e);
            if (self.selectedWarehouse()) {
                location.hash = "loadedcart/" + self.selectedWarehouse() + "/-1";
            }
        };

        self.editLoadedCart= function (d) {
            console.log("editLoadedCart", d);
            if (self.selectedWarehouse() && d.id) {
                location.hash = "loadedcart/" + self.selectedWarehouse() + "/" + d.id;
            }
        };
        
        self.removeLoadedCart = function (d) {
            self.root.showModalConfirm(new self.root.contracts.modalConfirm("Loaded cart", "Confirm delete?", d.id, self.removeLoadedCartConfirm));
        };
        self.removeLoadedCartConfirm = function (d) {
            console.log("removeLoadedCartConfirm", d);
            self.deleteLoadedCart(d);
        }

        // Ajax
        self.getWarehouses = function () {
            self.root.apis.getWarehouses(self.root.user().token)
                .done(function (wlist) {
                    if (wlist && wlist.length > 0) {
                        wlist.forEach(function (w) {
                            self.warehouses.push(w);
                        });
                        if (self.itemid && self.itemid !== -1) {
                            self.selectedWarehouse(self.itemid);
                        }
                    }
                }).fail(function (data) {
                    console.log("warehouses error", data);
                    self.warehouses.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getWarehouseLoadedCarts = function (warehouseid) {
            self.root.apis.getWarehouseLoadedCarts(self.root.user().token, warehouseid)
                .done(function (lclist) {
                    if (lclist && lclist.length > 0) {
                        lclist.forEach(function (lc) {
                            self.warehouseLoadedCarts.push(lc);
                        });
                    }
                }).fail(function (data) {
                    console.log("warehouse loaded carts error", data);
                    self.warehouseLoadedCarts.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.deleteLoadedCart = function (id) {
            self.root.apis.deleteLoadedCart(self.root.user().token, id)
                .done(function (data) {
                    self.warehouseLoadedCarts.removeAll();
                    self.getWarehouseLoadedCarts(self.selectedWarehouse());
                }).fail(function (data) {
                    console.error("delete loaded cart error", data);
                    self.warehouseLoadedCarts.removeAll();
                    self.root.handleServerError(data);
                });
        };

        // Init
        self.getWarehouses();
    }

    return { viewModel: LoadedCartsModel, template: tplString };

});
