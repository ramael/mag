define(['knockout', 'text!./m-components.html'], function (ko, tplString) {

    function ComponentsModel(params) {
        const self = this;
        self.root = params.value;
        self.components = ko.observableArray();

        // Events
        self.addComponent = function (e) {
            self.root.showModalComponent(new self.root.contracts.modalComponent("Add component", "m-component", -1, self.addComponentConfirm));
        };
        self.addComponentConfirm = function (d) {
            location.hash = "components";
        }

        self.editComponent = function (d) {
            self.root.showModalComponent(new self.root.contracts.modalComponent("Edit component", "m-component", d.id, self.editComponentConfirm));
        };
        self.editComponentConfirm = function (d) {
            location.hash = "components";
        }

        self.removeComponent = function (d) {
            self.root.showModalConfirm(new self.root.contracts.modalConfirm("Component", "Confirm delete?", d.id, self.removeComponentConfirm));
        };
        self.removeComponentConfirm = function (id) {
            self.deleteComponent(id);
            location.hash = "components";
        }

        // Ajax
        self.getComponents = function () {
            self.root.apis.getComponents(self.root.user().token)
                .done(function (clist) {
                    if (clist && clist.length > 0) {
                        clist.forEach(function (c) {
                            self.components.push(c);
                        });
                    }
                }).fail(function (data) {
                    console.error("components error", data);
                    self.components.removeAll();
                    self.root.handleServerError(data);
                });
        };

        self.deleteComponent = function (id) {
            self.root.apis.deleteComponent(self.root.user().token, id)
                .done(function (data) {
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
