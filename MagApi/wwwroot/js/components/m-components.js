define(['knockout', 'text!./m-components.html'], function (ko, tplString) {

    function ComponentsModel(params) {
        const self = this;
        self.root = params.value;
        self.components = ko.observableArray();

        // Events
        self.addComponent = function (e) {
            console.log("addComponent", e);
            self.root.showModalComponent(new self.root.contracts.modalComponent("Add component", "m-component", -1, self.addComponentConfirm));
        };
        self.addComponentConfirm = function (d) {
            console.log("addComponentConfirm", d);
            location.hash = "components";
        }

        self.editComponent = function (d) {
            console.log("editComponent", d);
            self.root.showModalComponent(new self.root.contracts.modalComponent("Edit component", "m-component", d.id, self.editComponentConfirm));
        };
        self.editComponentConfirm = function (d) {
            console.log("editComponentConfirm", d);
            location.hash = "components";
        }

        self.removeComponent = function (d) {
            self.root.showModalConfirm(new self.root.contracts.modalConfirm("Component", "Confirm delete?", d.id, self.removeComponentConfirm));
        };
        self.removeComponentConfirm = function (d) {
            console.log("removeComponentConfirm", d);
            self.deleteComponent(d);
            location.hash = "components";
        }

        // Ajax
        self.getComponents = function () {
            $.ajax({
                url: self.root.apis.components,
                type: 'get',
                contentType: 'application/json',
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                if (data && data.length > 0) {
                    data.forEach(function (c) {
                        self.components.push(new self.root.contracts.component(c.id, c.code, c.description, c.notes));
                    });
                }
            }).fail(function (data) {
                console.error("components error", data);
                self.components.removeAll();
                self.root.handleServerError(data);
            });
        };

        self.deleteComponent = function (id) {
            $.ajax({
                url: self.root.apis.component.replace("{id}", id),
                type: 'delete',
                contentType: 'application/json',
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                self.components.removeAll();
                self.getComponents();
            }).fail(function (data) {
                console.error("delete component error", data);
                self.components.removeAll();
                self.root.handleServerError(data);
            });
        };

        // Init
        self.getComponents();
    }

    return { viewModel: ComponentsModel, template: tplString };

});
