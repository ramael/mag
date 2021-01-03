define(['knockout', 'text!./m-loadedcart.html'], function (ko, tplString) {

    function LoadedCartModel(params) {
        const self = this;
        self.root = params.value;
        self.parentitemid = params.parentitemid * 1;
        self.itemid = params.itemid * 1;

        self.loadedCart = ko.observable();

        self.warehouses = ko.observableArray();
        self.areas = ko.observableArray();
        self.locations = ko.observableArray();
        self.carts = ko.observableArray();
        
        self.selectedWarehouse = ko.observable();
        self.selectedArea = ko.observable();
        self.selectedLocation = ko.observable();
        self.selectedCart = ko.observable();

        self.selectedWarehouse.subscribe(function (warehouseid) {
            self.areas.removeAll();
            self.locations.removeAll();
            if (warehouseid) {
                self.getAreas(warehouseid);
            }
        });

        self.selectedArea.subscribe(function (areaid) {
            self.locations.removeAll();
            if (areaid) {
                self.getLocations(areaid);
            }
        });
               

        // Events
        self.isEdit = function () {
            return self.itemid !== -1;
        };

        self.addComponent= function (e) {
            console.log("addComponent", e);
        };

        self.editComponent = function (d) {
            console.log("editComponent", d);
        };
        
        self.removeComponent = function (d) {
            self.root.showModalConfirm(new self.root.contracts.modalConfirm("Component", "Confirm delete?", d.id, self.removeComponentConfirm));
        };
        self.removeComponentConfirm = function (d) {
            console.log("removeComponentConfirm", d);
        }

        self.doSubmit = function () {
            console.log("doSubmit");
            //if (self.isEdit()) {
            //    self.updateComponent();
            //} else {
            //    self.createComponent();
            //}
        };

        self.doCancel = function () {
            location.hash = "loadedcarts/" + self.parentitemid;
        };

        // Ajax
        self.getWarehouses = function () {
            self.root.apis.getWarehouses(self.root.user().token)
                        .done(function (data) {
                            if (data && data.length > 0) {
                                data.forEach(function (w) {
                                    self.warehouses.push(new self.root.contracts.warehouse(w.id, w.name, w.description, w.notes));
                                });
                                if (self.parentitemid && self.parentitemid !== -1) {
                                    self.selectedWarehouse(self.parentitemid);
                                }
                            }
                        }).fail(function (data) {
                            console.log("warehouses error", data);
                            self.warehouses.removeAll();
                            self.root.handleServerError(data);
                        });
        };

        self.getAreas = function (warehouseid) {
            self.root.apis.getWarehouseAreas(self.root.user().token, warehouseid)
                .done(function (data) {
                    if (data && data.length > 0) {
                        data.forEach(function (a) {
                            self.areas.push(new self.root.contracts.area(a.id, a.name, a.description, a.notes));
                        });
                    }
                }).fail(function (data) {
                    console.log("areas error", data);
                    self.areas.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getLocations = function (areaid) {
            self.root.apis.getAreaLocations(self.root.user().token, areaid)
                .done(function (data) {
                    if (data && data.length > 0) {
                        data.forEach(function (l) {
                            self.locations.push(new self.root.contracts.location(l.id, l.name, l.description, l.notes));
                        });
                    }
                }).fail(function (data) {
                    console.log("locations error", data);
                    self.locations.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getCarts = function () {
            self.root.apis.getCarts(self.root.user().token, "Available")
                .done(function (data) {
                    if (data && data.length > 0) {
                        data.forEach(function (c) {
                            self.carts.push(new self.root.contracts.cart(c.id, c.serialnumber, c.status));
                        });
                    }
                }).fail(function (data) {
                    console.log("carts error", data);
                    self.carts.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getLoadedCart = function (id) {
            self.root.apis.getLoadedCart(self.root.user().token, id)
                .done(function (data) {
                    if (data) {
                        const lcDetails = [];
                        if (data.loadedcartdetails && data.loadedcartdetails.length > 0) {
                            lcDetails = data.loadedcartdetails.map(function (lcd) {
                                const mc = null;
                                if (lcd.component) {
                                    mc = new self.root.contracts.component(lcd.component.id, lcd.component.code, lcd.component.description, lcd.component.notes);
                                }
                                return new self.root.contracts.loadedcartdetail(lcd.is, lcd.loadedcartid, lcd.componentid, mc, lcd.notes);
                            });
                        }
                        const mlc = new self.root.contracts.loadedcart(lc.id, lc.year, lc.progressive, lc.locationid, null, lc.cartid, null, lc.description, lc.datein, null, lcDetails);
                        self.loadedCart(mlc);                        
                    }
                }).fail(function (data) {
                    console.log("loaded cart error", data);
                    self.root.handleServerError(data);
                });
        };

        // Init
        self.getWarehouses();
        self.getCarts();

        const today = new Date();
        self.loadedCart(new self.root.contracts.loadedcart(0, today.getFullYear(), "", 0, null, 0, null, "", today.toISOString().split('.')[0] + "Z", null, []))
        if (self.isEdit()) {
            self.getLoadedCart(self.id);
        }
    }

    return { viewModel: LoadedCartModel, template: tplString };

});
