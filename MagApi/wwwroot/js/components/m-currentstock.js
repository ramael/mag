define(['knockout', 'text!./m-currentstock.html'], function (ko, tplString) {

    function CurrentStockModel(params) {
        const self = this;
        self.root = params.value;
        self.warehouses = ko.observableArray();
        self.warehouseCurrentStock = ko.observableArray();

        self.selectedWarehouse = ko.observable();
        self.selectedWarehouse.subscribe(function (warehouseid) {
            self.getWarehouseCurrentStock(warehouseid);
        });

        // Events
        self.getWarehouses = function () {
            $.ajax({
                url: self.root.apis.warehouses,
                type: 'get',
                contentType: 'application/json',
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                if (data && data.length > 0) {
                    data.forEach(function (w) {
                        self.warehouses.push(new self.root.contracts.warehouse(w.id, w.name, w.description, w.notes));
                    });
                }
            }).fail(function (data) {
                console.log("warehouses error", data);
                self.warehouses.removeAll();
            });
        };

        self.getWarehouseCurrentStock = function (warehouseid) {
            $.ajax({
                url: self.root.apis.warehouseCurrentStock.replace("{id}", warehouseid),
                type: 'get',
                contentType: 'application/json',
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                console.log("current stock", data);
                if (data && data.length > 0) {
                    data.forEach(function (cs) {
                        console.log("cs from server", cs);
                        const mcs = new self.root.contracts.stock(cs.componentcode, cs.componentdescription, cs.componentqty, null);
                        if (cs && cs.details.length > 0) {
                            mcs.details = cs.details.map(function (csd) {
                                return new self.root.contracts.stockdetail(csd.serialnumber, csd.areaname, csd.locationname);
                            });
                        }
                        self.warehouseCurrentStock.push(mcs);
                    });
                }
            }).fail(function (data) {
                console.log("warehouse current stocl error", data);
                self.warehouseCurrentStock.removeAll();
            });
        };

        self.getWarehouses();
    }

    return { viewModel: CurrentStockModel, template: tplString };

});
