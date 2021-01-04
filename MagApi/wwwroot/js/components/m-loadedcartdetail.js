define(['knockout', 'text!./m-loadedcartdetail.html'], function (ko, tplString) {

    function LoadedCartDetailModel(params) {
        const self = this;
        self.root = params.value;
        self.data = params.data;
        self.modal = params.modal;

        self.components = ko.observableArray();
        self.notes = ko.observable();

        self.selectedComponent = ko.observable();        

        self.selectedComponent.subscribe(function (componentid) {
            self.data.componentid = componentid;           
        });

        self.notes.subscribe(function (value) {
            self.data.notes = value;
        });

        // Events
        self.isEdit = function () {
            return self.data.id !== 0;
        };

        self.doSubmit = function () {
            if (self.data.componentid === null || self.data.componentid === "undefined" || self.data.componentid === 0) {
                self.root.showModalMessage(new self.root.contracts.modalMessage("Validation", "Component is mandatory"));
                return;
            }
            if (self.modal) {
                self.data.notes = self.notes();
                self.data.componentid = self.selectedComponent();
                self.data.component = self.components().find(c => c.id === self.data.componentid);
                self.root.modalComponentStatus(self.data);
            }
        };

        self.doCancel = function () {
            if (self.modal) {
                self.root.modalComponentStatus("cancel");
            }
        };

        // Ajax
        self.getComponents= function () {
            self.root.apis.getComponents(self.root.user().token)
                .done(function (clist) {
                    if (clist && clist.length > 0) {
                        clist.forEach(function (c) {
                            self.components.push(c);
                        });
                        self.selectedComponent(self.data.componentid);                        
                    }
                }).fail(function (data) {
                    console.error("components error", data);
                    self.components.removeAll();
                    self.root.handleServerError(data);
                });
        };

        // Init
        self.getComponents();

        self.notes(self.data.notes);

    }

    return { viewModel: LoadedCartDetailModel, template: tplString };

});
