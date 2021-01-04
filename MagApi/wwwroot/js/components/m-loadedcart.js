define(['knockout', 'text!./m-loadedcart.html'], function (ko, tplString) {

    function LoadedCartModel(params) {
        const self = this;
        self.root = params.value;
        self.parentitemid = params.parentitemid * 1;
        self.itemid = params.itemid * 1;

        self.loadedCart = ko.observable();
        self.loadedCartDetails = ko.observableArray();
        self.area = ko.observable();
        self.location = ko.observable();
        self.cart = ko.observable();

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
            if (!self.isEdit()) {
                self.loadedCart().areaid = null;
                self.loadedCart().locationid = null;
            }
        });

        self.selectedArea.subscribe(function (areaid) {
            self.locations.removeAll();
            if (areaid) {
                self.getLocations(areaid);
            }
            self.loadedCart().areaid = areaid;
            self.loadedCart().locationid = null;
        });

        self.selectedLocation.subscribe(function (locationid) {
            self.loadedCart().locationid = locationid;
        });

        self.selectedCart.subscribe(function (cartid) {
            self.loadedCart().cartid = cartid;
        });

        // Events
        self.isEdit = function () {
            return self.itemid !== -1;
        };

        self.addComponent = function (e) {
            const d = new self.root.contracts.loadedcartdetail(0, self.loadedCart().id, 0, null, "");
            self.root.showModalComponent(new self.root.contracts.modalComponent("Add component", "m-loadedcartdetail", d, self.addComponentConfirm));
        };
        self.addComponentConfirm = function (d) {
            self.loadedCartDetails.push(d);
        }

        self.editComponent = function (d) {
            self.root.showModalComponent(new self.root.contracts.modalComponent("Edit component", "m-loadedcartdetail", d, self.editComponentConfirm));
        };
        self.editComponentConfirm = function (d) {
            self.loadedCartDetails.remove(function (lcd) {
                return lcd.id === d.id && lcd.loadedcartid === d.loadedcartid && lcd.componentid === d.componentid;
            });
            self.loadedCartDetails.push(d);
        }
        
        self.removeComponent = function (d) {
            self.root.showModalConfirm(new self.root.contracts.modalConfirm("Component", "Confirm delete?", d, self.removeComponentConfirm));
        };
        self.removeComponentConfirm = function (d) {
            self.loadedCartDetails.remove(function (lcd) {
                return lcd.id === d.id && lcd.loadedcartid === d.loadedcartid && lcd.componentid === d.componentid;
            });
        }

        self.doSubmit = function () {
            self.loadedCart().loadedcartdetails = self.loadedCartDetails();
            if (self.isEdit()) {
                self.updateLoadedCart();
            } else {
                self.createLoadedCart();
            }
        };

        self.doCancel = function () {
            location.hash = "loadedcarts/" + self.parentitemid;
        };

        // Ajax
        self.getWarehouses = function () {
             self.root.apis.getWarehouses(self.root.user().token)
                            .done(function (wlist) {
                                if (wlist && wlist.length > 0) {
                                    wlist.forEach(function (w) {
                                        self.warehouses.push(w);
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
                        .done(function (alist) {
                            if (alist && alist.length > 0) {
                                alist.forEach(function (a) {
                                    self.areas.push(a);
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
                        .done(function (llist) {
                            if (llist && llist.length > 0) {
                                llist.forEach(function (l) {
                                    self.locations.push(l);
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
                            .done(function (clist) {
                                if (clist && clist.length > 0) {
                                    clist.forEach(function (c) {
                                        self.carts.push(c);
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
                            .done(function (lc) {
                                if (lc) {
                                    self.loadedCart(lc);   
                                    self.loadedCartDetails.removeAll()
                                    if (lc.loadedcartdetails && lc.loadedcartdetails.length > 0) {
                                        lc.loadedcartdetails.forEach(function (d) {
                                            self.loadedCartDetails.push(d);
                                        });
                                    }
                                    self.area(lc.area.name + " - " + lc.area.description);
                                    self.location(lc.location.name + " - " + lc.location.description);
                                    self.cart(lc.cart.serialnumber);
                                    }
                            }).fail(function (data) {
                                console.log("loaded cart error", data);
                                self.root.handleServerError(data);
                            });
        };

        self.createLoadedCart= function () {
            self.root.apis.createLoadedCart(self.root.user().token, self.loadedCart())
                        .done(function (data) {
                            self.loadedCart("");
                            location.hash = "loadedcarts/" + self.parentitemid;
                        }).fail(function (data) {
                            console.error("create loaded cart error", data);
                            self.loadedCart("");
                            self.root.handleServerError(data);
                        });
        };

        self.updateLoadedCart = function () {
            self.root.apis.updateLoadedCart(self.root.user().token, self.loadedCart().id, self.loadedCart())
                        .done(function (data) {
                            self.loadedCart("");
                            location.hash = "loadedcarts/" + self.parentitemid;
                        }).fail(function (data) {
                            console.error("update loaded cart error", data);
                            self.loadedCart("");
                            self.root.handleServerError(data);
                        });
        };

        // Init
        const today = new Date();
        self.loadedCart(new self.root.contracts.loadedcart(0, today.getFullYear(), "", "", 0, null, 0, null, 0, null, today.toISOString().split('.')[0] + "Z", null, []))

        self.getWarehouses();
        if (self.isEdit()) {
            self.getLoadedCart(self.itemid);
        } else {
            self.getCarts();
        }

    }

    return { viewModel: LoadedCartModel, template: tplString };

});
