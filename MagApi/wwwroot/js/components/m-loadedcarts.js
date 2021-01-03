define(['knockout', 'text!./m-loadedcarts.html'], function (ko, tplString) {

    function LoadedCartsModel(params) {
        const self = this;
        self.root = params.value;
        self.itemid = params.itemid;
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
            if (self.selectedWarehouse() && self.selectedWarehouse().id && d.id) {
                location.hash = "loadedcart/" + self.selectedWarehouse().id + "/" + d.id;
            }
        };
        
        self.removeLoadedCart = function (d) {
            self.root.showModalConfirm(new self.root.contracts.modalConfirm("Loaded cart", "Confirm delete?", d.id, self.removeLoadedCartConfirm));
        };
        self.removeLoadedCartConfirm = function (d) {
            console.log("removeLoadedCartConfirm", d);
            self.deleteLoadedCart(d);
            location.hash = "loadedcarts";
        }

        // Ajax
        self.getWarehouses = function () {
            self.root.apis.getWarehouses(self.root.user().token)
                .done(function (data) {
                    if (data && data.length > 0) {
                        data.forEach(function (w) {
                            self.warehouses.push(new self.root.contracts.warehouse(w.id, w.name, w.description, w.notes));
                        });
                    }
                }).fail(function (data) {
                    console.log("warehouses error", data);
                    self.warehouses.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getWarehouseLoadedCarts = function (warehouseid) {
            self.root.apis.getWarehouseLoadedCarts(self.root.user().token, warehouseid)
                .done(function (data) {
                    if (data && data.length > 0) {
                        data.forEach(function (lc) {
                            const mc = new self.root.contracts.cart(lc.cart.id, lc.cart.serialnumber, lc.cart.status);
                            const ma = new self.root.contracts.area(lc.location.area.id, lc.location.area.name, lc.location.area.description, null, null, null);
                            const ml = new self.root.contracts.location(lc.location.id, lc.location.name, lc.location.description, null, ma);
                            const mlc = new self.root.contracts.loadedcart(lc.id, lc.year, lc.progressive, lc.locationid, ml, lc.cartid, mc, lc.description, lc.datein, null, null);
                            self.warehouseLoadedCarts.push(mlc);
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
                    self.getWarehouseLoadedCarts(self.selectedWarehouse().id);
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
