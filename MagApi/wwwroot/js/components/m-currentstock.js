define(['knockout', 'text!./m-currentstock.html'], function (ko, tplString) {

    function CurrentStockModel(params) {
        const self = this;
        self.root = params.value;
        self.warehouses = ko.observableArray();
        self.warehouseCurrentStock = ko.observableArray();

        self.selectedWarehouse = ko.observable();
        self.selectedWarehouse.subscribe(function (warehouseid) {
            self.warehouseCurrentStock.removeAll();
            if (warehouseid) {
                self.getWarehouseCurrentStock(warehouseid);
            }
        });

        // Events
        self.getWarehouses = function () {
            self.root.apis.getWarehouses(self.root.user().token)
                .done(function (wlist) {
                    if (wlist && wlist.length > 0) {
                        wlist.forEach(function (w) {
                            self.warehouses.push(w);
                        });
                    }
                }).fail(function (data) {
                    console.log("warehouses error", data);
                    self.warehouses.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getWarehouseCurrentStock = function (warehouseid) {
            self.root.apis.getWarehouseCurrentStock(self.root.user().token, warehouseid)
                .done(function (cslist) {
                    if (cslist && cslist.length > 0) {
                        cslist.forEach(function (cs) {
                            self.warehouseCurrentStock.push(cs);
                        });
                    }
                }).fail(function (data) {
                    console.log("warehouse current stock error", data);
                    self.warehouseCurrentStock.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.getWarehouses();
    }

    return { viewModel: CurrentStockModel, template: tplString };

});
